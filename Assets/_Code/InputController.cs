using DG.Tweening;
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


        if (!canReach)
        {
            // draw whatever path was returned (if any) regardless of canReach
            DrawPath(path, grid, Color.red);
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
        // draw whatever path was returned (if any) regardless of canReach
        DrawPath(path, grid, Color.green);
        // If all good, CanMerge
        OnCanMerge(secondCell);
    }

    void OnCanMerge(GridAreaCell secondCell)
    {
        var firstCell = firstSelectedCell;
        if (firstCell == null || firstCell.reel == null || secondCell == null || secondCell.reel == null)
        {
            firstSelectedCell = null;
            return;
        }

        // Increase level of second reel
        secondCell.reel.LevelUp(ClearPath);

        // animate first to scale 0 then destroy
        var toRemove = firstCell.reel;
        if (toRemove != null)
        {
            toRemove.Deselect();
            // clear cell reference immediately
            firstCell.reel = null;
            toRemove.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
            {
                Destroy(toRemove.gameObject);
            });
        }

        // after merge reset first selection
        firstSelectedCell = null;

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
        if (firstSelectedCell.reel != null) 
            firstSelectedCell.reel.Select();
    }

    void OnWrongColor(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) 
            firstSelectedCell.reel.Select();
    }

    void OnCantReach(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) 
            firstSelectedCell.reel.Select();
    }

    void OnTooMuchTurn(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) 
            firstSelectedCell.reel.Select();
    }


    void DrawPath(Vector2[] path, GirdAreaController grid, Color lineColor)
    {
        // handle null or empty path
        if (path == null || path.Length == 0)
        {
            ClearPath();
            return;
        }

        // ensure a LineRenderer exists
        if (pathRenderer == null)
        {
            var go = new GameObject("PathRenderer");
            go.transform.SetParent(this.transform, false);
            pathRenderer = go.AddComponent<LineRenderer>();
            // simple default material
            var mat = new Material(Shader.Find("Sprites/Default"));
            pathRenderer.material = mat;
            pathRenderer.widthMultiplier = 0.12f;
            pathRenderer.useWorldSpace = true;
            pathRenderer.loop = false;
            pathRenderer.numCapVertices = 8;
        }

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
                positions[i] = grid.transform.TransformPoint(localPos) + Vector3.up * pathHeightOffset;
            }

        }

        // apply positions
        pathRenderer.positionCount = positions.Length;
        pathRenderer.SetPositions(positions);
        pathRenderer.enabled = true;

        // apply color to line renderer (start/end and gradient)
        try
        {
            pathRenderer.startColor = lineColor;
            pathRenderer.endColor = lineColor;
        }
        catch { }

        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(lineColor, 0f), new GradientColorKey(lineColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(lineColor.a, 0f), new GradientAlphaKey(lineColor.a, 1f) }
        );
        pathRenderer.colorGradient = grad;

        // lock clearing for a short moment so immediate ClearPath calls don't hide it
    }

    void ClearPath()
    {
        if (pathRenderer == null) return;
        pathRenderer.positionCount = 0;
        pathRenderer.enabled = false;
        Debug.Log("ClearPath: path cleared.");
    }
}
