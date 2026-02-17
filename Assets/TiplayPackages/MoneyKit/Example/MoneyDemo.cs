using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tiplay.MoneyKit
{
    public class MoneyDemo : MonoBehaviour
    {
        public void IncreaseMoney()
        {
            Money.IncreaseMoney(1, "Money Increase");
        }

        public void DecreaseMoney()
        {
            if (Money.GetMoney() >= 1)
                Money.DecreaseMoney(1, "Money Decrease");
        }
    }
}