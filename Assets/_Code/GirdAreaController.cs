using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GirdAreaController : MonoBehaviour
{
    [Tooltip("Grid boyutu (X = s�tun say�s�, Y = sat�r say�s�)")]
    public Vector2 size = new Vector2(1, 1);

    [Tooltip("H�creler aras� offset (X -> d�nya X, Y -> d�nya Z)")]
    public Vector2 offset = Vector2.one;

    public GameObject prefab;

    // changed to hold GridAreaCell instead of ReelController
    [HideInInspector]
    public GridAreaCell[][] gridArray;

    // Context men�den �al��t�r�labilir: Editor'de se�ilen prefab� size ve offset'e g�re X/Z eksenlerinde �o�alt�r
    [ContextMenu("Create Grid Area")]
    public void CreateGridArea()
    {
        if (prefab == null)
        {
            Debug.LogWarning("GirdAreaController: Prefab atanmad�.");
            return;
        }

        int countX = Mathf.Max(0, Mathf.RoundToInt(size.x));
        int countZ = Mathf.Max(0, Mathf.RoundToInt(size.y));

        if (countX == 0 || countZ == 0)
        {
            Debug.LogWarning("GirdAreaController: size X veya Y s�f�r veya negatif. Do�ru bir boyut girin.");
            return;
        }

        // Parent nesne ad� ve haz�rlanmas�
        string parentName = $"GridArea_{gameObject.name}";
        Transform parentTransform = transform.Find(parentName);

#if UNITY_EDITOR
        // E�er �nceden olu�turulmu� parent varsa sil (Undo destekli)
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

        // Parent'� bu GameObject'in �ocu�� yap ve yerini s�f�rla
        parentTransform.SetParent(transform, false);
        parentTransform.localPosition = Vector3.zero;
        parentTransform.localRotation = Quaternion.identity;
        parentTransform.localScale = Vector3.one;

        // Hesaplama: X ve Z eksenlerinde ortalamak i�in ba�lang�� ofsetlerini hesapla
        float totalWidth = (countX - 1) * offset.x; // toplam geni�lik h�creler aras� uzakl�kla
        float startX = -totalWidth * 0.5f;
        float totalDepth = (countZ - 1) * offset.y; // toplam derinlik (Z)
        float startZ = -totalDepth * 0.5f;

        // Olu�turma d�ng�s� (X -> world X, Z -> world Z)
        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                // Lokal pozisyon parent i�inde, X ve Z eksenlerinde ortalanm��
                Vector3 localPos = new Vector3(startX + x * offset.x, 0f, startZ + z * offset.y);

                // Instantiate GridAreaCell prefab (expected to contain GridAreaCell component)
                GameObject instance = Instantiate(prefab.gameObject, parentTransform);
                instance.name = $"Cell_{x}_{z}";

                // Transform'u prefab�n transformuna g�re ayarla (pozisyon, rotasyon, �l�ek)
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
                // Undo deste�i ile olu�turmay� kaydet
                Undo.RegisterCreatedObjectUndo(instance, "Create Grid Cell");
#endif
            }
        }

#if UNITY_EDITOR
        // Se�imi parent nesneye ge�ir (kullan�c�ya g�r�n�rl�k i�in)
        Selection.activeGameObject = parentTransform.gameObject;
