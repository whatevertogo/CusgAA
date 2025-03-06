using System;
using UnityEngine;

// 可交互对象引用类
// 说明：用于在Unity Inspector中序列化对IInteract接口的引用
// 用途：
// 1. 使可交互对象可以在Inspector中进行配置
// 2. 提供对IInteract接口实现的类型安全访问
[Serializable]
public class IInteractReference
{
    // MonoBehaviour组件引用
    // 说明：存储实现了IInteract接口的MonoBehaviour组件
    // 用途：在Inspector中可以拖拽任何实现了IInteract的组件
    [SerializeField] private MonoBehaviour reference;

    // 获取IInteract接口实例
    // 说明：将存储的MonoBehaviour组件转换为IInteract接口
    // 返回：实现了IInteract接口的对象实例
    public IInteract Value => reference as IInteract;
}
