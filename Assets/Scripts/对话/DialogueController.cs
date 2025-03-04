using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] protected DialogueControl dialogueControl;

    protected virtual void Awake()
    {
        if (dialogueControl is null)
        {
            dialogueControl = GetComponent<DialogueControl>();
            Debug.Log("没有找到所属的 DialogueControl,尝试获取组件");
        }
    }

    public virtual void StartDialogue()
    {
        if (dialogueControl != null)
        {
            dialogueControl.ShowDialogue();
        }
    }

    public virtual void SkipDialogue()
    {
        if (dialogueControl != null)
        {
            dialogueControl.SkipDialogue();
        }
    }
}