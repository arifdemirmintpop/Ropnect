using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;
namespace tiplay.MoneyKit
{
    public class MoneyBarText : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private GameObject moneyImage;

        Vector3 moneyImageStartScale;

        private void Awake()
        {
            moneyText ??= GetComponent<TMP_Text>();
            moneyImageStartScale = moneyImage.transform.localScale;
        }

        private void OnEnable()
        {
            Money.OnChanged += UpdateText;
            InitializeText();
        }
        
         private void InitializeText()
        {
            moneyText.text = Money.GetMoneyString();
             
        }

        private void UpdateText()
        {
            int amount = 0;

            string moneyString = moneyText.text;

            bool parsed = int.TryParse(moneyString.RemoveSpecialCharacters(), out amount);

            if (parsed)
            {
                DOTween.To(
            () => amount,
            x => amount = x,
            (int)Money.GetMoney(),
            1
             ).OnUpdate(() =>
            {
                moneyText.text = Money.ConvertToMoneyFormat((float)amount);
            });
            }
            else moneyText.text = Money.GetMoneyString();

        }

        public void MoneyScaleEffect()
        {
            moneyImage.transform.DOScale(moneyImageStartScale * 1.2f, 0.1f)
                .OnComplete(() => moneyImage.transform.DOScale(moneyImageStartScale, 0.1f));
        }

        private void OnDisable()
        {
            Money.OnChanged -= UpdateText;
        }
    }
}
