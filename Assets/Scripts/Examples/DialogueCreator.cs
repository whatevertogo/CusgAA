using UnityEngine;

public class DialogueCreator : MonoBehaviour
{
    public void CreateSimpleDialogueExample()
    {
        // 创建简单对话
        Dialogue_SO dialogue = Dialogue_SO.CreateSimple("村民", "你好，旅行者！");
        dialogue.AddLine("今天天气真好，不是吗？");
        dialogue.AddLine("有什么我能帮你的吗？");
        
        // 设置对话属性
        dialogue.typingSpeed = 0.03f; // 更快的打字速度
        
        // 开始对话
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}