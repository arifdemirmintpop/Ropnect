using UnityEngine;

public class OrderReelController : MonoBehaviour
{
    public GameObject[] levelGraphics;
    public bool complete = false;
    public Material incompleteMaterial;

    [ContextMenu("Init")]
    public void Init(int level, ReelColorId color)
    {
        // activate level-1 graphic and set incomplete material if available
        if (levelGraphics == null || levelGraphics.Length == 0) return;
        int idx = Mathf.Clamp(level - 1, 0, levelGraphics.Length - 1);
        for (int i = 0; i < levelGraphics.Length; i++)
        {
            var go = levelGraphics[i];
            if (go == null) continue;
            go.SetActive(i == idx);
            if (i == idx && incompleteMaterial != null)
            {
                var mr = go.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.sharedMaterial = incompleteMaterial;
                }
            }
        }
    }

    public void Complete()
    {
        complete = true;
        Destroy(gameObject);
    }
}
