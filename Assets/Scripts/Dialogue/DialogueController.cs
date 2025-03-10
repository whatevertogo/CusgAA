/* 代码功能：对话控制器
 作为实际功能组件的基类，用于控制对话的显示和跳过
 */


using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] protected DialogueControl dialogueControl;

    // 初始化对话控制组件
    // 说明：
    // 1. 检查是否已经赋值对话控制组件
    // 2. 如果没有，尝试从当前游戏对象获取组件
    // 3. 输出日志提示获取组件的尝试
    protected virtual void Awake()
    {
        if (dialogueControl is null)
        {
            dialogueControl = GetComponent<DialogueControl>();
            Debug.Log("没有找到所属的 DialogueControl,尝试获取组件");
        }
    }

    // 开始对话
    // 说明：
    // 1. 检查对话控制组件是否存在
    // 2. 调用对话控制组件的显示方法
    // 用途：供外部调用以触发对话开始
    public virtual void StartDialogue()
    {
        if (dialogueControl != null) dialogueControl.ShowDialogue();
    }

    // 跳过对话
    // 说明：
    // 1. 检查对话控制组件是否存在
    // 2. 调用对话控制组件的跳过方法
    // 用途：当玩家想要快速结束当前对话时调用
    public virtual void SkipDialogue()
    {
        if (dialogueControl != null) dialogueControl.SkipDialogue();
    }
}