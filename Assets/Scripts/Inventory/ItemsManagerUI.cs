using System;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

public class ItemsManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemsContainer;


    void Start()
    {
        UpdateVisual();
        GameInput.Instance.OnOpenInventoryAction += InventoryManager_OnOpenInventoryAction;//通过GameInput的事件来打开背包
    }
    
    private void InventoryManager_OnOpenInventoryAction(object sender, EventArgs e)//通过GameInput的事件来打开背包
    {
        if (itemsContainer.gameObject.activeSelf)
            HideInventory();
        else
            ShowInventory();
    }
        
    public void ShowInventory()
    {
        itemsContainer.gameObject.SetActive(true);
        UpdateVisual();
    }
    public void HideInventory()
    {
        itemsContainer.gameObject.SetActive(false);
    }



    public void UpdateVisual()
    {
        //TODO-更新视觉
        //
        // foreach(Transform child in itemsContainer)
        // {
        //     Destroy(child.gameObject);
        // }
        //
        // foreach(Items_SO itemsSO in InventoryManager.Instance.items)
        // {
        //   
        //   
        // }
        
    }

}
