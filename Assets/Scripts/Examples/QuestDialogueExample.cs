using UnityEngine;

public class QuestDialogueExample : MonoBehaviour
{
    // 可以在编辑器中设置的预定义对话
    [SerializeField] private Dialogue_SO acceptQuestConsequence;
    [SerializeField] private Dialogue_SO rejectQuestConsequence;
    
    public void StartQuestDialogue()
    {
        // 创建任务对话
        Dialogue_SO questDialogue = Dialogue_SO.CreateSimple("任务发布者", "冒险者，我有一个危险的任务。");
        questDialogue.AddLine("森林里的怪物最近一直在袭击村民。");
        questDialogue.AddLine("你能帮我们消灭它们吗？");
        
        // 动态创建接受后的对话
        if (acceptQuestConsequence == null)
        {
            acceptQuestConsequence = Dialogue_SO.CreateSimple("任务发布者", "太感谢了！");
            acceptQuestConsequence.AddLine("这是一些金币，帮助你准备装备。");
            acceptQuestConsequence.eventID = "quest_accepted";
        }
        
        // 动态创建拒绝后的对话
        if (rejectQuestConsequence == null)
        {
            rejectQuestConsequence = Dialogue_SO.CreateSimple("任务发布者", "我理解，这确实很危险。");
            rejectQuestConsequence.AddLine("如果你改变主意，请再来找我。");
            rejectQuestConsequence.eventID = "quest_rejected";
        }
        
        // 添加分支选项
        questDialogue.AddBranch("接受任务", acceptQuestConsequence);
        questDialogue.AddBranch("拒绝任务", rejectQuestConsequence);
        questDialogue.AddBranch("询问更多信息", null); // 这会结束对话，您可以换成另一个对话
        
        // 开始对话
        DialogueManager.Instance.StartDialogue(questDialogue);
    }
}