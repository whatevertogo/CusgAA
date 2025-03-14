using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    /// <summary>
    ///     游戏输入管理器
    /// </summary>
    public class GameInput : MonoBehaviour
    {
        private static GameInput _instance;
        private GameInput() { }// 私有构造函数，防止外部实例化

        public static GameInput Instance => _instance; // 单例实例

        private Vector2 MoveDir = Vector2.zero;
        public Vector2 moveDir
        {
            get => MoveDir;
            set => MoveDir = value;
        }
        private PlayerInputSystem PlayerInput;
        public bool JumpPressed { get; private set; }

        // 事件定义
        public event EventHandler OnInteractAction;
        public event EventHandler OnOpenInventoryAction;
        public event EventHandler OnClickAction;
        public event EventHandler OnJumpAction;
        public event EventHandler OnEscapeAction;

        protected  void Awake()
        {
            _instance = this;
            PlayerInput = new PlayerInputSystem();
            RegisterInputCallbacks();
            PlayerInput.Enable();
        }

        private void Update()
        {
            moveDir = PlayerInput.Player.Move.ReadValue<Vector2>();
            JumpPressed = PlayerInput.Player.Jump.ReadValue<float>() > 0.1f;
        }

        private void OnEnable()
        {
            if (PlayerInput != null) PlayerInput.Enable();
        }

        private void OnDisable()
        {
            if (PlayerInput != null)
            {
                PlayerInput.Disable();
            }
        }

        private void OnDestroy()
        {
            CleanupInputSystem();
        }

        #region 输入系统管理

        /// <summary>
        /// 注册所有输入回调
        /// </summary>
        private void RegisterInputCallbacks()
        {
            // 使用Lambda表达式简化事件处理
            PlayerInput.Player.Interact.performed += ctx => OnInteractAction?.Invoke(this, EventArgs.Empty);
            PlayerInput.Player.OpenInventory.performed += ctx => OnOpenInventoryAction?.Invoke(this, EventArgs.Empty);
            PlayerInput.Player.Jump.performed += ctx => OnJumpAction?.Invoke(this, EventArgs.Empty);
            PlayerInput.Player.Click.performed += ctx => OnClickAction?.Invoke(this, EventArgs.Empty);
            PlayerInput.Player.ESC.performed += ctx => OnEscapeAction?.Invoke(this, EventArgs.Empty);
        }

        
        /// <summary>
        /// 清理输入系统资源
        /// </summary>
        private void CleanupInputSystem()
        {
            if (PlayerInput != null)
            {
                PlayerInput.Disable();  // 确保先禁用
                PlayerInput.Dispose();  // 完全释放资源
                PlayerInput = null;
            }
        }

        #endregion
    }
}
