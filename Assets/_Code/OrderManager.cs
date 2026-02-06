using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoSingleton<OrderManager>
{
    public OrderData[] orders;
    public GridAreaFiller gridAreaFiller;
    public OrderController orderPrefab;
    public int currentOrder = 0;

    public List<ReelController> reels = new List<ReelController>();

    // track active OrderController instances
    public List<OrderController> orderControllers = new List<OrderController>();

    void Start()
    {
        // populate reels by finding in scene
        reels = new List<ReelController>(FindObjectsByType<ReelController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        CreateOrder();
        gridAreaFiller.FillGridRandomly();
    }

    public void CreateOrder()
    {
        if (orders == null || orders.Length == 0) return;
        currentOrder = Mathf.Clamp(currentOrder, 0, orders.Length - 1);
        var data = orders[currentOrder];
        if (orderPrefab == null) return;
        var oc = Instantiate(orderPrefab,transform);
        oc.SetOrder(data);

        // register the created order controller so others can query orders
        orderControllers.Add(oc);
    }

    // Remove an order controller from registry (called when order completed)
    public void RemoveOrderController(OrderController oc)
    {
        if (oc == null) return;
        if (orderControllers.Contains(oc)) orderControllers.Remove(oc);
    }

    // Called by an OrderController when it is fully completed
    public void OnOrderCompleted(OrderController oc)
    {
        // remove the completed order controller from registry
        RemoveOrderController(oc);

        // advance to next order and create one
        if (orders == null || orders.Length == 0) return;
        currentOrder = (currentOrder + 1) % orders.Length;
        CreateOrder();

        // after creating new order, attempt to complete with existing reels
        foreach (var r in reels)
        {
            TryToCompleteOrder(r);
        }
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
            foreach (var r in reels)
            {
                TryToCompleteOrder(r);
            }
        }
    }
}
