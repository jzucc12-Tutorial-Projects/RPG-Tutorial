using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] Image iconField = null;
        [SerializeField] TextMeshProUGUI nameField = null;
        [SerializeField] TextMeshProUGUI availabilityField = null;
        [SerializeField] TextMeshProUGUI priceField = null;
        [SerializeField] TextMeshProUGUI quantityField = null;
        ShopItem myItem;
        Shop currentShop;

        public void SetUp(Shop shop, ShopItem item)
        {
            currentShop = shop;
            myItem = item;
            iconField.sprite = item.GetIcon();
            nameField.text = item.GetName();
            availabilityField.text = $"{item.GetAvailability()}";
            priceField.text = $"${item.GetPrice():N2}";
            quantityField.text = item.GetQuantity().ToString();
        }

        public void Add()
        {
            currentShop.AddToTransaction(myItem.GetItem(), 1);
        }

        public void Remove()
        {
            currentShop.AddToTransaction(myItem.GetItem(), -1);
        }
    }
}
