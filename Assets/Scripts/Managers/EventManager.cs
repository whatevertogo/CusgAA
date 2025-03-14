using System;
using System.Collections.Generic;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    /// <summary>
    ///     事件管理器 - 统一管理游戏中的事件系统
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        protected override void Awake()
        {
            base.Awake();
            // 初始化事件
            OnTriggerObjectSelected = null;
            OnInventoryUpdated = null;
        }
        #region 可交互物体选择事件

        public event EventHandler<TriggerObjectSelectedEventArgs> OnTriggerObjectSelected;

        /// <summary>
        ///     触发物体选中事件参数类
        /// </summary>
        public class TriggerObjectSelectedEventArgs : EventArgs
        {
            public TriggerObject SelectedObject { get; set; }
        }

        /// <summary>
        /// 触发物体选中事件
        /// </summary>
        /// <param name="selectedObject">被选中的物体</param>
        public void TriggerObjectSelected(TriggerObject selectedObject)
        {
            OnTriggerObjectSelected?.Invoke(this, new TriggerObjectSelectedEventArgs { SelectedObject = selectedObject });
        }

        #endregion

        #region 背包更新事件

        public event EventHandler<OnInventoryUpdatedArgs> OnInventoryUpdated;

        /// <summary>
        ///     背包更新事件参数类
        /// </summary>
        public class OnInventoryUpdatedArgs : EventArgs
        {
        }

        /// <summary>
        ///     触发背包更新事件
        /// </summary>
        public void InventoryUpdated()
        {
            if (OnInventoryUpdated == null) return;

            var args = new OnInventoryUpdatedArgs();
            OnInventoryUpdated(this, args);
        }

        #endregion

        #region 背包添加物品事件

        public class AddItemEventArgs : EventArgs
        {
            public ItemSO Item { get; set; }
        }

        public event EventHandler<AddItemEventArgs> TryToAddItem;

        /// <summary>
        /// ///     尝试添加物品到背包事件
        /// </summary>
        /// <param name="item">要添加的物品</param>
        public void AddItemToBackPack(ItemSO item) {
        TryToAddItem?.Invoke(this, new AddItemEventArgs { Item = item });
        }
        #endregion

        //TODO-打开Panel后停止移动事件
    }
}