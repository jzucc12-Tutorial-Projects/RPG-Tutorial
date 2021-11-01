using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] string shopName = "";
        [SerializeField] StockItemConfig[] stockConfig = new StockItemConfig[0];
        [SerializeField] float minBarterPercentage = 0.8f;
        Shopper currentShopper;
        ItemCategory category = ItemCategory.None;

        public event Action onChange;
        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();
        bool isBuyingMode = true;


        #region //Filtering
        public IEnumerable<ShopItem> GetFilteredItem() 
        { 
            IEnumerable<ShopItem> items = GetAllItems();
            bool isAll = category  == ItemCategory.None;
            foreach(ShopItem item in items)
            {
                if(!isAll && item.GetItem().GetItemCategory() != category) continue;
                yield return item;
            }
        }
        public void SelectFilter(ItemCategory category) 
        { 
            this.category = category; 
            onChange?.Invoke();
        }
        public ItemCategory GetFilter() { return category; }

        public IEnumerable<ShopItem> GetAllItems()
        {
            int shopperLevel = GetShopperLevel();
            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();

            foreach(InventoryItem item in availabilities.Keys)
            {
                if(availabilities[item] <= 0) continue;

                float price = prices[item];
                int quantityInTransaction; 
                int currentAvailability = availabilities[item];
                transaction.TryGetValue(item, out quantityInTransaction);
                yield return new ShopItem(item, currentAvailability, price, quantityInTransaction);
            }
        }
        #endregion

        #region //Building available stock
        int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = currentShopper.GetComponent<Inventory>();
            if(inventory == null) return 0;

            int count = 0;
            for(int ii = 0; ii < inventory.GetSize(); ii++)
            {
                if(inventory.GetItemInSlot(ii) != item) continue;
                count += inventory.GetNumberInSlot(ii);
            }

            return count;
        }
        
        int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();
            if(stats == null) return 0;

            return stats.GetLevel();
        }
        
        Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

            foreach(var config in GetAvailableConfigs())
            {
                if(isBuyingMode) 
                {
                    if (!prices.ContainsKey(config.item))
                        prices[config.item] = config.item.GetPrice() * GetBarterDiscount();

                    prices[config.item] *= config.buyingPercentage;
                }
                else prices[config.item] = config.item.GetPrice() * config.sellPercentage;
            }

            return prices;
        }

        private float GetBarterDiscount()
        {
            float percentage = (float)(100 - currentShopper.GetComponent<BaseStats>().GetStat(Stat.BuyingPercentage)) / 100;
            return Mathf.Max(percentage, minBarterPercentage);
        }

        Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities =  new Dictionary<InventoryItem, int>();

            foreach(var config in GetAvailableConfigs())
            {
                if(isBuyingMode)
                {
                    if(!availabilities.ContainsKey(config.item))
                    {
                        int sold;
                        stockSold.TryGetValue(config.item, out sold);
                        availabilities[config.item] = -sold;
                    }

                    availabilities[config.item] += config.initialStock;
                }
                else
                {
                    availabilities[config.item] = CountItemsInInventory(config.item);
                }
            }

            return availabilities;
        }

        IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach(var config in stockConfig)
            {
                if(config.levelToUnlock > shopperLevel) continue;
                yield return config;
            }
        }
        #endregion

        #region //Buy vs Sell
        public void SelectMode(bool isBuying) 
        { 
            isBuyingMode = isBuying;
            onChange?.Invoke();
        }

        public bool IsBuyingMode() { return isBuyingMode; }

        private void SellItem(Inventory shopperInventory, Purse purse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if(slot == -1) return;

            shopperInventory.RemoveFromSlot(slot, 1);
            AddToTransaction(item, -1);
            if(!stockSold.ContainsKey(item))
                stockSold[item] = 0;

            stockSold[item]--;
            purse.UpdateBalance(price);
        }

        private void BuyItem(Inventory shopperInventory, Purse purse, InventoryItem item, float price)
        {
            if (price > purse.GetBalance()) return;
            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                if(!stockSold.ContainsKey(item))
                    stockSold[item] = 0;

                stockSold[item]++;
                purse.UpdateBalance(-price);
            }
        }
        
        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for(int ii = 0; ii < shopperInventory.GetSize(); ii++)
            {
                if(shopperInventory.GetItemInSlot(ii) != item) continue;

                return ii;
            }

            return -1;
        }
        #endregion

        #region //Transaction
        public bool CanTransact() 
        { 
            if(IsTransactionEmpty()) return false;
            if(!isBuyingMode) return true;
            if(!HasSufficientFunds()) return false;
            if(!HasInventorySpace()) return false;
            return true; 
        }

        public void ConfirmTransaction() 
        { 
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse purse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || purse == null) return;

            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetItem();
                int quantity = shopItem.GetQuantity();
                float price = shopItem.GetPrice();

                for(int ii = 0; ii < quantity; ii++)
                {
                    if(isBuyingMode)
                        BuyItem(shopperInventory, purse, item, price);
                    else
                        SellItem(shopperInventory, purse, item, price);
                } 
            }

            onChange?.Invoke();
        }

        public float TransactionTotal() 
        { 
            float total = 0;
            foreach(ShopItem item in GetAllItems())
                total += item.GetPrice() * item.GetQuantity();

            return total;
        }
        
        public void AddToTransaction(InventoryItem item, int quantity) 
        { 
            if(!transaction.ContainsKey(item))
                transaction[item] = 0;

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];
            if(transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
                transaction[item] += quantity;

            if(transaction[item] <= 0)
                transaction.Remove(item);

            onChange?.Invoke();
        }
        #endregion

        #region //IRaycastable
        public bool HandleRaycast(PlayerController callingController)
        {
            if(Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }
        #endregion

        #region //Getters
        public string GetShopName() { return shopName; }
        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }
        #endregion
    
        #region //Transaction checks
        private bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }

        public bool HasSufficientFunds()
        {
            Purse purse = currentShopper.GetComponent<Purse>();
            if(purse == null) return false;
            return purse.GetBalance() >= TransactionTotal();
        }

        bool HasInventorySpace()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return false;

            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetItem();
                int quantity = shopItem.GetQuantity();
                for(int ii = 0; ii < quantity; ii++)
                    flatItems.Add(item);
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }
        #endregion

        public object CaptureState()
        {
            Dictionary<string, int> state = new Dictionary<string, int>();
            foreach(var pair in stockSold)
            {
                state[pair.Key.GetItemID()] = pair.Value;
            }

            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            stockSold.Clear();
            foreach(var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }
    }

    [Serializable]
    public class StockItemConfig
    {
        public InventoryItem item;
        public int initialStock;
        [Range(0,1)] public float buyingPercentage = 1;
        [Range(0,1)] public float sellPercentage = 0.5f;
        public int levelToUnlock = 0;
    }

    public class ShopItem
    {
        InventoryItem item;
        int availability;
        float price;
        int quantityInTransaction;

        public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
        {
            this.item = item;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public int GetAvailability()
        {
            return availability;
        }

        public string GetName()
        {
            return item.GetDisplayName();
        }

        public Sprite GetIcon()
        {
            return item.GetIcon();
        }

        public float GetPrice()
        {
            return price;
        }

        public InventoryItem GetItem()
        {
            return item;
        }

        public int GetQuantity()
        {
            return quantityInTransaction;
        }
    }
}