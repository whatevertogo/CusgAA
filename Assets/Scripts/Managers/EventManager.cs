using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// 事件管理器 - 统一管理游戏中的事件系统
    /// 有三个输入事件是通过GameInput输入的不写入内 
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        #region 触发可互动物品被选择视觉效果事件
        public event EventHandler<TriggerObjectSelectedEventArgs> OnTriggerObjectSelected;
        /// <summary>
        /// 触发Player选中物体事件参数
        /// </summary>
        public class TriggerObjectSelectedEventArgs : EventArgs
        {
            public TriggerObject SelectedObject { get; set; }
        }
        /// <summary>
        /// 触发Player选中物体事件
        /// </summary>
        public void TriggerObjectSelected(TriggerObject selectedObject)
        {
            OnTriggerObjectSelected?.Invoke(this, new TriggerObjectSelectedEventArgs { SelectedObject = selectedObject });
        }
        #endregion
        
        #region 触发背包UI更新事件
        public event EventHandler<OnInventoryUpdatedArgs> OnInventoryUpdated;
        
        /// <summary>
        /// 背包ui更新事件，暂时无参数
        /// </summary>
        public class OnInventoryUpdatedArgs : EventArgs { }

        public void InventoryUpdated()
        {
            OnInventoryUpdated?.Invoke(this, new OnInventoryUpdatedArgs());
        }
        #endregion


    }
}
