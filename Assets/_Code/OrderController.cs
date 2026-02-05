using UnityEngine;

public class OrderController : MonoBehaviour
{
    public float offset = 1f;
    public ReelController[] reels;
    public OrderData order;

    public void SetOrder(OrderData newOrder)
    {
        order = newOrder;
        if (reels == null) return;

        // activate according to count and center them on X axis
        int count = Mathf.Clamp(order.count, 0, reels.Length);
        float totalWidth = (count - 1) * offset;
        float startX = -totalWidth * 0.5f;
        for (int i = 0; i < reels.Length; i++)
        {
            var r = reels[i];
            if (r == null) continue;
            if (i < count)
            {
                r.gameObject.SetActive(true);
                r.transform.localPosition = new Vector3(startX + i * offset, r.transform.localPosition.y, r.transform.localPosition.z);
                r.SetData(order.level, order.color, true);
            }
            else
            {
                r.gameObject.SetActive(false);
            }
        }
    }
}
