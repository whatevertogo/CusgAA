using System;
using Managers;
using UnityEngine;
/*
作为触发物体的基类，实现了IInteract接口
*/

// 可触发对象的基类
// 说明：为所有可触发交互的对象提供基础功能
// 特点：
// 1. 实现IInteract接口
// 2. 提供与玩家和物品系统的基础交互
// 3. 可以被继承以实现具体的触发行为
public abstract class TriggerObject : MonoBehaviour, IInteract
{
    public PlayerController playerController;
    public InventoryManager inventoryManager;
    // 初始化触发对象
    // 说明：订阅玩家的触发物体选择事件
    // 用途：当玩家选择该触发物体时接收通知
    private void Start()
    {
        playerController.OnTriggerObjectChoosed += IamChoosed;
    }
    // 触发物体被选中时的回调
    // 参数：
    // - sender: 事件发送者
    // - e: 事件参数
    // 说明：当物体被玩家选中时触发，可以添加视觉反馈效果
    private void IamChoosed(object sender, PlayerController.OnTriggerObjectChoosedEventArgs e)
    {
        //TODO-视觉效果
    }

    // 实现IInteract接口的交互方法
    // 说明：定义物体被交互时的基础行为
    // 用途：派生类可以重写此方法实现具体的交互逻辑
    public void Interact()
    {
        Debug.Log("Interact");
    }





}
