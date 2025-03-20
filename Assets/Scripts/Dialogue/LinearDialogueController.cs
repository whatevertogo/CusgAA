/*
普通播放对话
*/

using UnityEngine;

public class LinearDialogueController : DialogueController
{
    // 初始化线性对话控制器
    // 说明：
    // 1. 调用基类的Awake方法初始化基础对话组件
    // 2. 可以在这里添加线性对话特有的初始化逻辑
    protected override void Awake()
    {
        base.Awake();
    }


    private void Start()
    {
        //对话开始
        //这只是个示例，你可以选择自由通过事件来通知他开始对话.或者使用已有的方法
        StartDialogue();  

    }

    // 开始线性对话
    // 说明：
    // 1. 调用基类的对话开始方法
    // 2. 输出线性对话开始的日志
    // 用途：当需要开始一段顺序播放的对话时调用
    public override void StartDialogue()
    {
        base.StartDialogue();
        Debug.Log("开始线性对话");
    }

    // 跳过当前线性对话
    // 说明：
    // 1. 调用基类的跳过对话方法
    // 2. 输出跳过对话的日志
    // 用途：当玩家想要跳过当前正在播放的对话时调用
    public override void SkipDialogue()
    {
        base.SkipDialogue();
        Debug.Log("跳过线性对话");
    }
}