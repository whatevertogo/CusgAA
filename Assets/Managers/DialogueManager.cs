using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject dialoguePanel;                   // 对话面板
    public TextMeshProUGUI characterNameText;          // 角色名称文本
    public TextMeshProUGUI dialogueText;               // 对话文本
    public Image characterImage;                       // 角色图像     
    public Image characterImage1;                      // 角色图像
    public GameObject continueIcon;                    // 继续对话提示图标
    public GameObject optionsPanel;                    // 选项面板
    public GameObject optionButtonPrefab;              // 选项按钮预制体
    public Transform optionsContainer;                 // 选项容器

    [Header("对话设置")]
    public float defaultTypingSpeed = 0.05f;           // 默认打字速度
    public AudioSource voiceSource;                    // 语音播放器
    public bool skipOnClick = true;                    // 点击时是否跳过当前打字效果
    public bool autoHidePanel = true;                  // 对话结束后是否自动隐藏面板

    [Header("事件")]
    public UnityEvent onDialogueStart;                 // 对话开始事件
    public UnityEvent onDialogueEnd;                   // 对话结束事件
    public UnityEvent<string> onEventTriggered;        // 事件触发

    // 内部状态
    private List<Dialogue_SO> dialogueQueue = new List<Dialogue_SO>();  // 对话队列
    private Dialogue_SO currentDialogue;               // 当前对话
    private int currentLineIndex = 0;                  // 当前行索引
    private bool isTyping = false;                     // 是否正在打字
    private bool dialogueActive = false;               // 对话是否激活
    private bool waitingForInput = false;              // 是否等待输入
    private List<GameObject> optionButtons = new List<GameObject>();   // 选项按钮列表
    private Dictionary<string, bool> gameFlags = new Dictionary<string, bool>();  // 游戏标记

    // 单例模式
    private static DialogueManager _instance;
    public static DialogueManager Instance { get { return _instance; } }

    [SerializeField] private DialogueTextEffects textEffects;

    private void Awake()
    {
        // 确保场景中只有一个对话管理器
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // 初始化时隐藏对话面板
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // 启动对话
    public void StartDialogue(Dialogue_SO dialogue)
    {
        if (dialogue == null) return;

        StopAllCoroutines();//停止所有协程
        dialogueQueue.Clear();//清空对话队列
        dialogueQueue.Add(dialogue);//添加对话到队列
        currentLineIndex = 0;//当前行索引为0
        dialogueActive = true;//对话激活

        ShowDialoguePanel();//显示对话面板
        onDialogueStart?.Invoke();//触发对话开始事件

        StartCoroutine(ProcessDialogue());//开始处理对话
    }

    // 将对话添加到队列
    public void QueueDialogue(Dialogue_SO dialogue)
    {
        if (dialogue == null) return;

        dialogueQueue.Add(dialogue);

        if (!dialogueActive)
        {
            dialogueActive = true;
            ShowDialoguePanel();
            StartCoroutine(ProcessDialogue());
        }
    }

    // 处理对话流程
    private IEnumerator ProcessDialogue()
    {
        while (dialogueQueue.Count > 0)
        {
            currentDialogue = dialogueQueue[0];
            currentLineIndex = 0;

            // 更新角色信息
            UpdateCharacterInfo();

            // 播放语音
            if (currentDialogue.voiceClip != null && voiceSource != null)
            {
                voiceSource.clip = currentDialogue.voiceClip;
                voiceSource.Play();
            }

            // 如果这个对话有事件ID，则触发事件
            if (!string.IsNullOrEmpty(currentDialogue.eventID))
            {
                onEventTriggered?.Invoke(currentDialogue.eventID);
            }

            // 显示每一行对话
            while (currentLineIndex < currentDialogue.dialogueLines.Count)
            {
                string currentLine = currentDialogue.dialogueLines[currentLineIndex];

                // 打字机效果显示文本
                isTyping = true;
                if (continueIcon != null) continueIcon.SetActive(false);

                if (textEffects != null)
                {
                    // 使用增强的文本效果
                    yield return textEffects.StartTyping(currentLine, currentDialogue.typingSpeed);
                }
                else
                {
                    // 使用简单的打字效果
                    yield return TypeDialogueLine(currentLine, currentDialogue.typingSpeed);
                }

                isTyping = false;
                if (continueIcon != null) continueIcon.SetActive(true);

                // 自动前进
                if (currentDialogue.autoAdvance)
                {
                    yield return new WaitForSeconds(currentDialogue.autoAdvanceDelay);
                    currentLineIndex++;
                }
                else
                {
                    // 等待用户输入
                    waitingForInput = true;
                    yield return new WaitUntil(() => !waitingForInput);
                }
            }

            // 检查是否有分支选择
            if (currentDialogue.hasBranch)
            {
                yield return ShowBranchOptions();
            }

            // 移除已处理的对话
            dialogueQueue.RemoveAt(0);
        }

        // 结束对话
        EndDialogue();
    }

    // 更新角色信息
    private void UpdateCharacterInfo()
    {
        if (characterNameText != null)
            characterNameText.text = currentDialogue.characterName;

        if (characterImage != null && currentDialogue.characterSprite != null)
        {
            characterImage.sprite = currentDialogue.characterSprite;
            characterImage.gameObject.SetActive(true);
        }
        else if (characterImage != null)
        {
            characterImage.gameObject.SetActive(false);
        }
    }

    // 打字机效果
    private IEnumerator TypeDialogueLine(string line, float speed)
    {
        dialogueText.text = "";  // 先清空文本
        float actualSpeed = speed > 0 ? speed : defaultTypingSpeed;

        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text = line.Substring(0, i + 1);  // 使用Substring而不是+=
            yield return new WaitForSeconds(actualSpeed);
        }
    }

    // 显示分支选项
    private IEnumerator ShowBranchOptions()
    {
        if (optionsPanel == null || optionButtonPrefab == null || optionsContainer == null)
            yield break;

        // 清除之前的选项按钮
        ClearOptionButtons();

        // 创建新的选项按钮
        foreach (var branch in currentDialogue.branches)
        {
            // 检查是否满足显示条件
            if (!string.IsNullOrEmpty(branch.requiredFlag) &&
                (!gameFlags.ContainsKey(branch.requiredFlag) || !gameFlags[branch.requiredFlag]))
            {
                continue; // 如果需要特定标记但没有，则跳过此选项
            }

            GameObject buttonObj = Instantiate(optionButtonPrefab, optionsContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = branch.optionText;

            if (button != null)
            {
                Dialogue_SO nextDialogue = branch.nextDialogue;
                button.onClick.AddListener(() => SelectBranch(nextDialogue));
            }

            optionButtons.Add(buttonObj);
        }

        // 显示选项面板
        optionsPanel.SetActive(true);
        // 等待玩家选择
        waitingForInput = true;
        yield return new WaitUntil(() => !waitingForInput);
        // 隐藏选项面板
        optionsPanel.SetActive(false);
    }

    // 选择分支
    public void SelectBranch(Dialogue_SO nextDialogue)
    {
        if (nextDialogue != null)
        {
            dialogueQueue.Insert(0, nextDialogue);
        }
        waitingForInput = false;
        ClearOptionButtons();
    }

    // 清除选项按钮
    private void ClearOptionButtons()
    {
        foreach (var button in optionButtons)
        {
            Destroy(button);
        }
        optionButtons.Clear();

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // 继续对话（用户输入）
    public void ContinueDialogue()
    {
        if (!dialogueActive) return;

        if (isTyping)
        {
            // 如果正在打字，则跳过打字效果
            if (skipOnClick)
            {
                if (textEffects != null)
                {
                    textEffects.CompleteTyping();
                }
                else
                {
                    StopAllCoroutines();
                }
                if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
                {
                    dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];
                    isTyping = false;
                    if (continueIcon != null) continueIcon.SetActive(true);
                }
            }
        }
        else if (waitingForInput)
        {
            // 进入下一行
            waitingForInput = false;
            currentLineIndex++;
        }
    }

    // 设置游戏标记,作用是分支系统，如果出bug找不到哪里就用这个强制防止对话重复
    public void SetFlag(string flag, bool value = true)
    {
        gameFlags[flag] = value;
    }

    // 检查游戏标记，如果有分支选项，需要检查是否满足条件，如果出bug找不到哪里就用这个强制防止对话重复
    public bool CheckFlag(string flag)
    {
        return gameFlags.ContainsKey(flag) && gameFlags[flag];
    }

    // 结束对话
    public void EndDialogue()
    {
        dialogueActive = false;
        if (autoHidePanel && dialoguePanel != null)
            dialoguePanel.SetActive(false);

        onDialogueEnd?.Invoke();
    }

    // 显示对话面板
    private void ShowDialoguePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
    }

    // 添加新对话到列表
    public void AddDialogue(Dialogue_SO newDialogue)
    {
        if (newDialogue != null)
            dialogueQueue.Add(newDialogue);
    }

    // 跳过当前对话
    public void SkipDialogue()
    {
        if (dialogueQueue.Count > 0)
            dialogueQueue.RemoveAt(0);

        if (dialogueQueue.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(ProcessDialogue());
        }
        else
        {
            EndDialogue();
        }
    }

    // 是否有对话正在进行，修bug用
    public bool IsDialogueActive()
    {
        return dialogueActive;
    }

    // 检查输入
    private void Update()
    {
        if (dialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            ContinueDialogue();
        }
    }
}
