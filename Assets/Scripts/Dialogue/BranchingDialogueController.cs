/*
    * 用于处理分支对话的控制器
    * 通过选择不同的选项，进入不同的对话
    */




using UnityEngine;
using System.Collections.Generic;

public class BranchingDialogueController : DialogueController
{
    [SerializeField] private Dictionary<string, DialogueSO> branchOptions;

    // 初始化分支对话控制器
    // 说明：
    // 1. 调用基类的Awake方法初始化基础对话组件
    // 2. 可以在这里添加分支对话特有的初始化逻辑
    protected override void Awake()
    {
        base.Awake();
    }

    // 开始分支对话
    // 说明：
    // 1. 调用基类的对话开始方法
    // 2. 输出分支对话开始的日志
    // 用途：当需要开始一段包含多个选项的对话时调用
    public override void StartDialogue()
    {
        base.StartDialogue();//调用基类的方法
        Debug.Log("开始分支对话");
    }

    // 选择对话分支选项
    // 参数：option - 选择的选项标识符
    // 说明：
    // 1. 检查选项是否存在于分支选项字典中
    // 2. 如果存在，切换到对应的对话内容并显示
    // 3. 如果不存在，输出错误日志
    // 用途：玩家选择不同对话选项时调用
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
