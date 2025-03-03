using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueControl : MonoBehaviour
{
    [FormerlySerializedAs("DialoguePanel")] [Header("对话框UI组件")] [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("对话内容")] [SerializeField] private DialogueSO dialogue_SO;

    [Header("对话显示速度")] [SerializeField] private float typingSpeed = 0.1f;

    [SerializeField] private float nextLineDelay = 2f;//下一行的时间

    private int currentLineIndex;

    private List<string> dialogueLinesList = new();

    private bool isTyping;
    private Coroutine typingCoroutine;

    public GameObject DialoguePanel { get => dialoguePanel; set => dialoguePanel = value; }

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

    // 显示对话框
    public void ShowDialogue()
    {
        if (DialoguePanel != null)
        {
            DialoguePanel.SetActive(true);

            if (dialogueLinesList.Count > 0)
            {
                currentLineIndex = 0;
                ShowNextLine(); //调用对话
            }
            else
            {
                Debug.LogWarning("No dialogue lines to display!");
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ShowNextLine()
    {
        // 如果正在打字，停止当前打字过程并立即显示完整的当前行
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.text = dialogueLinesList[currentLineIndex];
            isTyping = false;
            return;
        }

        // 清空文本框，准备显示下一行
        dialogueText.text = "";

        // 检查是否还有下一行对话
        if (currentLineIndex + 1 < dialogueLinesList.Count)
        {
            typingCoroutine = StartCoroutine(TypeDialogueLineByLine());
            currentLineIndex++;
        }
        else
            // 所有对话行已显示完毕
            Debug.Log("All dialogue lines have been displayed");
        // 可选：关闭对话面板
        // DialoguePanel.SetActive(false);
    }

    private IEnumerator TypeDialogueLineByLine()
    {
        isTyping = true;

        var currentLine = dialogueLinesList[currentLineIndex];

        foreach (var letter in currentLine)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        Debug.Log($"Finished showing line {currentLineIndex}");

        yield return new WaitForSeconds(nextLineDelay);
        ShowNextLine();
    }

    //进行下一行 
    public void SkipDialogueLines()
    {
        ShowNextLine();
        Debug.Log("Moving to next dialogue line");
    }

    //跳过所有行
    public void SkipDialogue()
    {
        StopAllCoroutines();
        //todo-其他工作
        Debug.Log("All dialogues skipped");
    }


    public void FadeInCharacter(Image characterImage) {
    // DOTween实现立绘淡入
    }

    public void FadeOutCharacter(Image characterImage) {
    // DOTween实现立绘淡出
    }

    
    
    
}