using UnityEngine;

// 互动接口
// 说明：定义可交互对象需要实现的基本行为
// 用途：
// 1. 为所有可交互对象提供统一的接口
// 2. 便于实现多态，使不同对象可以有不同的交互行为

namespace Interfaces
{
    public interface IInteract
    {
        // 执行互动行为
        // 说明：定义对象被交互时的行为
        // 用途：
        // 1. 实现具体的交互逻辑
        // 2. 可以是开门、拾取物品、触发对话等各种行为
        void Interact()
        {
            Debug.Log("不应该在基类中调用此方法");
        }
    }
}