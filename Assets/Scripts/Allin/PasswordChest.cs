using System;
using UnityEngine;
using  UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PasswordChest : TriggerObject
{
    [SerializeField] private GameObject passwordChestUI; 
    [SerializeField] private Button _closeButton;


    private void Start()
    {
        _closeButton.onClick.AddListener(ChestSet);
    }

    public override void Interact(){
        //TODO-互动逻辑
        ChestSet();//设置箱子解锁面板显示
        //TODO-播放密码箱动画
        //TODO-播放密码箱音效
        Debug.Log("打开密码箱");

    }

    private void ChestSet()
    {
        if (!passwordChestUI.activeSelf)
        {
            passwordChestUI.SetActive(true);
        }
        else
        {
            passwordChestUI.SetActive(false);
        }
    }
    

    





}
