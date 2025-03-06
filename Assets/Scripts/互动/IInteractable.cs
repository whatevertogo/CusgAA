using System;
using UnityEngine;

[Serializable]
public class InterfaceReference<T> where T : class
{
    [SerializeField] private GameObject reference;  // 直接存 GameObject

    public T Value
    {
        get
        {
            if (reference == null) return null;
            T component = reference.GetComponent<T>();  // 获取第一个 T 类型的组件
            if (component == null)
                Debug.LogWarning($"[InterfaceReference] {reference.name} 没有实现 {typeof(T).Name}，请检查赋值！");
            return component;
        }
    }
}
