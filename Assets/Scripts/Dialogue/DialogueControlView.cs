/// <summary>
/// 
/// 实现逻辑与视觉效果的分离，本来打算用事件通知的，后来想想都差不多
/// 对话系统的用户界面控制类，负责对话内容的可视化呈现和用户交互。
/// 
/// 功能：
/// - 管理对话框UI的显示和隐藏
/// - 实现打字机效果文本显示
/// - 处理用户输入（如点击"下一行"按钮）
/// - 通过事件系统与DialogueControl通信
/// - 提供立绘角色的淡入淡出效果
/// 
/// 用法：
/// 1. 将此组件附加到含有对话UI元素的GameObject上
/// 2. 设置必要的UI组件引用（对话面板、文本组件、按钮等）
/// 3. 通过DialogueControl调用其方法来控制对话显示
/// 4. 订阅事件以响应用户交互
/// 
/// 注意：
/// - 需要正确设置UI组件引用，否则会输出错误日志
/// - 打字机效果可通过typingSpeed参数调整
/// - 立绘淡入淡出功能需要使用DOTween实现（当前为占位方法）
/// </summary>

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueControlView : MonoBehaviour
{
    [Header("对话框UI组件")]
    [Tooltip("对话Panel")]
    [SerializeField] private GameObject dialoguePanel;
    [Tooltip("进入下一行或者完成这一行的按钮")]
    [SerializeField] private Button nextLineButton;
    [Tooltip("对话Text组件")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Tooltip("打字速度")]
    [SerializeField] private float typingSpeed = 0.1f;


    /// <summary>
    /// 未使用 
    /// </summary>
    public event EventHandler OnNextLineRequested;
    public event EventHandler OnDialogueSkipped;

    /// <summary>
    /// 私有变量
    /// </summary> 

    private string currentLine;
    private bool _isTyping;
    private Coroutine _typingCoroutine;

    private void Awake()
    {
        // 检查必要UI组件
        if (dialoguePanel == null) Debug.LogError("DialoguePanel is not assigned!");
        if (dialogueText == null) Debug.LogError("dialogueText is not assigned!");

        // 设置按钮监听
        if (nextLineButton != null)
            nextLineButton.onClick.AddListener(RequestNextLine);
    }

    private void RequestNextLine()
    {
        // 如果正在打字，立即完成
        if (_isTyping)
        {
            CompleteCurrentLine();
            return;
        }

        // 通知控制器请求下一行,未使用哦
        OnNextLineRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 打开对话panel
    /// </summary>
    public void ShowDialoguePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
    }

    /// <summary>
    /// 关闭对话panel
    /// </summary>
    public void HideDialoguePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// 打字
    /// </summary>
    /// <param name="line"></param>
    /// <param name="delayNextWord"></param>

    public void TypeDialogueLine(string line, float delayNextWord = default)
    {
        dialogueText.text = ""; // 清空文本
        currentLine = line;//设置currenLine等于当前的对话句子

        // 停止任何正在进行的打字效果
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        // 启动新的打字效果
        _typingCoroutine = StartCoroutine(TypeLineCoroutine(line, delayNextWord));
    }

    /// <summary>
    /// 完成当前行
    /// </summary>
    public void CompleteCurrentLine()
    {
        if (_isTyping && _typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _isTyping = false;

            // 直接显示完整文本
            dialogueText.text = currentLine;
        }
    }

    /// <summary>
    /// 标记正在打字,通过协程控制打字机效果
    /// </summary>
    /// <param name="line"></param>
    /// <param name="delayNextWord"></param>
    /// <returns></returns>

    private IEnumerator TypeLineCoroutine(string line, float delayNextWord)
    {
        _isTyping = true;

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        _isTyping = false;

        // 如果设置了延迟，等待后自动请求下一行
        if (delayNextWord > 0)
        {
            yield return new WaitForSeconds(delayNextWord);
            OnNextLineRequested?.Invoke(this, EventArgs.Empty);//激活下一行的事件未使用哦 
        }
    }

    /// <summary>
    /// 跳过对话
    /// </summary>

    public void SkipAllDialogue()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _isTyping = false;
        OnDialogueSkipped?.Invoke(this, EventArgs.Empty);
    }

    // 暂停打字效果
    public void PauseTyping()
    {
        if (_isTyping && _typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _isTyping = false;
        }
    }

    // 恢复打字效果
    public void ResumeTyping()
    {
        if (!_isTyping && currentLine != null)
        {
            // 恢复打字效果，从当前显示的文本继续
            string remainingText = currentLine.Substring(dialogueText.text.Length);
            if (!string.IsNullOrEmpty(remainingText))
            {
                _typingCoroutine = StartCoroutine(TypeLineCoroutine(remainingText, 0));
            }
        }
    }



    // 设置打字速度
    public void SetTypingSpeed(float speed)
    {
        if (speed > 0)
        {
            typingSpeed = speed;
        }
    }




    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }

    // 立绘淡入淡出方法,自己写加油哦！
    // 才不是因为我没学过多少这个插件
    public void FadeInCharacter(Image characterImage)
    {
        // DOTween实现立绘淡入
    }

    public void FadeOutCharacter(Image characterImage)
    {
        // DOTween实现立绘淡出
    }
}