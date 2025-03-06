/*对话Control类
 * 1. 显示对话框
 * 2. 逐字显示对话内容
 * 3. 跳过对话内容
 * 4. 切换对话内容
 * 5. 淡入淡出立绘（未实现）需要自己通过Dotween实现

*/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueControl : MonoBehaviour
{
    [FormerlySerializedAs("DialoguePanel")]
    [Header("对话框UI组件")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Button nextlineButton;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Header("对话内容")][SerializeField] private DialogueSO dialogue_SO;
    [Header("对话显示速度")][SerializeField] private float typingSpeed = 0.1f;
    [SerializeField] private float nextLineDelay = 2f;//下一行的时间

    private int _currentLineIndex;
    private List<string> dialogueLinesList = new();
    private bool _isTyping;
    private Coroutine _typingCoroutine;
    // dialoguePanel 的访问已经通过 ShowDialogue() 和其他方法进行了合理封装

    private void Awake()
    {
        // 检查必要组件是否存在
        if (dialoguePanel == null)
            Debug.LogError("DialoguePanel is not assigned!");
        if (dialogueText == null)
            Debug.LogError("dialogueText is not assigned!");
        if (dialogue_SO == null)
            Debug.LogError("dialogue_SO is not assigned!");
        if (nextlineButton is not null)
            nextlineButton.onClick.AddListener(SkipDialogueLines);

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
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);

            if (dialogueLinesList.Count > 0)
            {
                _currentLineIndex = 0;
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
        if (_isTyping)
        {
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            dialogueText.text = dialogueLinesList[_currentLineIndex];
            _isTyping = false;
            return;
        }

        // 清空文本框，准备显示下一行
        dialogueText.text = "";

        // 检查是否还有下一行对话
        if (_currentLineIndex + 1 < dialogueLinesList.Count)
        {
            _typingCoroutine = StartCoroutine(TypeDialogueLineByLine());
            _currentLineIndex++;
        }
        else
            // 所有对话行已显示完毕
            Debug.Log("All dialogue lines have been displayed");
        // 可选：关闭对话面板
        // dialoguePanel.SetActive(false);
    }

    private IEnumerator TypeDialogueLineByLine()
    {
        _isTyping = true;

        var currentLine = dialogueLinesList[_currentLineIndex];

        foreach (var letter in currentLine)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        _isTyping = false;
        Debug.Log($"Finished showing line {_currentLineIndex}");

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
        //TODO-其他工作
        Debug.Log("All dialogues skipped");
    }

    public void SetDialogueSO(DialogueSO newDialogueSO)
    {
        if (newDialogueSO is null)
        {
            Debug.LogError("新对话数据为空");
            return;
        }

        dialogue_SO = newDialogueSO;
        dialogueLinesList = dialogue_SO.dialoguelinesList;
        _currentLineIndex = 0;

        Debug.Log($"切换对话数据: {newDialogueSO.name}");
    
        ShowDialogue();
    }

    public void GoDialogueSOToLine(DialogueSO oldDialogueSO, int lineIndex)
    {
        if (oldDialogueSO is null)
        {
            Debug.LogError("旧对话数据不为空");
            return;
        }

        dialogue_SO = oldDialogueSO;
        dialogueLinesList = dialogue_SO.dialoguelinesList;
        _currentLineIndex = lineIndex;

        Debug.Log($"返回对话数据: {oldDialogueSO.name}");
    
        ShowDialogue();
    }


    public void FadeInCharacter(Image characterImage)
    {
        // DOTween实现立绘淡入
    }

    public void FadeOutCharacter(Image characterImage)
    {
        // DOTween实现立绘淡出
    }




}
