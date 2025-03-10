using System;
using System.Collections.Generic;
using Interfaces;

namespace Managers
{
    /// <summary>
    ///     事件管理器 - 统一管理游戏中的事件系统
    ///     特点：
    ///     1. 使用对象池优化事件参数对象的创建和回收
    ///     2. 支持自动重置和回收事件参数
    ///     3. 统一管理所有游戏事件
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        #region 对象池系统

        // 事件参数对象池字典
        // 键：事件参数类型
        // 值：该类型的对象池栈
        private readonly Dictionary<Type, Stack<EventArgs>> _eventArgsPool = new();

        /// <summary>
        ///     从对象池获取事件参数对象
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <returns>一个事件参数实例，可能是复用的或新创建的</returns>
        private T GetEventArgs<T>() where T : EventArgs, new()
        {
            var type = typeof(T);

            // 获取或创建对象池
            if (!_eventArgsPool.TryGetValue(type, out var pool))
            {
                pool = new Stack<EventArgs>();
                _eventArgsPool[type] = pool;
            }

            // 如果池中有对象则复用，否则创建新对象
            return pool.Count > 0 ? (T)pool.Pop() : new T();
        }

        /// <summary>
        ///     将事件参数对象回收到对象池
        /// </summary>
        /// <param name="args">要回收的事件参数对象</param>
        /// <remarks>
        ///     如果对象实现了IResettable接口，会在回收前调用Reset方法
        /// </remarks>
        private void ReleaseEventArgs<T>(T args) where T : EventArgs
        {
            if (args == null) return;

            var type = args.GetType();

            // 获取或创建对象池
            if (!_eventArgsPool.TryGetValue(type, out var pool))
            {
                pool = new Stack<EventArgs>();
                _eventArgsPool[type] = pool;
            }

            // 如果对象支持重置，则在回收前重置
            if (args is IResettable resettable) resettable.Reset();

            pool?.Push(args);
        }

        protected override void Awake()
        {
            base.Awake();
            _eventArgsPool.Clear();
        }

        #endregion

        #region 可交互物体选择事件

        public event EventHandler<TriggerObjectSelectedEventArgs> OnTriggerObjectSelected;

        /// <summary>
        ///     触发物体选中事件参数类
        ///     实现IResettable接口以支持对象池重置
        /// </summary>
        public class TriggerObjectSelectedEventArgs : EventArgs, IResettable
        {
            public TriggerObject SelectedObject { get; set; }

            public void Reset()
            {
                SelectedObject = null;
            }
        }

        /// <summary>
        ///     触发物体选中事件
        /// </summary>
        /// <param name="selectedObject">被选中的物体</param>
        /// <remarks>
        ///     使用对象池管理事件参数对象，自动处理创建和回收
        /// </remarks>
        public void TriggerObjectSelected(TriggerObject selectedObject)
        {
            if (OnTriggerObjectSelected == null) return;

            // 从对象池获取事件参数
            var args = GetEventArgs<TriggerObjectSelectedEventArgs>();
            args.SelectedObject = selectedObject;

            try
            {
                OnTriggerObjectSelected(this, args);
            }
            finally
            {
                // 确保事件处理完后回收参数对象
                ReleaseEventArgs(args);
            }
        }

        #endregion

        #region 背包更新事件

        public event EventHandler<OnInventoryUpdatedArgs> OnInventoryUpdated;

        /// <summary>
        ///     背包更新事件参数类
        ///     实现IResettable接口以支持对象池重置
        /// </summary>
        public class OnInventoryUpdatedArgs : EventArgs, IResettable
        {
            public void Reset()
            {
                // 当前无需重置任何状态
            }
        }

        /// <summary>
        ///     触发背包更新事件
        /// </summary>
        /// <remarks>
        ///     使用对象池管理事件参数对象，避免重复创建
        /// </remarks>
        public void InventoryUpdated()
        {
            if (OnInventoryUpdated == null) return;

            var args = GetEventArgs<OnInventoryUpdatedArgs>();
            try
            {
                OnInventoryUpdated(this, args);
            }
            finally
            {
                ReleaseEventArgs(args);
            }
        }

        #endregion
    }
}