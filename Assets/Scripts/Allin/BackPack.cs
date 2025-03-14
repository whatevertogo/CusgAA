using UnityEngine;
using System.Collections.Generic;
using Managers;
/// <summary>
///     背包管理器
///     负责：
///     1. 管理物品的添加和移除
///     2. 物品数据的加载和存储
///     3. 触发背包更新事件
/// </summary>
public class BackPack : MonoBehaviour
{
    #region 序列化字段
    [Header("引用")]
    [Tooltip("物品数据库配置")]
    [SerializeField]
    private ItemDatabaseSO itemDatabaseSO; // 物品数据库
    [Tooltip("背包UI管理器")]
    [SerializeField] private ItemsManagerUI itemsManagerUI; // UI 管理

    #endregion

    // 当前背包中的物品列表
    public List<ItemSO> items = new();
    // 物品字典：用于快速查找物品数据
    private readonly Dictionary<string, ItemSO> itemDictionary = new();

    #region Unity生命周期

    /// <summary>
    ///     游戏开始时初始化
    ///     1. 加载所有物品数据
    ///     2. 测试添加物品功能
    /// </summary>
    private void Start()
    {
        LoadItems();
        items.ForEach(item => Debug.Log(item.itemName));
        AddItem("KeyForFirst"); // 测试物品添加
        AddItem("KeyForFirst1");
        EventManager.Instance.TryToAddItem += AddItem; // 订阅添加物品事件
    }

    #endregion

    #region 物品系统方法

    /// <summary>
    ///     从物品数据库加载所有物品到字典中
    ///     用途：初始化时填充物品字典，方便后续查找
    /// </summary>
    private void LoadItems()
    {
        if (itemDatabaseSO == null)
        {
            Debug.LogError("物品数据库丢失！");
            return;
        }

        foreach (var item in itemDatabaseSO.itemsList)
        {
            itemDictionary[item.itemName] = item;
        }
        Debug.Log($"已加载 {itemDictionary.Count} 个物品数据");
    }

    /// <summary>
    ///     向背包添加物品
    /// 两种添加的方法，第一种用函数调用第二种用事件调用，
    /// /// 第一种示例 在一个需要调用添加物品的物品互动里面写AddItem(name)
    /// 推荐第二种示例 在一个需要调用添加物品的物品互动里面写 Managers.EventManager.Instance.AddItemToBackPack(item);
    /// </summary>
    /// <param name="itemName">要添加的物品名称</param>
    /// <remarks>
    ///     1. 检查物品是否存在于数据库中
    ///     2. 检查物品是否已在背包中
    ///     3. 添加成功后触发背包更新事件
    /// </remarks>
    public void AddItem(string itemName)
    {
        if (itemDictionary.TryGetValue(itemName, out var itemSO))
        {
            if (!items.Contains(itemSO))
            {
                items.Add(itemSO);
                EventManager.Instance?.InventoryUpdated();// 触发背包更新事件,事件实现解耦
                Debug.Log($"已添加物品：{itemSO.itemName}");
            }
            else
            {
                Debug.Log($"物品 {itemName} 已存在于背包中");
            }
        }
        else
        {
            Debug.LogWarning($"物品 {itemName} 在数据库中未找到！");
        }
    }

    public void AddItem(object sender, EventManager.AddItemEventArgs e)
    {
        if (e.Item == null) return;
        if (itemDictionary.TryGetValue(e.Item.itemName, out var itemSO))
        {
            if (!items.Contains(itemSO))
            {
                items.Add(itemSO);
                EventManager.Instance?.InventoryUpdated();// 触发背包更新事件,事件实现解耦
                Debug.Log($"已添加物品：{itemSO.itemName}");
            }
            else
            {
                Debug.Log($"物品 {itemSO.itemName} 已存在于背包中");
            }
        }
        else
        {
            Debug.LogWarning($"物品 {e.Item.itemName} 在数据库中未找到！");
        }

    }

    /// <summary>
    ///     检查背包中是否包含指定物品
    /// </summary>
    /// <param name="itemSO">要检查的物品对象</param>
    /// <returns>true - 背包中存在该物品，false - 不存在</returns>
    public bool HasItem(ItemSO itemSO)
    {
        return items.Contains(itemSO);
    }

    /// <summary>
    ///     从背包中移除指定物品
    /// </summary>
    /// <param name="itemSO">要移除的物品对象</param>
    /// <remarks>
    ///     1. 检查物品是否在背包中
    ///     2. 移除物品
    ///     3. 触发背包更新事件
    /// </remarks>
    public void RemoveItem(ItemSO itemSO)
    {
        if (items.Contains(itemSO))
        {
            items.Remove(itemSO);
            EventManager.Instance?.InventoryUpdated();// 触发背包更新事件,事件实现解耦
            Debug.Log($"已移除物品：{itemSO.itemName}");
        }
    }


    #endregion
}