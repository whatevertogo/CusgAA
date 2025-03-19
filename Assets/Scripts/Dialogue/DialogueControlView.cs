using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueControlView : MonoBehaviour
{
    [Header("对话框UI组件")] 
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Button nextLineButton;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typingSpeed = 0.1f;
    
    // 事件定义
    public event EventHandler OnNextLineRequested;
    public event EventHandler OnDialogueSkipped;
    
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
        
        // 通知控制器请求下一行
        OnNextLineRequested?.Invoke(this, EventArgs.Empty);
    }
    
    public void ShowDialoguePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
    }
    
    public void HideDialoguePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
    
    public void TypeDialogueLine(string line, float delayBeforeNext = 0)
    {
        dialogueText.text = ""; // 清空文本
        
        // 停止任何正在进行的打字效果
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);
            
        // 启动新的打字效果
        _typingCoroutine = StartCoroutine(TypeLineCoroutine(line, delayBeforeNext));
    }
    
    public void CompleteCurrentLine()
    {
        if (_isTyping && _typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _isTyping = false;
            
            // 显示完整文本
            string currentLine = dialogueText.text;
            dialogueText.text = currentLine;
        }
    }
    
    private IEnumerator TypeLineCoroutine(string line, float delayBeforeNext)
    {
        _isTyping = true;
        
        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        _isTyping = false;
        
        // 如果设置了延迟，等待后自动请求下一行
        if (delayBeforeNext > 0)
        {
            yield return new WaitForSeconds(delayBeforeNext);
            OnNextLineRequested?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public void SkipAllDialogue()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);
            
        _isTyping = false;
        OnDialogueSkipped?.Invoke(this, EventArgs.Empty);
    }
    
    // 立绘淡入淡出方法
    public void FadeInCharacter(Image characterImage)
    {
        // DOTween实现立绘淡入
    }
    
    public void FadeOutCharacter(Image characterImage)
    {
        // DOTween实现立绘淡出
    }
}