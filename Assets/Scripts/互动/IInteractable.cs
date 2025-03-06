using System;
using UnityEngine;

[Serializable]
public class InterfaceReference<T>where T : class{
    [SerializeField] private MonoBehaviour reference;
    public T Value => reference as T;
}