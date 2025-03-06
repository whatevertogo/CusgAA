using System;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemsManagerUI : MonoBehaviour
{
    [SerializeField] private Transform AllItems;
    [SerializeField] private GameObject itemsContainerFirst;
    [SerializeField] private GameObject itemContainerPrefab;


    private void Awake()
    {
        InventoryManager.Instance.OnInventoryUpdated += (sender, args) => UpdateVisual(); // 监听背包更新事件,事件激活在InventoryManager中
    }

    void Start()
    {
        UpdateVisual();
        GameInput.Instance.OnOpenInventoryAction += InventoryManager_OnOpenInventoryAction; //通过GameInput的事件来打开背包
        itemsContainerFirst.gameObject.SetActive(false);
    }

    private void InventoryManager_OnOpenInventoryAction(object sender, EventArgs e) //通过GameInput的事件来打开背包
    {
        Open_CloseInventory();
    }

// 打开或关闭背包
    public void Open_CloseInventory()
    {
        if (AllItems.gameObject.activeSelf)
        {
            HideInventory();
        }
        else
        {
            ShowInventory();
        }
    }

    public void ShowInventory()
    {
        AllItems.gameObject.SetActive(true);
        UpdateVisual();
    }

    public void HideInventory()
    {
        AllItems.gameObject.SetActive(false);
    }


    


    public void UpdateVisual()
    {
        // 清空当前的 UI 元素（销毁之前的物品容器）
        foreach (Transform child in AllItems)
        {
            Destroy(child.gameObject);
        }

        // 遍历背包中的所有物品，实例化并显示它们
        foreach (var item in InventoryManager.Instance.items)
        {
            // 实例化一个新的 ItemContainer
            GameObject newItemContainer = Instantiate(itemContainerPrefab, AllItems);

            // 获取容器中的 Image 组件
            Image itemImage = newItemContainer.transform.Find("Image").GetComponent<Image>();
            Text itemNameText = newItemContainer.transform.Find("Text").GetComponent<Text>(); // 获取物品名称的 Text 组件

            // 设置 Image 组件的图片为物品的图片
            if (itemImage != null)
            {
                itemImage.sprite = item.itemImage; // 设置物品的图片
            }

            // 设置物品名称文本
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName; // 显示物品名称
            }

            #region 可选按钮

            // // 可选：为按钮添加事件（例如点击使用物品）
            // Button itemButton = newItemContainer.transform.Find("Button")?.GetComponent<Button>();
            // if (itemButton != null)
            // {
            //     itemButton.onClick.AddListener(() => UseItem(item)); // 为按钮绑定点击事件
            // }

            #endregion
        }
    }
}