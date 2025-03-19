using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueControl : MonoBehaviour
{
    public static DialogueControl Instance { get; private set; }

    [Header("对话内容")]
    [SerializeField] private DialogueSO dialogue_SO;
    [SerializeField] private float nextLineDelay = 2f;
    [SerializeField] private DialogueControlView dialogueView;

    private int _currentLineIndex;
    private List<string> dialogueLinesList = new();
    
    // 事件定义
    public event EventHandler OnDialogueStarted;
    public event EventHandler OnDialogueEnded;
    public event EventHandler<DialogueLineChangedEventArgs> OnDialogueLineChanged;
    
    public class DialogueLineChangedEventArgs : EventArgs
    {
        public string DialogueLine;
        public int LineIndex;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        // 检查对话视图
        if (dialogueView == null)
        {
            dialogueView = FindFirstObjectByType<DialogueControlView>();
            if (dialogueView == null)
                Debug.LogError("DialogueControlView not found!");
        }
        
        // 从SO资源中加载对话内容
        if (dialogue_SO != null)
        {
            dialogueLinesList = dialogue_SO.dialoguelinesList;
            Debug.Log($"Loaded {dialogueLinesList.Count} dialogue lines");
        }
        else
            Debug.LogError("dialogue_SO is not assigned!");
    }
    
    private void OnEnable()
    {
        if (dialogueView != null)
        {
            dialogueView.OnNextLineRequested += HandleNextLineRequested;
            dialogueView.OnDialogueSkipped += HandleDialogueSkipped;
        }
    }
    
    private void OnDisable()
    {
        if (dialogueView != null)
        {
            dialogueView.OnNextLineRequested -= HandleNextLineRequested;
            dialogueView.OnDialogueSkipped -= HandleDialogueSkipped;
        }
    }
    /// <summary>
    /// start测试是否成功
    /// </summary>

    private void Start()
    {
        ShowDialogue();
    }

    
    
    private void HandleNextLineRequested(object sender, EventArgs e)
    {
        ShowNextLine();
    }
    
    private void HandleDialogueSkipped(object sender, EventArgs e)
    {
        SkipDialogue();
    }

    public void ShowDialogue()
    {
        dialogueView.ShowDialoguePanel();
        OnDialogueStarted?.Invoke(this, EventArgs.Empty);

        if (dialogueLinesList.Count > 0)
        {
            _currentLineIndex = 0;
            ShowNextLine();
        }
        else
        {
            Debug.LogWarning("No dialogue lines to display!");
        }
    }

    public void ShowNextLine()
    {
        if (_currentLineIndex < dialogueLinesList.Count)
        {
            string currentLine = dialogueLinesList[_currentLineIndex];
            
            dialogueView.TypeDialogueLine(currentLine, nextLineDelay);
            
            OnDialogueLineChanged?.Invoke(this, new DialogueLineChangedEventArgs 
            { 
                DialogueLine = currentLine, 
                LineIndex = _currentLineIndex 
            });
            
            _currentLineIndex++;
        }
        else
        {
            Debug.Log("All dialogue lines have been displayed");
            OnDialogueEnded?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public void SetDialogueSO(DialogueSO newDialogueSO)
    {
        if (newDialogueSO == null)
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
        if (oldDialogueSO == null)
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

    public void SkipDialogue()
    {
        Debug.Log("All dialogues skipped");
        OnDialogueEnded?.Invoke(this, EventArgs.Empty);
        dialogueView.HideDialoguePanel();
    }
}