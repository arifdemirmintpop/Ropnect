using UnityEngine;

public class InputController : MonoBehaviour
{
    // Now store selected cell instead of direct ReelController
    public GridAreaCell firstSelectedCell;
    public LayerMask layer;

    // optional LineRenderer to visualize path; will be created if null
    public LineRenderer pathRenderer;

    // vertical offset to raise the line above ground so it's visible
    const float pathHeightOffset = 0.15f;

    // short lock to prevent ClearPath immediately after DrawPath
    const float pathLockDuration = 0.25f;
    float pathLockUntil = 0f;

    void Update()
    {
        ListenClick();
    }

    void ListenClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var cell = TryGetCell();
        if (cell == null)
        {
            // clicked empty space -> deselect current
            if (firstSelectedCell != null && firstSelectedCell.reel != null)
            {
                firstSelectedCell.reel.Deselect();
            }
            firstSelectedCell = null;

            // clear path visualization
            ClearPath();
            return;
        }

        // if cell has no reel, treat as empty (deselect)
        if (cell.reel == null)
        {
            if (firstSelectedCell != null && firstSelectedCell.reel != null)
            {
                firstSelectedCell.reel.Deselect();
            }
            firstSelectedCell = null;

            // clear path visualization
            ClearPath();
            return;
        }

