using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tiplay.MoneyKit
{
    public static class Money
    {
        public static void IncreaseMoney(float amount, string source)
        {
            SetMoney(GetMoney() + amount, source);
        }

        public static void DecreaseMoney(float amount, string source)
        {
            SetMoney(GetMoney() - amount, source);
        }

        public static void SetMoney(float newMoney, string source)
        {
            if (GetMoney() != newMoney)
            {
                ElephantManager.EconomyTransaction("Money", GlobalData.Database.LevelDatabase.LevelTextValue, (long)GetMoney(), (long)newMoney, source);
            }

            GlobalData.Database.InventoryDatabase.Money = newMoney;
            OnChanged?.Invoke();
        }

        public static float GetMoney() => GlobalData.Database.InventoryDatabase.Money;

        public static Action OnChanged;

        public static string GetMoneyString()
        {
            if (MoneySettings.Abbreviations)
            {
                return ConvertToMoneyFormat(GetMoney());
            }

            return GetMoney().ToString("F0") + MoneySettings.Symbol;
        }

        public static string ConvertToMoneyFormat(float value)
        {
            int decimals = 0;
            string suffix = string.Empty;

            if (value >= 1000000000)
            {
                decimals = MoneySettings.BillionDecimals;
                suffix = MoneySettings.BillionSuffix;
            }
            else if (value >= 1000000)
            {
                decimals = MoneySettings.MillionDecimals;
                suffix = MoneySettings.MillionSuffix;
            }
            else if (value >= 1000)
            {
                decimals = MoneySettings.ThousandDecimals;
                suffix = MoneySettings.ThousandSuffix;
            }
            else
            {
                decimals = MoneySettings.DefaultDecimals;
            }

            float result = ConvertValue(value, decimals);
            
            return result.ToString($"#0.{new string('#', decimals)}") + suffix + MoneySettings.Symbol;
        }

        private static float ConvertValue(float value, int decimals)
        {
            int digitCount = ((int)value).ToString().Length;
            int powerOfTen = Mathf.FloorToInt((float)(digitCount - 1) / 3) * 3;
            value = Mathf.Floor(value / Mathf.Pow(10, powerOfTen - decimals));
            value /= Mathf.Pow(10, decimals);
            return value;
        }
    }
}
