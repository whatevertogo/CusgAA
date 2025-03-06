using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private ItemDatabaseSO itemDatabaseSO; // 物品数据库
        [SerializeField] private ItemsManagerUI itemsManagerUI; // UI 管理
        public event EventHandler OnInventoryUpdated; // 背包更新事件
        private readonly Dictionary<string, ItemSO> itemDictionary = new(); // 物品字典
        public List<ItemSO> items = new(); // 背包里的物品


        // 初始化单例实例
        protected override void Awake()
        {
            base.Awake();
        }


        // 游戏开始时初始化：
        // 1. 加载所有物品数据
        // 2. 更新UI显示
        // 3. 输出当前物品列表
        // 4. 测试添加物品功能
        private void Start()
        {
            LoadItems();
            itemsManagerUI.UpdateVisual();
            items.ForEach(item => Debug.Log(item.itemName));
            AddItem("KeyForFirst"); //测试是否能加入存在的东西
            AddItem("KeyForFirst1");
        }

        #region 物品类方法

        // 从物品数据库中加载所有物品到物品字典中
        // 如果数据库为空则输出错误日志
        // 加载完成后触发背包更新事件
        private void LoadItems()
        {
            if (itemDatabaseSO == null)
            {
                Debug.LogError("ItemDatabase is missing!");
                return;
            }

            foreach (var item in itemDatabaseSO.itemsList)
            {
                itemDictionary[item.itemName] = item;
            }

            OnInventoryUpdated?.Invoke(this, EventArgs.Empty);

            Debug.Log($"Loaded {itemDictionary.Count} items.");
        }

        // 通过物品名称添加物品到背包中
        // 参数：itemName - 要添加的物品名称
        // 说明：
        // 1. 检查物品是否存在于数据库中
        // 2. 检查物品是否已在背包中
        // 3. 添加成功后触发背包更新事件
        public void AddItem(string itemName)
        {
            if (itemDictionary.TryGetValue(itemName, out ItemSO itemSO))
            {
                if (!items.Contains(itemSO))
                {
                    items.Add(itemSO);
                    //itemsManagerUI.UpdateVisual();
                    OnInventoryUpdated?.Invoke(this, EventArgs.Empty);
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

        // 检查背包中是否包含指定物品
        // 参数：itemSO - 要检查的物品对象
        // 返回：true - 背包中存在该物品，false - 背包中不存在该物品
        public bool HasItem(ItemSO itemSO)
        {
            return items.Contains(itemSO);
        }


        // 从背包中移除指定物品
        // 参数：itemSO - 要移除的物品对象
        // 说明：移除成功后会触发背包更新事件
        public void RemoveItem(ItemSO itemSO)
        {
            if (items.Contains(itemSO))
            {
                items.Remove(itemSO);
                //itemsManagerUI.UpdateVisual();
                OnInventoryUpdated?.Invoke(this, EventArgs.Empty);
                Debug.Log($"Removed {itemSO.itemName}");
            }
        }

        #endregion
    }
}