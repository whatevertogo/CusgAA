using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
	public class GameInput : Singleton<GameInput>
	{
		private PlayerInputSystem PlayerInput; // 玩家输入
		public Vector2 moveDir = Vector2.zero; // 人物的移动方向

		public event EventHandler OnInteractAction;//互动事件E
		public event EventHandler OnOpenInventoryAction;//打开背包事件B

		public event EventHandler OnClickAction; // 点击事件

		public event EventHandler OnJumpAction; // 新增跳跃事件SPACE
		public bool JumpPressed { get; private set; } // 跟踪跳跃按钮状态

		protected override void Awake()
		{
			base.Awake();
			PlayerInput = new PlayerInputSystem(); // 创建 PlayerInputControl 实例	
			PlayerInput.Player.Interact.performed += Interact_performed;
			PlayerInput.Player.OpenInventory.performed += OpenClosedInventory_performed;
			PlayerInput.Player.Jump.performed += Jump_performed;
			PlayerInput.Player.Click.performed += Clicked_performed;
		}

        private void Clicked_performed(InputAction.CallbackContext context)
        {
            OnClickAction?.Invoke(this, EventArgs.Empty);
        }

        private void Update()
		{
			moveDir = PlayerInput.Player.Move.ReadValue<Vector2>();
			JumpPressed = PlayerInput.Player.Jump.ReadValue<float>() > 0.1f; // 检查跳跃按钮是否被按下
		}

		#region 通过事件委托触发按键事件

		// 添加互动事件#TODO-写互动功能 
		private void Interact_performed(InputAction.CallbackContext obj) =>
			OnInteractAction?.Invoke(this, EventArgs.Empty);//PlayerController里面订阅并执行

		// private void InteractAlternate_performed(InputAction.CallbackContext obj)=>
		// 	OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);//Player里面订阅并执行

		// 打开背包B键位
		private void OpenClosedInventory_performed(InputAction.CallbackContext obj) =>
			OnOpenInventoryAction?.Invoke(this, EventArgs.Empty);//在InventoryManager中执行

		// 添加跳跃Space键位
		private void Jump_performed(InputAction.CallbackContext obj) =>
			OnJumpAction?.Invoke(this, EventArgs.Empty);//在PlayerController中执行

		#endregion

		private void OnEnable()
		{
			PlayerInput.Enable();
		}

		private void OnDisable()
		{
			PlayerInput.Disable();
		}


	}
}
