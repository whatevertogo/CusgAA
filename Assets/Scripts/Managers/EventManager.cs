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
            // 初始化所有事件
            OnTriggerObjectSelected = null;
            OnInventoryUpdated = null;
            TryToAddItem = null;
            OnPanelStateChanged = null;
            OnGameStateChanged = null;
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
            OnInventoryUpdated?.Invoke(this, new OnInventoryUpdatedArgs());
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

        #region 面板状态事件

        public event EventHandler<PanelStateEventArgs> OnPanelStateChanged;

        /// <summary>
        ///     面板状态事件参数类
        /// </summary>
        public class PanelStateEventArgs : EventArgs
        {
            public string PanelName { get; set; }
            public bool IsOpen { get; set; }
        }

        /// <summary>
        ///     触发面板状态改变事件
        /// </summary>
        /// <param name="panelName">面板名称</param>
        /// <param name="isOpen">是否打开</param>
        public void PanelStateChanged(string panelName, bool isOpen)
        {
            OnPanelStateChanged?.Invoke(this, new PanelStateEventArgs { PanelName = panelName, IsOpen = isOpen });
        }

        #endregion
        
        #region 游戏状态事件

        public event EventHandler<GameStateEventArgs> OnGameStateChanged;

        /// <summary>
        ///     游戏状态事件参数类
        /// </summary>
        public class GameStateEventArgs : EventArgs
        {
            public GameState NewState { get; set; }
            public GameState PreviousState { get; set; }
        }

        /// <summary>
        ///     游戏状态枚举
        /// </summary>
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            Dialogue,
            Inventory,
            GameOver
        }

        /// <summary>
        ///     触发游戏状态改变事件
        /// </summary>
        /// <param name="newState">新状态</param>
        /// <param name="previousState">前一个状态</param>
        public void GameStateChanged(GameState newState, GameState previousState)
        {
            OnGameStateChanged?.Invoke(this, new GameStateEventArgs 
            { 
                NewState = newState, 
                PreviousState = previousState 
            });
        }

        #endregion

        //TODO-打开Panel后停止移动事件
    }
}