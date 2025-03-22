/// <summary>
/// 对话系统的核心控制类，负责管理对话流程和内容展示。
/// 
/// 功能：
/// - 作为对话系统的中央控制器，管理对话的加载、显示和进度
/// - 使用单例模式提供全局访问点
/// - 从DialogueSO加载对话数据并控制对话流程
/// - 通过DialogueControlView处理UI显示
/// - 提供事件系统用于监听对话状态变化
/// - 支持动态切换和跳转不同对话内容
/// 
/// 用法：
/// 1. 将此组件附加到场景中的GameObject上
/// 2. 设置DialogueSO作为对话内容来源
/// 3. 连接DialogueControlView处理UI显示
/// 4. 通过StartDialogue()开始对话，或者使用SetDialogueSO()切换对话内容
/// 
/// 注意：
/// - 目前在Start()中自动启动对话，可以通过需要再各个controller里面调整 
/// - 实现了对象池模式，可通过Instance静态属性全局访问
/// 
/// 未考虑使用单例模式，单例模式的生命周期个人很讨厌，而这里又没必要
/// 当然单例模式也没问题，场景里面多个对话共用一个Control里面的方法也是非常的good
/// </summary>


using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueControl : MonoBehaviour
{
    [Header("对话内容")]
    [SerializeField] private DialogueSO dialogue_SO;
    [Tooltip("到下一行时间")]
    [SerializeField] private float nextLineDelay = 2f;
    [Tooltip("对话视觉")]
    [SerializeField] private DialogueControlView dialogueView;

    private int _currentLineIndex;
    private List<string> dialogueLinesList = new();

    /// <summary>
    /// 事件定义,分别是对话开始，对话结束，暂时未用
    /// </summary>
    public event EventHandler OnDialogueStarted;
    public event EventHandler OnDialogueEnded;
    /// <summary>
    /// 对话行变更事件，方便语音播放捏~(￣▽￣)~*
    /// </summary> 
    public event EventHandler<DialogueLineChangedEventArgs> OnDialogueLineChanged;

    public class DialogueLineChangedEventArgs : EventArgs
    {
        public string DialogueLine;
        public int LineIndex;
    }

    #region 事件生命周期(用于订阅和取消事件)方便Control控制(观察者模式的体现MVC分离)
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
    #endregion

    /// <summary>
    /// 检查视图和SO是否为空
    /// </summary>
    private void Awake()
    {
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

    private void HandleNextLineRequested(object sender, EventArgs e)
    {
        ShowNextLine();
    }

    private void HandleDialogueSkipped(object sender, EventArgs e)
    {
        SkipDialogue();
    }
    /// <summary>
    /// 打开对话panel激活对话事件(未使用)
    /// 这里是控制对话的主要事件
    /// </summary>

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

    /// <summary>
    /// 立刻完成这一行
    /// </summary>
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

    /// <summary>
    /// 设置新的对话SO并默认为第0行，推荐无脑用下面的，因为下面的我默认的也是0行
    /// </summary>
    /// <param name="newDialogueSO"></param>

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

    /// <summary>
    /// 选择DialogueSO并选择跳到哪一行
    /// </summary>
    /// <param name="oldDialogueSO"></param>
    /// <param name="lineIndex"></param>
    public void GoDialogueSOToLine(DialogueSO oldDialogueSO, int lineIndex = 0)
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

/// <summary>
/// 跳过所有对话，隐藏对话面板
/// </summary>
    public void SkipDialogue()
    {
        Debug.Log("All dialogues skipped");
        OnDialogueEnded?.Invoke(this, EventArgs.Empty);

        // 确保停止任何可能正在进行的打字效果
        if (dialogueView != null)
        {
            dialogueView.CompleteCurrentLine(); // 先完成当前行的打字效果
            dialogueView.HideDialoguePanel();   // 然后隐藏对话面板
        }
    }
}