/*
    * 用于处理分支对话的控制器
    * 通过选择不同的选项，进入不同的对话
    */




using UnityEngine;
using System.Collections.Generic;

public class BranchingDialogueController : DialogueController
{
    [SerializeField] private Dictionary<string, DialogueSO> branchOptions;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void StartDialogue()
    {
        base.StartDialogue();//调用基类的方法
        Debug.Log("开始分支对话");
    }

    public void ChooseOption(string option)
    {
        if (branchOptions.ContainsKey(option))
        {
            dialogueControl.SetDialogueSO(branchOptions[option]);
            dialogueControl.ShowDialogue();
        }
        else
        {
            Debug.LogError("无效选项：" + option);
        }
    }
}
