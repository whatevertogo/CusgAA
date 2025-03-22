/*
 * 用于处理分支对话的控制器
 * 通过选择不同的选项，进入不同的对话
 * 将方法与控制分离
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BranchingDialogueController : DialogueController,IBranchingDialogue
{

    /// <summary>
    /// 字典无法被初始化，所以写一个包装类，然后把每个包装类数据加入可被序列化的List里面去
    /// </summary> <summary>
    /// 使用字典的原因是字典用起来真的很爽,性能也好，就是初始化的时候性能会炸一丢丢
    /// 分支功能需要独立再创建一个DialogueSO
    /// </summary>

    [System.Serializable]
    public class DialogueOption{
        public string optionText; // 选项显示文本
        public string optionKey;  // 选项键值
        public DialogueSO dialogueData;
    }

    [Header("分支选项配置")]
    [SerializeField] private List<DialogueOption> branchOptionsList = new();
    [SerializeField] private GameObject optionsPanel; // 选项面板
    [SerializeField] private GameObject optionButtonPrefab; // 选项按钮预制体
    [SerializeField] private Transform optionsContainer; // 选项容器

    // 修改字典类型以匹配实际使用方式
    private Dictionary<string, DialogueOption> branchOptions = new();

    // 初始化分支对话控制器
    // 说明：
    // 1. 调用基类的Awake方法初始化基础对话组件
    // 2. 可以在这里添加分支对话特有的初始化逻辑
    protected override void Awake()
    {
        base.Awake();

        branchOptions.Clear();
        //初始化字典，将list的数据转到字典里面
        foreach(var option in branchOptionsList)
        {
            if(!string.IsNullOrEmpty(option.optionKey) && option.dialogueData != null)
            {
                branchOptions[option.optionKey] = option;
            }
        }
        
        // 确保选项面板初始状态为隐藏
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    // 开始分支对话
    // 说明：
    // 1. 调用基类的对话开始方法
    // 2. 输出分支对话开始的日志
    // 用途：当需要开始一段包含多个选项的对话时调用
    public override void StartDialogue()
    {
        base.StartDialogue(); //调用基类的方法
        Debug.Log("开始分支对话");
    }
    
    // 显示对话选项
    // 说明：
    // 1. 激活选项面板
    // 2. 清理旧的选项按钮
    // 3. 为每个选项创建按钮并设置点击事件
    // 用途：当对话到达分支点时，显示可选择的分支选项
    public void ShowDialogueOptions()
    {
        if (optionsPanel == null || optionsContainer == null)
        {
            Debug.LogError("选项面板或容器未设置");
            return;
        }
        
        // 激活选项面板
        optionsPanel.SetActive(true);
        
        // 清理现有选项
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 生成选项按钮,需要添加grid 那啥组件，就是能让组件并列或者竖列的组件(我不用，所以没测试略略略)
        foreach (var option in branchOptionsList)
        {
            if (option.dialogueData != null)
            {
                GameObject buttonObj = Instantiate(optionButtonPrefab, optionsContainer);
                Button button = buttonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                
                if (buttonText != null)
                {
                    buttonText.text = option.optionText;
                }
                
                if (button != null)
                {
                    string optionKey = option.optionKey;
                    button.onClick.AddListener(() => {
                        ChooseOption(optionKey);
                        HideDialogueOptions();
                    });
                }
            }
        }
    }
    
    // 隐藏对话选项面板
    public void HideDialogueOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    // 选择对话分支选项
    // 参数：option - 选择的选项标识符
    // 说明：
    // 1. 检查选项是否存在于分支选项字典中
    // 2. 如果存在，切换到对应的对话内容并显示
    // 3. 如果不存在，输出错误日志
    // 用途：玩家选择不同对话选项时调用
    public void ChooseOption(string optionKey)
    {
        if (branchOptions.ContainsKey(optionKey))
        {
            DialogueOption option = branchOptions[optionKey];
            if (option != null && option.dialogueData != null)
            {
                dialogueControl.SetDialogueSO(option.dialogueData);
                dialogueControl.ShowDialogue();
            }
            else
            {
                Debug.LogError("选项键值存在，但对话数据为空：" + optionKey);
            }
        }
        else
        {
            Debug.LogError("无效选项键值：" + optionKey);
        }
    }
    
    // 在对话结束时自动显示选项
    // 可以在适当的地方（如订阅对话结束事件）调用此方法
    public void ShowOptionsAfterDialogue()
    {
        if (dialogueControl != null)
        {
            dialogueControl.OnDialogueEnded += (sender, args) => {
                ShowDialogueOptions();
            };
        }
    }
    
    // 跳过当前对话，也隐藏选项面板
    public override void SkipDialogue()
    {
        base.SkipDialogue();
        HideDialogueOptions();
        Debug.Log("跳过分支对话");
    }
}