using System;
using Managers;
using UnityEngine;
/*
作为触发物体的基类，实现了IInteract接口
*/

public abstract class TriggerObject : MonoBehaviour, IInteract
{
    public PlayerController playerController;
    public InventoryManager inventoryManager;
    private void Start()
    {
        playerController.OnTriggerObjectChoosed += IamChoosed;
    }
    private void IamChoosed(object sender, PlayerController.OnTriggerObjectChoosedEventArgs e)
    {
        //TODO-视觉效果
    }

    public void Interact()
    {
        Debug.Log("Interact");
    }





}
