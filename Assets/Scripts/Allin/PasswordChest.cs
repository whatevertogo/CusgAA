using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PasswordChest : TriggerObject
{
    [SerializeField] private GameObject passwordChestUI;
    
    public event EventHandler PasswordChestUI_Open;


    
    public override void Interact()
    {
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
    
    // //Todo-Debug完后记得删除
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);
    // }

}
