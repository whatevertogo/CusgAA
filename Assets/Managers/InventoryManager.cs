using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private GameObject inventoryUIItems;
        [SerializeField] private ItemsManagerUI itemsManagerUI;
        
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




    }
}