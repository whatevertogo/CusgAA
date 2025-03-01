using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueControl : MonoBehaviour
{
    [FormerlySerializedAs("DialoguePanel")]
    [Header("对话框UI组件")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;

    [Header("对话内容")]
    [SerializeField] private Dialogue_SO dialogue_SO;

    [Header("对话显示速度")]
    [SerializeField] private float typingSpeed = 0.1f;
    
    private List<string> dialogueLinesList = new List<string>();
    
    private int currentLineIndex = 0;
    
    public GameObject DialoguePanel
    {
        get => dialoguePanel;
        set => dialoguePanel = value;
    }
    
    
    private void Awake()
    {
        // 检查必要组件是否存在
        if (dialoguePanel == null)
            Debug.LogError("DialoguePanel is not assigned!");
        if (dialogueText == null)
            Debug.LogError("dialogueText is not assigned!");
        if (dialogue_SO == null)
            Debug.LogError("dialogue_SO is not assigned!");
            
        // 从SO资源中加载对话内容
        if (dialogue_SO != null)
        {
            dialogueLinesList = dialogue_SO.dialoguelinesList;
            Debug.Log($"Loaded {dialogueLinesList.Count} dialogue lines");
        }
    }
    
    private void Start()
    {
        ShowDialogue();
    }
    
    private void ShowDialogue()
    {
        if (DialoguePanel != null)
        {
            DialoguePanel.SetActive(true);
            
            if (dialogueLinesList.Count > 0)
            {
                StartCoroutine(TypeDialogue());
                Debug.Log("Started typing dialogue");
            }
            else
            {
                Debug.LogWarning("No dialogue lines to display!");
            }
        }
    }
    
    private IEnumerator TypeDialogue()
    {
        int currentLineIndex = 0;
        
        foreach (string line in dialogueLinesList)
        {
            currentLineIndex++;
            
            dialogueText.text = "";
            
            foreach (char letter in line.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            Debug.Log($"Finished showing line {currentLineIndex}");
            yield return new WaitForSeconds(1);
        }
        
    }

    public void SkipDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        Debug.Log("Dialogue skipped");
    }

}