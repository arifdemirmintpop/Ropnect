using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GirdAreaController : MonoBehaviour
{
    [Tooltip("Grid boyutu (X = sütun sayýsý, Y = satýr sayýsý)")]
    public Vector2 size = new Vector2(1, 1);

    [Tooltip("Hücreler arasý offset (X -> dünya X, Y -> dünya Z)")]
    public Vector2 offset = Vector2.one;

    public GameObject prefab;

    // changed to hold GridAreaCell instead of ReelController
    [HideInInspector]
    public GridAreaCell[][] gridArray;

    // Context menüden çalýþtýrýlabilir: Editor'de seçilen prefabý size ve offset'e göre X/Z eksenlerinde çoðaltýr
    [ContextMenu("Create Grid Area")]
    public void CreateGridArea()
    {
        if (prefab == null)
        {
            Debug.LogWarning("GirdAreaController: Prefab atanmadý.");
            return;
        }

        int countX = Mathf.Max(0, Mathf.RoundToInt(size.x));
        int countZ = Mathf.Max(0, Mathf.RoundToInt(size.y));

        if (countX == 0 || countZ == 0)
        {
            Debug.LogWarning("GirdAreaController: size X veya Y sýfýr veya negatif. Doðru bir boyut girin.");
            return;
        }

        // Parent nesne adý ve hazýrlanmasý
        string parentName = $"GridArea_{gameObject.name}";
        Transform parentTransform = transform.Find(parentName);

#if UNITY_EDITOR
        // Eðer önceden oluþturulmuþ parent varsa sil (Undo destekli)
        if (parentTransform != null)
        {
            Undo.DestroyObjectImmediate(parentTransform.gameObject);
            parentTransform = null;
        }

        GameObject parentGO = new GameObject(parentName);
        Undo.RegisterCreatedObjectUndo(parentGO, "Create Grid Area Parent");
        parentTransform = parentGO.transform;
#else
        if (parentTransform != null)
        {
            DestroyImmediate(parentTransform.gameObject);
            parentTransform = null;
        }

        GameObject parentGO = new GameObject(parentName);
        parentTransform = parentGO.transform;
#endif

        // Parent'ý bu GameObject'in çocuðý yap ve yerini sýfýrla
        parentTransform.SetParent(transform, false);
        parentTransform.localPosition = Vector3.zero;
        parentTransform.localRotation = Quaternion.identity;
        parentTransform.localScale = Vector3.one;

        // Hesaplama: X ve Z eksenlerinde ortalamak için baþlangýç ofsetlerini hesapla
        float totalWidth = (countX - 1) * offset.x; // toplam geniþlik hücreler arasý uzaklýkla
        float startX = -totalWidth * 0.5f;
        float totalDepth = (countZ - 1) * offset.y; // toplam derinlik (Z)
        float startZ = -totalDepth * 0.5f;

        // Oluþturma döngüsü (X -> world X, Z -> world Z)
        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                // Lokal pozisyon parent içinde, X ve Z eksenlerinde ortalanmýþ
                Vector3 localPos = new Vector3(startX + x * offset.x, 0f, startZ + z * offset.y);

                // Instantiate GridAreaCell prefab (expected to contain GridAreaCell component)
                GameObject instance = Instantiate(prefab.gameObject, parentTransform);
                instance.name = $"Cell_{x}_{z}";

                // Transform'u prefabýn transformuna göre ayarla (pozisyon, rotasyon, ölçek)
                instance.transform.localPosition = localPos;
                instance.transform.localRotation = prefab.transform.localRotation;
                instance.transform.localScale = prefab.transform.localScale;

                // try to set GridAreaCell component coordinate
                var cellComp = instance.GetComponent<GridAreaCell>();
                if (cellComp != null)
                {
                    cellComp.SetCoordinate(x, z);
                    cellComp.reel = null;
                }

                // Deactivate border cells so they are not active/interactive
                if (x == 0 || z == 0 || x == countX - 1 || z == countZ - 1)
                {
                    instance.SetActive(false);
                }

#if UNITY_EDITOR
                // Undo desteði ile oluþturmayý kaydet
                Undo.RegisterCreatedObjectUndo(instance, "Create Grid Cell");
#endif
            }
        }

#if UNITY_EDITOR
        // Seçimi parent nesneye geçir (kullanýcýya görünürlük için)
        Selection.activeGameObject = parentTransform.gameObject;
