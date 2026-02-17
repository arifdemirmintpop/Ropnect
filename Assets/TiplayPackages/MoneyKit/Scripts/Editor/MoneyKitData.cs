using UnityEngine;

namespace tiplay.MoneyKit
{
    //[CreateAssetMenu(menuName = "Money Kit")]
    public class MoneyKitData : ScriptableObject
    {
        [SerializeField] private GameObject moneyBar;

        public static GameObject MoneyBar => GetInstance().moneyBar;

        private static MoneyKitData instance;
        private static MoneyKitData GetInstance()
        {
            instance ??= Resources.Load<MoneyKitData>(nameof(MoneyKitData));
            return instance;
        }
    }
}
