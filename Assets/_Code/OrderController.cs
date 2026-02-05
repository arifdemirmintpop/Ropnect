using DG.Tweening;
using UnityEngine;

public class OrderController : MonoBehaviour
{
    public float offset = 1f;
    public ReelController[] reels;
    public OrderData order;

    // how many reels for this order are completed
    public int completedCount = 0;

    void OnDisable()
    {
        // no-op: removal from OrderManager will be triggered when order completes
    }

    void OnDestroy()
    {
        // no-op: removal handled by OrderManager when order completes
    }

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

        completedCount = 0;
    }

    // called by a ReelController when it reaches this order (i.e. matched)
    public void AddReel(ReelController reel)
    {
        if (reel == null) return;

        completedCount++;

        var curOrder = reels[completedCount].transform;


        // Optionally disable or mark that reel
        var curCompleteCount = completedCount;
        reel.transform.DOJump(curOrder.position, 0.5f, 1, 0.5f).OnComplete(() =>
        {
            reel.gameObject.SetActive(false);
            curOrder.gameObject.SetActive(false);

            // if full, log and notify manager to remove
            int needed = Mathf.Clamp(order != null ? order.count : 0, 0, reels != null ? reels.Length : 0);
            if (curCompleteCount >= needed)
            {
                Debug.Log($"Order completed: color={order.color} level={order.level}");
                // notify manager
                OrderManager.Instance.OnOrderCompleted(this);

                // destroy this order controller
                Destroy(gameObject);
            }

        });


    }
}
