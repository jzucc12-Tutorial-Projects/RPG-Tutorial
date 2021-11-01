using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName = null;
        [SerializeField] Transform listRoot = null;
        [SerializeField] RowUI rowPrefab = null;
        [SerializeField] TextMeshProUGUI totalField = null;
        [SerializeField] Button confirmButton = null;
        [SerializeField] Button switchButton = null;
        Color originalTotalColor;
        Shopper shopper = null;
        Shop currentShop = null;
        
        private void Start() 
        {
            originalTotalColor = totalField.color;
            shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();  
            if(shopper == null) return;
            shopper.activeShopChanged += ShopChanged;
            confirmButton.onClick.AddListener(Confirm);
            switchButton.onClick.AddListener(SwitchMode);
            ShopChanged();
        }

        void ShopChanged()
        {
            if(currentShop != null) currentShop.onChange -= RefreshUI;
            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop);

            foreach(FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
                button.SetShop(currentShop);
            
            if(currentShop == null) return;
            shopName.text = currentShop.GetShopName();
            currentShop.onChange += RefreshUI;
            RefreshUI();
        }

        void RefreshUI()
        {
            foreach(Transform child in listRoot)
                Destroy(child.gameObject);

            foreach(ShopItem item in currentShop.GetFilteredItem())
            {
                RowUI row = Instantiate(rowPrefab, listRoot);
                row.SetUp(currentShop, item);
            }

            foreach(FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
                button.RefreshUI();

            totalField.text = $"Total: ${currentShop.TransactionTotal():N2}";
            confirmButton.interactable = currentShop.CanTransact();
            totalField.color = !currentShop.IsBuyingMode() || currentShop.HasSufficientFunds() ? originalTotalColor : Color.red;
            if(currentShop.IsBuyingMode())
            {
                switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Switch to Selling";
                confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            }
            else
            {
                switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Switch to Buying";
                confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sell";
            }

        }

        public void Close()
        {
            shopper.SetActiveShop(null);
        }

        void Confirm()
        {
            currentShop.ConfirmTransaction();
        }

        void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode());
        }
    }
}