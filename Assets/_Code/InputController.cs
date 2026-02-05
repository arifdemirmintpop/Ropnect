using UnityEngine;

public class InputController : MonoBehaviour
{
    // Now store selected cell instead of direct ReelController
    public GridAreaCell firstSelectedCell;
    public LayerMask layer;

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
        bool canReach = true;
        if (grid != null)
        {
            Vector2 fromCoord = firstSelectedCell.coordinate;
            Vector2 toCoord = secondCell.coordinate;
            if (!grid.TryToReach(fromCoord, toCoord, out var path))
            {
                canReach = false;
            }
        }
        else
        {
            // fallback to simple distance check if no grid controller found
            float maxReach = 5f;
            if (Vector3.Distance(first.transform.position, second.transform.position) > maxReach)
            {
                canReach = false;
            }
        }

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
    }

    void OnWrongColor(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) firstSelectedCell.reel.Select();
    }

    void OnCantReach(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) firstSelectedCell.reel.Select();
    }

    void OnTooMuchTurn(GridAreaCell secondCell)
    {
        ResetFirstSelectionVisual(firstSelectedCell);
        firstSelectedCell = secondCell;
        if (firstSelectedCell.reel != null) firstSelectedCell.reel.Select();
    }
}
