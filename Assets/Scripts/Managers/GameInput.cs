using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    /// <summary>
    ///     游戏输入管理器
    /// </summary>
    public class GameInput : Singleton<GameInput>
    {
        public Vector2 moveDir = Vector2.zero;
        private PlayerInputSystem PlayerInput;
        public bool JumpPressed { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            PlayerInput = new PlayerInputSystem();

            PlayerInput.Player.Interact.performed += Interact_performed;
            PlayerInput.Player.OpenInventory.performed += OpenInventory_performed;
            PlayerInput.Player.Jump.performed += Jump_performed;
            PlayerInput.Player.Click.performed += Click_performed;
            PlayerInput.Player.ESC.performed += Escape_performed;

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
            PlayerInput.Player.Interact.performed -= Interact_performed;
            PlayerInput.Player.OpenInventory.performed -= OpenInventory_performed;
            PlayerInput.Player.Jump.performed -= Jump_performed;
            PlayerInput.Player.Click.performed -= Click_performed;
            PlayerInput.Player.ESC.performed -= Escape_performed;
            if (PlayerInput != null) PlayerInput.Disable();
        }

        private void OnDestroy()
        {
            Destroy(gameObject);
        }

        public event EventHandler OnInteractAction;
        public event EventHandler OnOpenInventoryAction;
        public event EventHandler OnClickAction;
        public event EventHandler OnJumpAction;
        public event EventHandler OnEscapeAction;

        private void Interact_performed(InputAction.CallbackContext obj)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }

        private void OpenInventory_performed(InputAction.CallbackContext obj)
        {
            OnOpenInventoryAction?.Invoke(this, EventArgs.Empty);
        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            OnJumpAction?.Invoke(this, EventArgs.Empty);
        }

        private void Click_performed(InputAction.CallbackContext obj)
        {
            OnClickAction?.Invoke(this, EventArgs.Empty);
        }

        private void Escape_performed(InputAction.CallbackContext obj)
        {
            OnEscapeAction?.Invoke(this, EventArgs.Empty);
        }
    }
}