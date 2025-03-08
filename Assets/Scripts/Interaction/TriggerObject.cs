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
    [SerializeField] private InventoryManager inventoryManager;


    // 实现IInteract接口的交互方法
    // 说明：定义物体被交互时的基础行为
    // 用途：派生类可以重写此方法实现具体的交互逻辑
    public virtual void Interact()
    {
        Debug.Log("Interact");
    }





}