        if (firstSelectedCell == null)
        {
            OnFirstSelected(cell);
        }
        else if (firstSelectedCell == cell)
        {
            // same one, ignore
            return;
        }
        else
        {
            OnSecondSelected(cell);
        }
    }

    GridAreaCell TryGetCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f, layer))
        {
            // prefer GridAreaCell in parents
            var cell = hit.collider.GetComponentInParent<GridAreaCell>();
            if (cell != null) return cell;

            // fallback: if hit a Reel, try its parent transform for a cell
            var reel = hit.collider.GetComponentInParent<ReelController>();
            if (reel != null && reel.transform.parent != null)
            {
                var parentCell = reel.transform.parent.GetComponent<GridAreaCell>();
                if (parentCell != null) return parentCell;
            }
        }
        return null;
    }

    void OnFirstSelected(GridAreaCell cell)
    {
        // deselect previous just in case
        if (firstSelectedCell != null && firstSelectedCell.reel != null)
        {
            firstSelectedCell.reel.Deselect();
        }

        firstSelectedCell = cell;
        // visual highlight via reel
        firstSelectedCell.reel.Select();

        // clear any previous path
        ClearPath();
    }

    void OnSecondSelected(GridAreaCell secondCell)
    {
        // Decision tree based on simple checks
        if (firstSelectedCell == null || firstSelectedCell.reel == null)
        {
            firstSelectedCell = secondCell;
            if (firstSelectedCell.reel != null) firstSelectedCell.reel.Select();
            return;
        }

        var first = firstSelectedCell.reel;
        var second = secondCell.reel;
        if (first == null || second == null) return;

        // Reach check: use grid connectivity via GirdAreaController.TryToReach
        var grid = FindObjectOfType<GirdAreaController>();
        Vector2 fromCoord = firstSelectedCell.coordinate;
        Vector2 toCoord = secondCell.coordinate;

        // call TryToReach and capture returned path (may be null)
        bool canReach = grid.TryToReach(fromCoord, toCoord, out var path);

        Debug.Log($"InputController: TryToReach returned canReach={canReach}, pathLength={(path==null?0:path.Length)}");

        // draw whatever path was returned (if any) regardless of canReach
        DrawPath(path, grid);

        if (!canReach)
        {
            OnCantReach(secondCell);
            return;
        }

        // Color check
        if (first.color != second.color)
        {
            OnWrongColor(secondCell);
            return;
        }

        // Level check
        if (first.level != second.level)
        {
            OnWrongLevel(secondCell);
            return;
        }

        // If all good, CanMerge
        OnCanMerge(secondCell);
    }

    void OnCanMerge(GridAreaCell secondCell)
    {
        var firstCell = firstSelectedCell;
        if (firstCell == null || firstCell.reel == null || secondCell == null || secondCell.reel == null)
        {
            firstSelectedCell = null;
            ClearPath();
            return;
        }

        // Increase level of second reel
        secondCell.reel.LevelUp();

        // animate first to scale 0 then destroy
        var toRemove = firstCell.reel;
        if (toRemove != null)
        {
            toRemove.Deselect();
            // clear cell reference immediately
            firstCell.reel = null;
            StartCoroutine(ScaleAndDestroy(toRemove.gameObject));
        }

        // after merge reset first selection
        firstSelectedCell = null;

        // clear path visualization
        ClearPath();
    }

    System.Collections.IEnumerator ScaleAndDestroy(GameObject go)
    {
        float dur = 0.3f;
        Vector3 start = go.transform.localScale;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            go.transform.localScale = Vector3.Lerp(start, Vector3.zero, t / dur);
            yield return null;
        }
        Destroy(go);
    }

    void ResetFirstSelectionVisual(GridAreaCell firstCell)
    {
        // call Deselect to trigger visual return
        if (firstCell != null && firstCell.reel != null)
        {
            firstCell.reel.Deselect();
        }
    }

    void OnWrongLevel(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        // set second as new first
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) firstSelectedCell.reel.Select();

        // do not clear path here so user can inspect the proposed route
    }

    void OnWrongColor(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) firstSelectedCell.reel.Select();

        // do not clear path here so user can inspect the proposed route
    }

    void OnCantReach(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) firstSelectedCell.reel.Select();

        // keep the drawn path (if any) so user can see attempted route
    }

    void OnTooMuchTurn(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) firstSelectedCell.reel.Select();

        // keep the path so the player can see why it's invalid
    }

    // LineRenderer helpers
    void EnsurePathRenderer()
    {
        if (pathRenderer != null) return;
        pathRenderer = GetComponent<LineRenderer>();
        if (pathRenderer != null) return;

        Debug.Log("InputController: creating LineRenderer for path visualization.");
        pathRenderer = gameObject.AddComponent<LineRenderer>();
        pathRenderer.material = new Material(Shader.Find("Sprites/Default"));
        pathRenderer.widthMultiplier = 0.1f;
        // prefer an unlit color shader for clear visibility
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = Color.cyan;
        pathRenderer.material = mat;
        // set widths
        pathRenderer.startWidth = 0.25f;
        pathRenderer.endWidth = 0.25f;
        pathRenderer.loop = false;
        pathRenderer.positionCount = 0;
        pathRenderer.numCapVertices = 2;
        pathRenderer.numCapVertices = 8;
        pathRenderer.useWorldSpace = true;
        pathRenderer.startColor = Color.cyan;
        pathRenderer.endColor = Color.cyan;
        pathRenderer.enabled = false;

        // rendering tweaks for visibility
        pathRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        pathRenderer.receiveShadows = false;
        pathRenderer.alignment = LineAlignment.View;
        var rend = pathRenderer.GetComponent<Renderer>();
        if (rend != null) rend.sortingOrder = 1000;
    }

    void DrawPath(Vector2[] path, GirdAreaController grid)
    {
        if (path == null || path.Length == 0)
        {
            ClearPath();
            Debug.Log("DrawPath: no path to draw.");
            return;
        }

        EnsurePathRenderer();

        Debug.Log($"DrawPath: drawing path with {path.Length} nodes.");

        var positions = new Vector3[path.Length];

        // try to find world positions from existing cell transforms
        string parentName = $"GridArea_{grid.gameObject.name}";
        Transform parentTransform = grid.transform.Find(parentName);

        for (int i = 0; i < path.Length; i++)
        {
            int x = Mathf.RoundToInt(path[i].x);
            int z = Mathf.RoundToInt(path[i].y);
            Transform cellT = null;
            if (parentTransform != null)
            {
                string cellName = $"Cell_{x}_{z}";
                var ct = parentTransform.Find(cellName);
                if (ct != null) cellT = ct;
            }

            if (cellT != null)
            {
                positions[i] = cellT.position;
                positions[i] = cellT.position + Vector3.up * pathHeightOffset;
            }
            else
            {
                // fallback: compute approximate world position based on grid offsets and parent's transform
                float countX = Mathf.Max(0, Mathf.RoundToInt(grid.size.x));
                float countZ = Mathf.Max(0, Mathf.RoundToInt(grid.size.y));
                float totalWidth = (countX - 1) * grid.offset.x;
                float startX = -totalWidth * 0.5f;
                float totalDepth = (countZ - 1) * grid.offset.y;
                float startZ = -totalDepth * 0.5f;

                Vector3 localPos = new Vector3(startX + x * grid.offset.x, 0f, startZ + z * grid.offset.y);
                positions[i] = grid.transform.TransformPoint(localPos);
                positions[i] = grid.transform.TransformPoint(localPos) + Vector3.up * pathHeightOffset;
            }

            Debug.Log($"DrawPath: node {i} -> grid ({path[i].x},{path[i].y}) -> world {positions[i]}");
        }

        pathRenderer.positionCount = positions.Length;
        pathRenderer.SetPositions(positions);
        pathRenderer.enabled = true;
        // lock clearing for a short moment so immediate ClearPath calls don't hide it
        pathLockUntil = Time.time + pathLockDuration;
        Debug.Log($"DrawPath: renderer enabled, positionCount={pathRenderer.positionCount}");
    }

    void ClearPath()
    {
        // respect temporary lock to avoid immediate clearing after drawing
        if (Time.time < pathLockUntil) return;

        if (pathRenderer == null) return;
        pathRenderer.positionCount = 0;
        pathRenderer.enabled = false;
        Debug.Log("ClearPath: path cleared.");
    }
}
