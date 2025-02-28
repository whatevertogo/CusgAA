using UnityEngine;

public class SimpleDialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue_SO dialogue;
    
    //开始对话
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartDialogue();
        }
    }
    //结束对话
    
    // 修改现有StartDialogue方法，使其更具体
    public void StartDialogue()
    {
        if (dialogue != null && DialogueManager.Instance != null)
        {
            Debug.Log("开始对话：" + dialogue.characterName);
            DialogueManager.Instance.StartDialogue(dialogue);
        }
        else
        {
            Debug.LogWarning("无法开始对话，对话数据或管理器为空");
        }
    }
}