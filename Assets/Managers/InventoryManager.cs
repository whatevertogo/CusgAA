using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private GameObject inventoryUI;
        
        private void Start()
        {
            HideInventory();
            GameInput.Instance.OnOpenInventoryAction += InventoryManager_OnOpenInventoryAction;
            
        }

        private void InventoryManager_OnOpenInventoryAction(object sender, EventArgs e)//通过GameInput的事件来打开背包
        {
            if(inventoryUI.activeSelf)
                HideInventory();
            else
                ShowInventory();
        }
        
        public void ShowInventory()
        {
            inventoryUI.SetActive(true);
        }
        public void HideInventory()
        {
            inventoryUI.SetActive(false);
        }




    }
}