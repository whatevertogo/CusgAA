using System;
using UnityEngine;
using System.Linq;  // 用于数组处理

[Serializable]
public class InterfaceReference<T> where T : class
{
    [SerializeField] private GameObject reference;

    public T[] Values  // 返回所有 T 类型的组件
    {
        get
        {
            if (reference == null) return Array.Empty<T>();  // 返回空数组，避免 null 处理
            T[] components = reference.GetComponents<T>();  // 获取所有 T 类型的组件
            if (components.Length == 0)
                Debug.LogWarning($"[InterfaceReference] {reference.name} 没有实现 {typeof(T).Name}，请检查赋值！");
            return components;
        }
    }

    public T FirstValue => Values.FirstOrDefault();  // 获取第一个组件（如果需要单个）
}
