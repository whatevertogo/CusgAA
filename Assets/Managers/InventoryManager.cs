using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private GameObject inventoryUIItems;
        [SerializeField] private ItemsManagerUI itemsManagerUI;
        public List<Items_SO> items= new List<Items_SO>();
        
        private void Start()
        {
            HideInventory();
            GameInput.Instance.OnOpenInventoryAction += InventoryManager_OnOpenInventoryAction;//通过GameInput的事件来打开背包
            
        }

        private void InventoryManager_OnOpenInventoryAction(object sender, EventArgs e)//通过GameInput的事件来打开背包
        {
            if (inventoryUIItems.activeSelf)
                HideInventory();
            else
                ShowInventory();
        }
        
        public void ShowInventory()
        {
            inventoryUIItems.SetActive(true);
            itemsManagerUI.UpdateVisual();
        }
        public void HideInventory()
        {
            inventoryUIItems.SetActive(false);
        }
        
        #region 物品类方法
        public void AddItem(Items_SO itemsSO) 
        {
            if (!items.Contains(itemsSO))
            {
                items.Add(itemsSO);
                itemsManagerUI.UpdateVisual();
                Debug.Log($"Add {itemsSO.itemName}");
            }

        }
        
        public bool HasItem(Items_SO itemsSO)
        {
            return items.Contains(itemsSO);
        }
        
        public void RemoveItem(Items_SO itemsSO)
        {
            if (items.Contains(itemsSO))
            {
                items.Remove(itemsSO);
                itemsManagerUI.UpdateVisual();
                Debug.Log($"Remove {itemsSO.itemName}");
            }
        }
        #endregion



    }
}