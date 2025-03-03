using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private ItemDatabase itemDatabase; // 物品数据库
        [SerializeField] private ItemsManagerUI itemsManagerUI; // UI 管理
        private Dictionary<string, ItemSO> itemDictionary = new(); // 物品字典
        public List<ItemSO> items = new(); // 背包里的物品
        
        
        private void Start()
        {
            LoadItems();
            itemsManagerUI.UpdateVisual();
            items.ForEach(item => Debug.Log(item.itemName));
            AddItem("钥匙");
            AddItem("KeyForFirst");
        }

        #region 物品类方法
        
        // 加载物品
        private void LoadItems()
        {
            if (itemDatabase == null)
            {
                Debug.LogError("ItemDatabase is missing!");
                return;
            }
    
            foreach (var item in itemDatabase.itemsList)
            {
                itemDictionary[item.itemName] = item;
            }
    
            Debug.Log($"Loaded {itemDictionary.Count} items.");
        }
        
        // 通过物品名称添加物品
        public void AddItem(string itemName) 
        {
            if (itemDictionary.TryGetValue(itemName, out ItemSO itemSO))
            {
                if (!items.Contains(itemSO))
                {
                    items.Add(itemSO);
                    itemsManagerUI.UpdateVisual();
                    Debug.Log($"Added {itemSO.itemName}");
                }
                else
                {
                    Debug.Log($"Item {itemName} already exists in inventory.");
                }
            }
            else
            {
                Debug.LogWarning($"Item {itemName} not found in database!");
            }
        }
        
        // 检查是否有该物品
        public bool HasItem(ItemSO itemSO)
        {
            return items.Contains(itemSO);
        }
        
        // 移除物品
        public void RemoveItem(ItemSO itemSO)
        {
            if (items.Contains(itemSO))
            {
                items.Remove(itemSO);
                itemsManagerUI.UpdateVisual();
                Debug.Log($"Removed {itemSO.itemName}");
            }
        }
        
        #endregion
    }
}