#endif

        Debug.Log($"GirdAreaController: {countX * countZ} h�cre olu�turuldu ({countX}x{countZ}).");

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
                // IMPORTANT: do NOT override occupancy here. Leave `cell.reel` as-is (default null)
                // Occupancy should be managed elsewhere and not inferred from components on the cell.
            }

            gridArray[x][z] = cell; // can be null if no GridAreaCell on the cell
        }
    }

    // Optional validator to reconcile occupancy references with actual scene children
    void ValidateGridOccupancy()
    {
        string parentName = $"GridArea_{gameObject.name}";
        Transform parentTransform = transform.Find(parentName);
        if (parentTransform == null || gridArray == null) return;

        foreach (Transform child in parentTransform)
        {
            if (!child.name.StartsWith("Cell_")) continue;
            var parts = child.name.Split('_');
            if (parts.Length < 3) continue;
            if (!int.TryParse(parts[1], out int x)) continue;
            if (!int.TryParse(parts[2], out int z)) continue;
            if (x < 0 || z < 0 || x >= gridArray.Length || gridArray[x] == null || z >= gridArray[x].Length) continue;

            var cell = gridArray[x][z];
            if (cell == null) continue;

            // find an active ReelController child under this cell
            ReelController found = null;
            for (int i = 0; i < child.childCount; i++)
            {
                var rc = child.GetChild(i).GetComponent<ReelController>();
                if (rc != null && rc.gameObject.activeInHierarchy)
                {
                    found = rc;
                    break;
                }
            }

            // reconcile
            if (cell.reel != found)
            {
                cell.reel = found; // updates collider via setter
            }
        }
    }

    // Try to reach 'to' from 'from' using A* with constraints:
    // - Max 3 turns
    // - Cannot pass through occupied cells (except start and end)
    // Returns shortest path (Manhattan) if exists
    public bool TryToReach(Vector2 from, Vector2 to, out Vector2[] path, out PathResult result, out GameObject blockingObject)
    {
        blockingObject = null;
        path = null;
        result = PathResult.NoOpenPath;

        int width = Mathf.RoundToInt(size.x);
        int height = Mathf.RoundToInt(size.y);

        // validate bounds
        int sx = Mathf.RoundToInt(from.x);
        int sz = Mathf.RoundToInt(from.y);
        int tx = Mathf.RoundToInt(to.x);
        int tz = Mathf.RoundToInt(to.y);

        if (width <= 0 || height <= 0) return false;
        if (sx < 0 || sz < 0 || sx >= width || sz >= height) return false;
        if (tx < 0 || tz < 0 || tx >= width || tz >= height) return false;

        // ensure grid built
        if (gridArray == null || gridArray.Length != width || (width > 0 && (gridArray[0] == null || gridArray[0].Length != height)))
        {
            BuildGridArray();
        }
        // reconcile occupancy to avoid stale blockers
        ValidateGridOccupancy();

        // helper: traversable considering occupancy and active state; allow endpoints to be occupied
        // occupancy considers only active reels to reduce false positives
        // also returns a reason/object for diagnostics
        bool IsTraversable(int x, int z, out string reason, out GameObject reasonObject)
        {
            reason = null;
            reasonObject = null;
            if (x < 0 || z < 0 || x >= width || z >= height)
            {
                reason = "OutOfBounds";
                return false;
            }
            var cell = gridArray != null && gridArray[x] != null ? gridArray[x][z] : null;
            if (cell == null)
            {
                reason = "MissingCell";
                return false;
            }
            if (!cell.gameObject.activeInHierarchy)
            {
                reason = "InactiveCell";
                reasonObject = cell.gameObject;
                return false;
            }
            bool isEndpoint = (x == sx && z == sz) || (x == tx && z == tz);
            if (!isEndpoint && cell.reel != null && cell.reel.gameObject.activeInHierarchy)
            {
                reason = "OccupiedByReel";
                reasonObject = cell.reel != null ? cell.reel.gameObject : cell.gameObject;
                return false;
            }
            return true;
        }

        string startReason, endReason; GameObject startObj, endObj;
        bool startOk = IsTraversable(sx, sz, out startReason, out startObj);
        bool endOk = IsTraversable(tx, tz, out endReason, out endObj);
        if (!startOk || !endOk)
        {
            // endpoints invalid (e.g., outside or missing cells)
            result = PathResult.NoOpenPath;
            blockingObject = !startOk ? (startObj != null ? startObj : this.gameObject) : (!endOk ? (endObj != null ? endObj : this.gameObject) : this.gameObject);
            Debug.Log($"GirdAreaController: NoOpenPath (endpoints invalid). StartOk={startOk}({startReason}), EndOk={endOk}({endReason}), from=({sx},{sz}) to=({tx},{tz})", blockingObject);
            return false;
        }

        // diagnostics counters
        int prunedOutOfBounds = 0;
        int prunedMissingCell = 0;
        int prunedInactive = 0;
        int prunedOccupied = 0;
        int prunedTurnLimit = 0;
        GameObject firstBlockedObject = null;

        // Axis-aligned fast path: if straight line cells are traversable, return it
        if (sx == tx || sz == tz)
        {
            System.Collections.Generic.List<Vector2> axisPath = new System.Collections.Generic.List<Vector2>();
            axisPath.Add(new Vector2(sx, sz));
            bool clear = true;
            if (sx == tx)
            {
                int step = (tz > sz) ? 1 : -1;
                for (int zz = sz + step; zz != tz; zz += step)
                {
                    string rsn; GameObject robj;
                    if (!IsTraversable(sx, zz, out rsn, out robj))
                    {
                        clear = false;
                        break;
                    }
                    axisPath.Add(new Vector2(sx, zz));
                }
            }
            else
            {
                int step = (tx > sx) ? 1 : -1;
                for (int xx = sx + step; xx != tx; xx += step)
                {
                    string rsn; GameObject robj;
                    if (!IsTraversable(xx, sz, out rsn, out robj))
                    {
                        clear = false;
                        break;
                    }
                    axisPath.Add(new Vector2(xx, sz));
                }
            }
            axisPath.Add(new Vector2(tx, tz));
            if (clear)
            {
                path = axisPath.ToArray();
                result = PathResult.Success;
                return true;
            }
        }

        // A* with turn constraint
        Vector2[] FindPathWithTurnLimit(int maxTurns)
        {
            // directions: 0: +X, 1: -X, 2: +Z, 3: -Z
            int[] dx = new int[] { 1, -1, 0, 0 };
            int[] dz = new int[] { 0, 0, 1, -1 };

            // Node structure
            System.Collections.Generic.List<Node> open = new System.Collections.Generic.List<Node>();
            System.Collections.Generic.Dictionary<string, int> bestG = new System.Collections.Generic.Dictionary<string, int>();

            // Seed start with 4 directional states so the first step can go any way
            for (int sd = 0; sd < 4; sd++)
            {
                Node start = new Node
                {
                    x = sx,
                    z = sz,
                    g = 0,
                    h = Mathf.Abs(tx - sx) + Mathf.Abs(tz - sz),
                    dir = sd,
                    turns = 0,
                    parent = null
                };
                open.Add(start);
                bestG[Key(start.x, start.z, start.dir, start.turns)] = 0;
            }

            while (open.Count > 0)
            {
                // pick node with lowest f = g + h; tie-break lower h
                int bestIndex = 0;
                int bestF = open[0].g + open[0].h;
                int bestH = open[0].h;
                for (int i = 1; i < open.Count; i++)
                {
                    int f = open[i].g + open[i].h;
                    if (f < bestF || (f == bestF && open[i].h < bestH))
                    {
                        bestF = f;
                        bestH = open[i].h;
                        bestIndex = i;
                    }
                }

                Node current = open[bestIndex];
                open.RemoveAt(bestIndex);

                // reached target
                if (current.x == tx && current.z == tz)
                {
                    return Reconstruct(current);
                }

                // explore neighbors
                for (int d = 0; d < 4; d++)
                {
                    int nx = current.x + dx[d];
                    int nz = current.z + dz[d];
                    string rsn; GameObject robj;
                    if (!IsTraversable(nx, nz, out rsn, out robj))
                    {
                        if (rsn == "OutOfBounds") prunedOutOfBounds++;
                        else if (rsn == "MissingCell") prunedMissingCell++;
                        else if (rsn == "InactiveCell") prunedInactive++;
                        else if (rsn == "OccupiedByReel") prunedOccupied++;
                        if (firstBlockedObject == null && robj != null) firstBlockedObject = robj;
                        continue;
                    }

                    int newTurns = current.turns;
                    if (current.dir != d) newTurns++;
                    if (newTurns > maxTurns)
                    {
                        prunedTurnLimit++;
                        continue;
                    }

                    int ng = current.g + 1;
                    int nh = Mathf.Abs(tx - nx) + Mathf.Abs(tz - nz);

                    string k = Key(nx, nz, d, newTurns);
                    if (bestG.TryGetValue(k, out int prevG))
                    {
                        if (ng >= prevG) continue;
                    }

                    bestG[k] = ng;
                    Node next = new Node
                    {
                        x = nx,
                        z = nz,
                        g = ng,
                        h = nh,
                        dir = d,
                        turns = newTurns,
                        parent = current
                    };
                    open.Add(next);
                }
            }

            return null;
        }

        // First attempt with turn cap = 3
        var capped = FindPathWithTurnLimit(3);
        if (capped != null)
        {
            path = capped;
            result = PathResult.Success;
            return true;
        }

        // Check if a path exists with a generous finite turn cap (diagnostic)
        // Use a bound tied to grid size to prevent infinite state space.
        int generousTurnCap = Mathf.Max(width * height, 8);
        var unlimited = FindPathWithTurnLimit(generousTurnCap);
        if (unlimited != null)
        {
            // signal too many turns required
            result = PathResult.TooMuchTurn;
            path = unlimited; // optional: can return alternative path for visualization
            return false;
        }

        // no open path at all
        result = PathResult.NoOpenPath;
        path = null;
        // If axis-aligned, emit straight-line cell diagnostics to pinpoint blockers
        if (sx == tx || sz == tz)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("StraightLineCheck: ");
            GameObject straightCtx = null;
            if (sx == tx)
            {
                int step = (tz > sz) ? 1 : -1;
                for (int zz = sz + step; zz != tz; zz += step)
                {
                    string rsn; GameObject robj;
                    bool ok = IsTraversable(sx, zz, out rsn, out robj);
                    sb.Append($"({sx},{zz})->{(ok ? "OK" : rsn)} ");
                    if (!ok && straightCtx == null && robj != null) straightCtx = robj;
                }
            }
            else // sz == tz
            {
                int step = (tx > sx) ? 1 : -1;
                for (int xx = sx + step; xx != tx; xx += step)
                {
                    string rsn; GameObject robj;
                    bool ok = IsTraversable(xx, sz, out rsn, out robj);
                    sb.Append($"({xx},{sz})->{(ok ? "OK" : rsn)} ");
                    if (!ok && straightCtx == null && robj != null) straightCtx = robj;
                }
            }
            Debug.Log(sb.ToString(), straightCtx != null ? straightCtx : this.gameObject);
        }

        Debug.Log($"GirdAreaController: NoOpenPath (no route found). from=({sx},{sz}) to=({tx},{tz}), grid={width}x{height}, turnCap=3, diagnosticCap={generousTurnCap}. Pruned -> OutOfBounds:{prunedOutOfBounds}, MissingCell:{prunedMissingCell}, Inactive:{prunedInactive}, Occupied:{prunedOccupied}, TurnLimit:{prunedTurnLimit}", firstBlockedObject != null ? firstBlockedObject : this.gameObject);
        return false;
    }

    class Node
    {
        public int x;
        public int z;
        public int g;
        public int h;
        public int dir; // -1 none, 0:+X,1:-X,2:+Z,3:-Z
        public int turns;
        public Node parent;
    }

    static string Key(int x, int z, int dir, int turns) => x + "," + z + "," + dir + "," + turns;

    static Vector2[] Reconstruct(Node end)
    {
        System.Collections.Generic.List<Vector2> rev = new System.Collections.Generic.List<Vector2>();
        Node cur = end;
        while (cur != null)
        {
            rev.Add(new Vector2(cur.x, cur.z));
            cur = cur.parent;
        }
        rev.Reverse();
        return rev.ToArray();
    }

    public enum PathResult
    {
        NoOpenPath,
        TooMuchTurn,
        Success
    }
}