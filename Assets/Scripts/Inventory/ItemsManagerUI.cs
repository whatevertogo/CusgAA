using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     背包UI管理器
///     负责：
///     1. 显示和隐藏背包界面
///     2. 动态更新背包物品显示
///     3. 处理背包相关的输入事件
/// </summary>
public class ItemsManagerUI : MonoBehaviour
{
    #region 序列化字段

    [Header("UI引用")] [Tooltip("所有物品的容器")] [SerializeField]
    private Transform AllItems;

    [Tooltip("物品容器预制体")] [SerializeField] private GameObject itemContainerPrefab;

    [Tooltip("背包背景")] [SerializeField] private GameObject InventoryBackGround;

    #endregion

    #region Unity生命周期

    /// <summary>
    ///     初始化背包UI系统
    ///     1. 订阅背包更新事件
    ///     2. 订阅背包开关事件
    ///     3. 设置初始状态
    /// </summary>
    private void Start()
    {
        // 订阅背包更新事件
        EventManager.Instance.OnInventoryUpdated += EventManager_OnInventoryUpdated;

        // 订阅背包开关事件
        GameInput.Instance.OnOpenInventoryAction += InventoryManager_OnOpenInventoryAction;

        // 初始化UI状态
        AllItems.gameObject.SetActive(false);
        InventoryBackGround.SetActive(false);
        UpdateVisual();
    }

    /// <summary>
    ///     取消事件订阅
    /// </summary>
    private void OnDestroy()
    {
        if (EventManager.Instance != null) EventManager.Instance.OnInventoryUpdated -= EventManager_OnInventoryUpdated;

        if (GameInput.Instance != null)
            GameInput.Instance.OnOpenInventoryAction -= InventoryManager_OnOpenInventoryAction;
    }

    #endregion

    #region 事件处理

    /// <summary>
    ///     处理背包内容更新事件
    /// </summary>
    private void EventManager_OnInventoryUpdated(object sender, EventManager.OnInventoryUpdatedArgs args)
    {
        UpdateVisual();
    }

    /// <summary>
    ///     处理背包开关事件
    /// </summary>
    private void InventoryManager_OnOpenInventoryAction(object sender, EventArgs e)
    {
        Open_CloseInventory();
    }

    #endregion

    #region UI操作方法

    /// <summary>
    ///     切换背包显示状态
    /// </summary>
    public void Open_CloseInventory()
    {
        if (AllItems.gameObject.activeSelf)
            HideInventory();
        else
            ShowInventory();
    }

    /// <summary>
    ///     显示背包界面
    /// </summary>
    public void ShowInventory()
    {
        AllItems.gameObject.SetActive(true);
        InventoryBackGround.SetActive(true);
        UpdateVisual();
    }

    /// <summary>
    ///     隐藏背包界面
    /// </summary>
    public void HideInventory()
    {
        AllItems.gameObject.SetActive(false);
        InventoryBackGround.SetActive(false);
    }

    /// <summary>
    ///     更新背包界面显示
    ///     1. 清理现有物品显示
    ///     2. 为每个物品创建显示容器
    ///     3. 设置物品图片和名称
    /// </summary>
    public void UpdateVisual()
    {
        // 清空当前的UI元素
        foreach (Transform child in AllItems) Destroy(child.gameObject);

        // 载入所有背包物品
        if (InventoryManager.Instance != null)
            foreach (var item in InventoryManager.Instance.items)
            {
                // 创建物品容器
                var newItemContainer = Instantiate(itemContainerPrefab, AllItems);

                // 设置物品图片
                if (newItemContainer.transform.Find("Image")?.TryGetComponent<Image>(out var itemImage) == true)
                    itemImage.sprite = item.itemImage;

                // 设置物品名称
                if (newItemContainer.transform.Find("Text")?.TryGetComponent<Text>(out var itemNameText) == true)
                    itemNameText.text = item.itemName;
            }

        //TODO-写一个更好的视觉效果
    }

    #endregion
}