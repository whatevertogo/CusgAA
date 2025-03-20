using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
///     背包类(本来写的管理单例，后来想着试试用类)
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
    
    public class ItemChangedEventArgs : EventArgs
    {
        public ItemSO Item { get; set; }
        public bool IsAdded { get; set; }
    }

    public event EventHandler<InventoryUIUpdatedEventArgs> InventoryUIUpdated;
    
    public class InventoryUIUpdatedEventArgs: EventArgs
    {
        public List<ItemSO> Items { get; set; }
    }
    

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
        // 订阅添加物品事件
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
    /// 第二种示例 在一个需要调用添加物品的物品互动通过事件管理写 Managers.EventManager.Instance.AddItemToBackPack(item);
    /// 第三种就是哪里需要激活委托就在哪里写EventHanler，让多个对象监听，符合观察者模式，在其他类是不能激活这个类的事件的,但是可以通过方法简洁调用(我个人喜欢这样写，只是闲着无聊想学一下EventManager纯观察者模式实现(我有点不太喜欢这种写法,因为知道功能封装分类的好，事件是不会乱的，保证一个事件多个订阅者，而不是事件嵌套事件))
    /// 前两种是你不会委托的情况下我为你封装了一个方法
    /// 第三种是我给你一个正常哪怕不用EventManager的委托方法
    /// 就是创建一个基于委托的事件激活，别的类激活后委托传递参数 (不需要的话可以不传递参数那就是TryToAddItem?.Invoke(this, EventArgs.Empty))
    /// Managers.EventManager.Instance.TryToAddItem += AddItem; // 订阅添加物品委托
    /// Managers.EventManager.Instance.TryToAddItem?.Invoke(this, new Managers.EventManagerAddItemEventArgs { Item = item }激活委托
    /// </summary>
    /// <param name="itemName">要添加的物品名称</param>
    /// <remarks>
    ///     1. 检查背包是否已满
    ///     2. 检查物品是否在数据库中
    ///     3. 检查物品是否已存在于背包中
    ///     4. 添加物品到背包
    ///     5. 触发背包更新事件 
    /// </remarks>
    public void AddItem(string itemName)
    {
        if (this.items.Count >= 6) return;
        if (itemDictionary.TryGetValue(itemName, out var itemSO))
        {
            if (!items.Contains(itemSO))
            {
                items.Add(itemSO);
                InventoryUIUpdated?.Invoke(this,new InventoryUIUpdatedEventArgs{Items = items});// 触发背包更新事件,事件实现解耦
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
            InventoryUIUpdated?.Invoke(this,new InventoryUIUpdatedEventArgs{Items = items});// 触发背包更新事件,事件实现解耦
            Debug.Log($"已移除物品：{itemSO.itemName}");
        }
    }


    #endregion
}