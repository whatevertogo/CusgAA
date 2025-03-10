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
    [FormerlySerializedAs("DialoguePanel")] [Header("对话框UI组件")] [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField] private Button nextlineButton;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Header("对话内容")] [SerializeField] private DialogueSO dialogue_SO;
    [Header("对话显示速度")] [SerializeField] private float typingSpeed = 0.1f;
    [SerializeField] private float nextLineDelay = 2f; //下一行的时间

    private int _currentLineIndex;
    private bool _isTyping;
    private Coroutine _typingCoroutine;

    private List<string> dialogueLinesList = new();
    // dialoguePanel 的访问已经通过 ShowDialogue() 和其他方法进行了合理封装

    // 初始化对话系统组件
    // 说明：
    // 1. 检查必要UI组件是否已正确配置
    // 2. 设置下一行按钮的点击事件监听
    // 3. 从SO资源加载对话内容到列表
    // 用途：确保所有必要组件和数据在游戏开始前准备就绪
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

    // 显示对话框并开始对话
    // 说明：
    // 1. 激活对话面板
    // 2. 如果有对话内容，从第一行开始显示
    // 3. 如果没有对话内容，输出警告日志
    // 用途：开始一段新的对话序列
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
    } // ReSharper disable Unity.PerformanceAnalysis
    // 显示下一行对话
    // 说明：
    // 1. 如果当前正在打字，则立即完成当前行
    // 2. 如果不在打字，清空文本准备显示下一行
    // 3. 如果还有更多对话，开始打字效果
    // 4. 如果对话结束，输出日志提示
    // 用途：推进对话进度，显示下一句对话内容
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
        {
            Debug.Log("All dialogue lines have been displayed");
        }
        // 可选：关闭对话面板
        // dialoguePanel.SetActive(false);
    }

    // 逐字显示对话文本的协程
    // 说明：
    // 1. 设置正在打字状态
    // 2. 逐个字符显示文本，每个字符间有时间间隔
    // 3. 显示完成后等待指定时间
    // 4. 自动显示下一行
    // 用途：实现打字机效果，增强对话的表现力
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

    // 跳过当前对话行，显示下一行
    // 说明：调用ShowNextLine显示下一行对话
    // 用途：玩家手动跳过当前对话行时调用
    public void SkipDialogueLines()
    {
        ShowNextLine();
        Debug.Log("Moving to next dialogue line");
    }

    // 跳过当前所有对话
    // 说明：停止所有协程，结束当前对话
    // 用途：玩家想要直接结束整段对话时调用
    public void SkipDialogue()
    {
        StopAllCoroutines();
        //TODO-其他工作
        Debug.Log("All dialogues skipped");
    }

    // 设置新的对话数据
    // 参数：newDialogueSO - 新的对话数据SO文件
    // 说明：
    // 1. 检查新对话数据是否有效
    // 2. 更新当前对话数据和列表
    // 3. 重置对话进度
    // 4. 开始显示新对话
    // 用途：切换到新的对话内容时调用
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

    // 跳转到指定对话数据的特定行
    // 参数：
    // - oldDialogueSO: 要跳转的对话数据
    // - lineIndex: 目标行索引
    // 说明：
    // 1. 检查对话数据有效性
    // 2. 设置对话数据和当前行索引
    // 3. 开始显示对话
    // 用途：在分支对话中返回特定位置时使用
    public void GoDialogueSOToLine(DialogueSO oldDialogueSO, int lineIndex)
    {
        if (oldDialogueSO is null)
        {
            Debug.LogError("旧对话数据为空");
            return;
        }

        dialogue_SO = oldDialogueSO;
        dialogueLinesList = dialogue_SO.dialoguelinesList;
        _currentLineIndex = lineIndex;

        Debug.Log($"返回对话数据: {oldDialogueSO.name}");

        ShowDialogue();
    }

    // 角色立绘淡入效果
    // 参数：characterImage - 要淡入的角色图片
    // 说明：通过DOTween实现立绘的淡入效果（待实现）
    // 用途：显示新角色或切换角色立绘时使用
    public void FadeInCharacter(Image characterImage)
    {
        // DOTween实现立绘淡入
    }

    // 角色立绘淡出效果
    // 参数：characterImage - 要淡出的角色图片
    // 说明：通过DOTween实现立绘的淡出效果（待实现）
    // 用途：隐藏角色或切换角色立绘时使用
    public void FadeOutCharacter(Image characterImage)
    {
        // DOTween实现立绘淡出
    }
}