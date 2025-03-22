/* 代码功能：对话控制器
 作为实际功能组件的基类，用于控制对话的显示和跳过等等
 Control负责方法和数据，而Controller提供自定义控制对话
 所有控制对话Controller的基类，可以根据对话需要个性化定制，比如声音分支普通对话等等,不一定只有3种对话方式.
 */


using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{

    /// <summary>
    /// 所有控制对话Controller的基类，方便后面对话个性化定制，不一定只有3种对话方式
    /// </summary>

    [Header("Control负责方法和数据")]
    [SerializeField] protected DialogueControl dialogueControl;

    // 初始化对话控制组件
    // 说明：
    // 1. 检查是否已经赋值对话控制组件
    // 2. 如果没有，尝试从当前游戏对象获取组件
    // 3. 如果当前对象没有，尝试在场景中查找
    protected virtual void Awake()
    {
        if (dialogueControl == null)
        {
            // 先尝试从当前对象获取
            if (TryGetComponent<DialogueControl>(out DialogueControl dialogueControl))
            {
                Debug.Log("本对象有对话组件");
            }
            else
            {
                dialogueControl = FindFirstObjectByType<DialogueControl>();
                if (dialogueControl == null)
                {
                    Debug.LogError("无法找到 DialogueControl 组件，对话系统无法正常工作，请确保场景中有此组件");
                }
                else
                {
                    Debug.Log("已在场景中找到 DialogueControl 组件");
                }

            }

        }
    }

    // 开始对话
    // 说明：
    // 1. 检查对话控制组件是否存在
    // 2. 调用对话控制组件的显示方法
    // 用途：供外部调用以触发对话开始
    public virtual void StartDialogue()
    {
        if (dialogueControl != null)
        {
            dialogueControl.ShowDialogue();
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] 无法开始对话:DialogueControl 组件不存在");
        }
    }

    // 跳过对话
    // 说明：
    // 1. 检查对话控制组件是否存在
    // 2. 调用对话控制组件的跳过方法
    // 用途：当玩家想要快速结束当前对话时调用
    public virtual void SkipDialogue()
    {
        if (dialogueControl != null)
        {
            dialogueControl.SkipDialogue();
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] 无法跳过对话:DialogueControl 组件不存在");
        }
    }
}