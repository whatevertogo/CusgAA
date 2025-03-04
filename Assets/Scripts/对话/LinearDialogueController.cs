using UnityEngine;

public class LinearDialogueController : DialogueController
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void StartDialogue()
    {
        base.StartDialogue();
        Debug.Log("开始线性对话");
    }

    public override void SkipDialogue()
    {
        base.SkipDialogue();
        Debug.Log("跳过线性对话");
    }
}