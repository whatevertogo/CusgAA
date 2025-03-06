using System;
using UnityEngine;

[Serializable]
public class IInteractReference
{
    [SerializeField] private MonoBehaviour reference;  // 直接存 MonoBehaviour 组件

    public IInteract Value => reference as IInteract;  // 直接转换成 IInteract
}