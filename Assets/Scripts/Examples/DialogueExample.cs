using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueExample : MonoBehaviour
{
    [SerializeField] private Dialogue_SO exampleDialogue;
    [SerializeField] private Dialogue_SO branchDialogue1;
    [SerializeField] private Dialogue_SO branchDialogue2;
    
    private DialogueManager dialogueManager;
    
    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
        
        // 添加一些示例游戏标记
        dialogueManager.SetFlag("HasMetNPC", true);
    }
    
    // 在Inspector中可以通过按钮调用此方法
    public void StartSimpleDialogue()
    {
        if (dialogueManager != null && exampleDialogue != null)
        {
            dialogueManager.StartDialogue(exampleDialogue);
        }
    }
    
    // 创建一个动态的带分支的对话
    public void CreateDynamicBranchingDialogue()
    {
        // 创建主对话
        Dialogue_SO mainDialogue = Dialogue_SO.CreateSimple("NPC", "你想去哪里?");
        
        // 添加分支选项
        mainDialogue.AddBranch("去城镇", branchDialogue1);
        mainDialogue.AddBranch("去森林", branchDialogue2, "HasExplored");  // 需要HasExplored标记
        mainDialogue.AddBranch("留在这里", null);  // 选择后不继续对话
        
        // 开始对话
        dialogueManager.StartDialogue(mainDialogue);
    }
    
    // 响应对话事件的示例方法
    public void OnDialogueEvent(string eventID)
    {
        Debug.Log("对话事件触发: " + eventID);
        
        // 根据事件ID执行不同操作
        switch (eventID)
        {
            case "give_quest":
                Debug.Log("任务已添加到任务日志");
                break;
                
            case "unlock_area":
                Debug.Log("新区域已解锁");
                dialogueManager.SetFlag("HasExplored", true);
                break;
        }
    }
}