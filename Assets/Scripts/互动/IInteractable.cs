using System;
using UnityEngine;

[Serializable]
public class InterfaceReference<T> where T : class
{
    [SerializeField] private MonoBehaviour reference;  // 直接存 MonoBehaviour，而不是 GameObject

    public T Value => reference as T;
}
