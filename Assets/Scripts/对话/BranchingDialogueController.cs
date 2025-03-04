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
        base.StartDialogue();
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