#endif

        Debug.Log($"GirdAreaController: {countX * countZ} hücre oluþturuldu ({countX}x{countZ}).");

        // Build grid array after creation
        BuildGridArray();
    }

    // Build or refresh the internal gridArray from child objects named "Cell_x_z"
    public void BuildGridArray()
    {
        int countX = Mathf.Max(0, Mathf.RoundToInt(size.x));
        int countZ = Mathf.Max(0, Mathf.RoundToInt(size.y));
        gridArray = new GridAreaCell[countX][];
        for (int x = 0; x < countX; x++)
        {
            gridArray[x] = new GridAreaCell[countZ];
            for (int z = 0; z < countZ; z++) gridArray[x][z] = null;
        }

        string parentName = $"GridArea_{gameObject.name}";
        Transform parentTransform = transform.Find(parentName);
        if (parentTransform == null) return;

        foreach (Transform child in parentTransform)
        {
            // expecting name Cell_x_z
            string n = child.name;
            if (!n.StartsWith("Cell_")) continue;
            var parts = n.Split('_');
            if (parts.Length < 3) continue;
            if (!int.TryParse(parts[1], out int x)) continue;
            if (!int.TryParse(parts[2], out int z)) continue;

            if (x < 0 || z < 0) continue;
            if (x >= countX || z >= countZ) continue;

            var cell = child.GetComponent<GridAreaCell>();
            if (cell == null)
            {
                // attempt to add GridAreaCell to this transform so gridArray always filled
                cell = child.gameObject.AddComponent<GridAreaCell>();
                cell.SetCoordinate(x, z);
                cell.reel = child.GetComponent<ReelController>();
            }

            gridArray[x][z] = cell; // can be null if no GridAreaCell on the cell
        }
    }

    // Try to find a path from 'from' to 'to' using A* over the grid indices.
    // Movement is allowed only through empty cells (gridArray[x][z] == null or gridArray[x][z].reel == null).
    // The start and end cells are allowed to be occupied (they are considered walkable).
    // from/to are expected to contain integer-like coordinates (x,z). Returns true if a path found and outputs the path as array of Vector2 (grid indices).
    // Updated: Instead of shortest path, choose path with minimum number of turns (direction changes). Distance is ignored.
    public bool TryToReach(Vector2 from, Vector2 to, out Vector2[] path)
    {
        BuildGridArray(); // ensure up-to-date

        int countX = gridArray?.Length ?? 0;
        int countZ = (countX > 0) ? gridArray[0].Length : 0;
        path = null;
        if (countX == 0 || countZ == 0) return false;

        int sx = Mathf.RoundToInt(from.x);
        int sz = Mathf.RoundToInt(from.y);
        int tx = Mathf.RoundToInt(to.x);
        int tz = Mathf.RoundToInt(to.y);

        if (sx < 0 || sx >= countX || sz < 0 || sz >= countZ) return false;
        if (tx < 0 || tx >= countX || tz < 0 || tz >= countZ) return false;

        // if same cell
        if (sx == tx && sz == tz)
        {
            path = new Vector2[] { new Vector2(sx, sz) };
            return true;
        }

        // Dijkstra-like search minimizing number of turns (direction changes). Distance is ignored.
        // State includes current direction to count turns properly.
        int dirs = neighborOffsets.Length; // should be 4

        // bestTurns[x,z,dir] = minimal turns to reach (x,z) when arriving with direction 'dir'
        int[,,] bestTurns = new int[countX, countZ, dirs];
        for (int x = 0; x < countX; x++) for (int z = 0; z < countZ; z++) for (int d = 0; d < dirs; d++) bestTurns[x, z, d] = int.MaxValue;

        var open = new List<TurnNode>();

        // start node
        TurnNode startNode = new TurnNode(sx, sz, -1, 0, 0, null);
        open.Add(startNode);

        while (open.Count > 0)
        {
            // pick node with minimal turns, tie-breaker minimal steps
            TurnNode current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                var n = open[i];
                if (n.turns < current.turns || (n.turns == current.turns && n.steps < current.steps)) current = n;
            }
            open.Remove(current);

            // expand neighbors
            foreach (var nOff in neighborOffsets)
            {
                int ndir = GetDirectionIndex(nOff.x, nOff.z);
                int nx = current.x + nOff.x;
                int nz = current.z + nOff.z;
                if (nx < 0 || nz < 0 || nx >= countX || nz >= countZ) continue;

                // walkable if empty OR it's the target OR it's the start
                bool walkable = (gridArray[nx][nz] == null || gridArray[nx][nz].reel == null) || (nx == tx && nz == tz) || (nx == sx && nz == sz);
                if (!walkable) continue;

                int newTurns = current.dir == -1 ? 0 : (current.dir == ndir ? current.turns : current.turns + 1);
                if (newTurns >= 4) continue; // reject paths with 4 or more turns

                int newSteps = current.steps + 1;

                // if this state improves bestTurns, push
                if (newTurns < bestTurns[nx, nz, ndir])
                {
                    bestTurns[nx, nz, ndir] = newTurns;
                    var node = new TurnNode(nx, nz, ndir, newTurns, newSteps, current);

                    // if reached target, reconstruct path
                    if (nx == tx && nz == tz)
                    {
                        // build path by walking parents (includes start node with dir=-1)
                        var rev = new List<Vector2>();
                        TurnNode p = node;
                        while (p != null)
                        {
                            rev.Add(new Vector2(p.x, p.z));
                            p = p.parent;
                        }
                        rev.Reverse();
                        path = rev.ToArray();
                        return true;
                    }

                    open.Add(node);
                }
            }
        }

        return false;
    }

    // helper to map offset to direction index
    static int GetDirectionIndex(int dx, int dz)
    {
        // neighborOffsets: { (1,0), (-1,0), (0,1), (0,-1) }
        if (dx == 1 && dz == 0) return 0;
        if (dx == -1 && dz == 0) return 1;
        if (dx == 0 && dz == 1) return 2;
        return 3;
    }

    class Node
    {
        public int x, z;
        public int g, h, f;
        public Node parent;

        public Node(int x, int z, int g, int h, Node parent)
        {
            this.x = x; this.z = z; this.g = g; this.h = h; this.f = g + h; this.parent = parent;
        }
    }

    // TurnNode moved to class scope to be a valid C# type
    class TurnNode
    {
        public int x, z;
        public int dir; // -1 = start (no direction yet), otherwise 0..3 matching neighborOffsets
        public int turns; // number of direction changes so far
        public int steps; // number of steps taken (used only to break ties)
        public TurnNode parent;

        public TurnNode(int x, int z, int dir, int turns, int steps, TurnNode parent)
        {
            this.x = x; this.z = z; this.dir = dir; this.turns = turns; this.steps = steps; this.parent = parent;
        }
    }

    static readonly (int x, int z)[] neighborOffsets = new (int x, int z)[] { (1,0), (-1,0), (0,1), (0,-1) };

    static int Heuristic(int x1, int z1, int x2, int z2)
    {
        // Manhattan distance
        return Mathf.Abs(x1 - x2) + Mathf.Abs(z1 - z2);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}