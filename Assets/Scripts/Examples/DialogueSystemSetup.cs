using UnityEngine;

public class DialogueSystemSetup : MonoBehaviour
{
    [Header("必要的预制体引用")]
    [SerializeField] private GameObject dialoguePanelPrefab;
    [SerializeField] private GameObject optionButtonPrefab;
    
    public void SetupDialogueSystem()
    {
        // 创建对话系统的GameObject
        GameObject dialogueSystemObj = new GameObject("DialogueSystem");
        DialogueManager dialogueManager = dialogueSystemObj.AddComponent<DialogueManager>();
        
        // 创建对话UI
        GameObject dialogueUI = Instantiate(dialoguePanelPrefab, dialogueSystemObj.transform);
        dialogueUI.name = "DialoguePanel";
        
        // 设置对话管理器引用
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        
        dialogueUI.transform.SetParent(canvas.transform, false);
        
        Debug.Log("对话系统已设置完成！");
    }
}