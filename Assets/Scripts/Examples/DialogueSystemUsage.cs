using UnityEngine;
using UnityEngine.UI;

public class DialogueSystemUsage : MonoBehaviour
{
    [Header("示例UI")]
    [SerializeField] private Button startDialogueButton;
    [SerializeField] private Button branchingDialogueButton;
    
    private DialogueManager dialogueManager;
    
    private void Start()
    {
        // 获取对话管理器
        dialogueManager = DialogueManager.Instance;
        if (dialogueManager == null)
        {
            Debug.LogError("场景中没有找到DialogueManager！请确保DialogueManager已创建。");
            return;
        }
        
        // 设置按钮事件
        if (startDialogueButton != null)
            startDialogueButton.onClick.AddListener(StartExampleDialogue);
            
        if (branchingDialogueButton != null)
            branchingDialogueButton.onClick.AddListener(StartBranchingDialogue);
    }
    
    // 开始简单对话示例
    public void StartExampleDialogue()
    {
        // 创建简单对话
        Dialogue_SO simpleDialogue = CreateSimpleDialogue();
        
        // 开始对话
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(simpleDialogue);
        }
    }
    
    // 开始分支对话示例
    public void StartBranchingDialogue()
    {
        // 创建主对话和分支对话
        Dialogue_SO mainDialogue = Dialogue_SO.CreateSimple("村长", "年轻人，你愿意帮我们解决村子的问题吗？");
        Dialogue_SO acceptDialogue = Dialogue_SO.CreateSimple("村长", "太好了！请击退森林里的怪物。");
        Dialogue_SO rejectDialogue = Dialogue_SO.CreateSimple("村长", "真遗憾，希望你改变主意。");
        Dialogue_SO thinkDialogue = Dialogue_SO.CreateSimple("村长", "不要犹豫，村民们需要你的帮助！");
        
        // 添加更多对话行
        acceptDialogue.AddLine("我会给你丰厚的报酬。");
        rejectDialogue.AddLine("下次再来找我吧。");
        
        // 设置分支
        mainDialogue.AddBranch("接受任务", acceptDialogue);
        mainDialogue.AddBranch("拒绝任务", rejectDialogue);
        mainDialogue.AddBranch("考虑一下", thinkDialogue, "HasMetNPC"); // 仅当HasMetNPC标记为true时才显示
        
        // 设置事件触发
        acceptDialogue.eventID = "accept_quest";
        
        // 开始对话
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(mainDialogue);
        }
    }
    
    // 创建简单对话
    private Dialogue_SO CreateSimpleDialogue()
    {
        Dialogue_SO dialogue = Dialogue_SO.CreateSimple("NPC", "你好，旅行者！");
        dialogue.AddLine("欢迎来到我们的村庄。");
        dialogue.AddLine("有什么我能帮助你的吗？");
        return dialogue;
    }
    
    // 响应对话事件
    public void OnDialogueEvent(string eventID)
    {
        switch (eventID)
        {
            case "accept_quest":
                Debug.Log("玩家接受了任务！");
                // 这里可以添加接受任务的逻辑
                dialogueManager.SetFlag("QuestAccepted", true);
                break;
        }
    }
}