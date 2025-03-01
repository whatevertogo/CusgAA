using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
	public class GameInput : Singleton<GameInput>
	{
		private PlayerInputSystem PlayerInput; // 玩家输入
	
		public Vector3 moveDir=Vector2.zero; // 人物的移动方向
	
		public event EventHandler OnInteractAction;
	
		//public event EventHandler OnInteractAlternateAction;
	
		public event EventHandler OnOpenInventoryAction;

		protected override void Awake()
		{
			base.Awake();
			PlayerInput = new PlayerInputSystem(); // 创建 PlayerInputControl 实例	
			PlayerInput.Enable();
			PlayerInput.Player.Interact.performed+=Interact_performed;
			PlayerInput.Player.OpenInventory.performed+=Open_Inventory_performed;
			// PlayerInput.Player.InteractAlternate.performed+=InteractAlternate_performed;
		}

		private void Interact_performed(InputAction.CallbackContext obj)=> 
			OnInteractAction?.Invoke(this,EventArgs.Empty);//Player里面订阅并执行
	
		// private void InteractAlternate_performed(InputAction.CallbackContext obj)=>
		// 	OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);//Player里面订阅并执行
	
		private void Open_Inventory_performed(InputAction.CallbackContext obj)=>
			OnOpenInventoryAction?.Invoke(this, EventArgs.Empty);//在InventoryManager中执行
	
		
	
		private void OnEnable()
		{
			PlayerInput.Enable();
		}
	
		private void OnDisable()
		{
			PlayerInput.Disable();
		}
	
		private void Update()
		{
			moveDir = PlayerInput.Player.Move.ReadValue<Vector2>();
		}
	

	}
}
