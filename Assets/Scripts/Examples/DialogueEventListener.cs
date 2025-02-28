using UnityEngine;

public class DialogueEventListener : MonoBehaviour
{
    private void Start()
    {
        // 获取对话管理器
        DialogueManager dialogueManager = DialogueManager.Instance;
        if (dialogueManager != null)
        {
            // 添加事件监听
            dialogueManager.onDialogueStart.AddListener(OnDialogueStarted);
            dialogueManager.onDialogueEnd.AddListener(OnDialogueEnded);
            dialogueManager.onEventTriggered.AddListener(OnDialogueEvent);
        }
    }
    
    // 对话开始
    private void OnDialogueStarted()
    {
        Debug.Log("对话已开始");
        // 可以在这里暂停游戏、锁定控制等
    }
    
    // 对话结束
    private void OnDialogueEnded()
    {
        Debug.Log("对话已结束");
        // 可以在这里恢复游戏、解锁控制等
    }
    
    // 对话事件触发
    private void OnDialogueEvent(string eventID)
    {
        Debug.Log("对话事件: " + eventID);
        switch (eventID)
        {
            case "quest_accepted":
                // 添加任务到玩家的任务日志
                Debug.Log("任务已添加到玩家日志");
                break;
            
            case "give_item":
                // 给玩家物品
                Debug.Log("向玩家添加物品");
                break;
        }
    }
}