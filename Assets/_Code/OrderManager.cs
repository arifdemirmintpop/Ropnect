using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoSingleton<OrderManager>
{
    public OrderData[] orders;
    public OrderController orderPrefab;
    public int currentOrder = 0;

    public List<ReelController> reels = new List<ReelController>();

    void Start()
    {
        // populate reels by finding in scene
        reels = new List<ReelController>(FindObjectsByType<ReelController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        CreateOrder();
    }

    public void CreateOrder()
    {
        if (orders == null || orders.Length == 0) return;
        currentOrder = Mathf.Clamp(currentOrder, 0, orders.Length - 1);
        var data = orders[currentOrder];
        if (orderPrefab == null) return;
        var oc = Instantiate(orderPrefab,transform);
        oc.SetOrder(data);
    }

    public void TryToCompleteOrder(ReelController reel)
    {
        // naive implementation: check if any order matches reel properties
        // For demonstration we remove reel if its level and color match current order
        if (orders == null || orders.Length == 0) return;
        var data = orders[currentOrder];
        if (reel.level == data.level && reel.color == data.color)
        {
            // consider this reel completed
            reels.Remove(reel);
            currentOrder = (currentOrder + 1) % orders.Length;
            CreateOrder();

            // after creating new order, attempt to complete with existing reels
            var copy = new List<ReelController>(reels);
            foreach (var r in copy)
            {
                TryToCompleteOrder(r);
            }
        }
    }
}
