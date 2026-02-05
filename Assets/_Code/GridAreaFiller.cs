using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridAreaFiller : MonoBehaviour
{
    [Tooltip("Min/Max arasýnda oluþturulacak reel sayýsý (inclusive)")]
    public Vector2 reelCountRange = new Vector2(1, 3);

    [Tooltip("Prefab of ReelController to instantiate into cells")]
    public ReelController reelPrefab;

    [Tooltip("Reference to the GridAreaController to fill")]
    public GirdAreaController gridAreaController;

    void Start()
    {
        // keep original behavior: fill at start
        FillGridRandomlyInternal();
    }

    [ContextMenu("Fill Grid Randomly")]
    public void FillGridRandomly()
    {
        FillGridRandomlyInternal();
    }

    void FillGridRandomlyInternal()
    {
        if (gridAreaController == null) gridAreaController = GetComponent<GirdAreaController>();
        if (gridAreaController == null)
        {
            Debug.LogWarning("GridAreaFiller: GridAreaController not assigned or found on same GameObject.");
            return;
        }

        if (reelPrefab == null)
        {
            Debug.LogWarning("GridAreaFiller: reelPrefab not assigned.");
            return;
        }

        // ensure gridArray is built
        gridAreaController.BuildGridArray();

        int countX = Mathf.Max(0, Mathf.RoundToInt(gridAreaController.size.x));
        int countZ = Mathf.Max(0, Mathf.RoundToInt(gridAreaController.size.y));
        if (countX == 0 || countZ == 0) return;

        int min = Mathf.RoundToInt(Mathf.Min(reelCountRange.x, reelCountRange.y));
        int max = Mathf.RoundToInt(Mathf.Max(reelCountRange.x, reelCountRange.y));
        int toSpawn = Random.Range(min, max + 1);

        // find parent transform where cells were created
        string parentName = $"GridArea_{gridAreaController.gameObject.name}";
        Transform parentTransform = gridAreaController.transform.Find(parentName);
        if (parentTransform == null)
        {
            Debug.LogWarning("GridAreaFiller: grid parent not found.");
            return;
        }

        // gather empty cell coords (do NOT clear existing reels; fill on top of current state)
        var emptyCells = new List<(int x, int z)>();
        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                bool occupied = gridAreaController.gridArray != null && gridAreaController.gridArray[x][z] != null && gridAreaController.gridArray[x][z].reel != null;
                if (!occupied) emptyCells.Add((x, z));
            }
        }

        if (emptyCells.Count == 0)
        {
            Debug.Log("GridAreaFiller: no empty cells to populate.");
            return;
        }

        // clamp spawn number
        toSpawn = Mathf.Clamp(toSpawn, 0, emptyCells.Count);

        // randomly pick cells and instantiate reels
        for (int i = 0; i < toSpawn; i++)
        {
            int pickIndex = Random.Range(0, emptyCells.Count);
            var coord = emptyCells[pickIndex];
            emptyCells.RemoveAt(pickIndex);

            string cellName = $"Cell_{coord.x}_{coord.z}";
            Transform cell = parentTransform.Find(cellName);
            if (cell == null)
            {
                Debug.LogWarning($"GridAreaFiller: cell transform {cellName} not found.");
                continue;
            }

#if UNITY_EDITOR
            GameObject instGO = null;
            var created = PrefabUtility.InstantiatePrefab(reelPrefab.gameObject) as GameObject;
            if (created != null)
            {
                instGO = created;
                instGO.transform.SetParent(cell, false);
                Undo.RegisterCreatedObjectUndo(instGO, "Create Reel");
            }
            else
            {
                instGO = Instantiate(reelPrefab.gameObject, cell);
                Undo.RegisterCreatedObjectUndo(instGO, "Create Reel");
            }
            var inst = instGO.GetComponent<ReelController>();
#else
            var inst = Instantiate(reelPrefab, cell);
#endif

            // assign random color and level to the instantiated reel
            if (inst != null)
            {
                // pick random color from enum
                var colors = System.Enum.GetValues(typeof(ReelColorId));
                ReelColorId randColor = (ReelColorId)colors.GetValue(Random.Range(0, colors.Length));

                // determine max level from levelGraphics if available, otherwise default to 3
                int maxLevel = 3;
                if (inst.levelGraphics != null && inst.levelGraphics.Length > 0) maxLevel = inst.levelGraphics.Length;

                // Restrict maximum level to current order level if OrderManager exists
                if (OrderManager.Instance != null && OrderManager.Instance.orders != null && OrderManager.Instance.orders.Length > 0)
                {
                    int idx = Mathf.Clamp(OrderManager.Instance.currentOrder, 0, OrderManager.Instance.orders.Length - 1);
                    int orderLevel = OrderManager.Instance.orders[idx].level;
                    // ensure at least 1
                    orderLevel = Mathf.Max(1, orderLevel);
                    maxLevel = Mathf.Min(maxLevel, orderLevel);
                }

                int randLevel = Random.Range(1, maxLevel + 1);

                // set data (isOrder = false)
                inst.SetData(randLevel, randColor, false);

                // ensure naming and local transform for instantiated prefab
                inst.gameObject.name = $"Reel_{coord.x}_{coord.z}";
                inst.transform.localPosition = Vector3.zero;
                inst.transform.localRotation = Quaternion.identity;
                inst.transform.localScale = Vector3.one;

                // update grid array entry to reference this reel
                var cellComp = gridAreaController.gridArray[coord.x][coord.z];
                if (cellComp != null)
                {
                    cellComp.reel = inst;
                }
            }
        }

        // refresh to ensure consistency
        gridAreaController.BuildGridArray();

        Debug.Log($"GridAreaFiller: instantiated {toSpawn} reels.");
    }
}
