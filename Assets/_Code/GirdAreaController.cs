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

        // A* implementation
        bool[,] closed = new bool[countX, countZ];
        Node[,] nodes = new Node[countX, countZ];
        var open = new List<Node>();

        Node start = new Node(sx, sz, 0, Heuristic(sx, sz, tx, tz), null);
        nodes[sx, sz] = start;
        open.Add(start);

        while (open.Count > 0)
        {
            // get node with lowest f
            Node current = open[0];
            for (int i = 1; i < open.Count; i++) if (open[i].f < current.f) current = open[i];
            open.Remove(current);

            if (current.x == tx && current.z == tz)
            {
                // reconstruct path
                var rev = new List<Vector2>();
                Node p = current;
                while (p != null)
                {
                    rev.Add(new Vector2(p.x, p.z));
                    p = p.parent;
                }
                rev.Reverse();

                // count direction changes (turns) along the path
                int turns = 1;
                if (rev.Count >= 3)
                {
                    Vector2 prevDir = rev[1] - rev[0];
                    for (int i = 2; i < rev.Count; i++)
                    {
                        Vector2 dir = rev[i] - rev[i - 1];
                        if (dir != prevDir) turns++;
                        prevDir = dir;
                        // early exit: if turns reach 4 or more, reject this path
                        if (turns >= 4) break;
                    }
                }

                if (turns >= 4)
                {
                    // reject this found path and continue searching for alternative paths
                    continue;
                }

                path = rev.ToArray();
                return true;
            }

            closed[current.x, current.z] = true;

            // neighbors 4-dir
            foreach (var nOff in neighborOffsets)
            {
                int nx = current.x + nOff.x;
                int nz = current.z + nOff.z;
                if (nx < 0 || nz < 0 || nx >= countX || nz >= countZ) continue;
                if (closed[nx, nz]) continue;

                // walkable if empty OR it's the target OR it's the start
                bool walkable = (gridArray[nx][nz] == null || gridArray[nx][nz].reel == null) || (nx == tx && nz == tz) || (nx == sx && nz == sz);
                if (!walkable) continue;

                int ng = current.g + 1;
                Node existing = nodes[nx, nz];
                if (existing == null)
                {
                    existing = new Node(nx, nz, ng, Heuristic(nx, nz, tx, tz), current);
                    nodes[nx, nz] = existing;
                    open.Add(existing);
                }
                else if (ng < existing.g)
                {
                    existing.g = ng;
                    existing.f = ng + existing.h;
                    existing.parent = current;
                    if (!open.Contains(existing)) open.Add(existing);
                }
            }
        }

        return false;
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