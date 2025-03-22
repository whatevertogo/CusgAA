using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IBranchingDialogue
{
    public void ShowDialogueOptions()
    {
        // if (optionsPanel == null || optionsContainer == null)
        // {
        //     Debug.LogError("选项面板或容器未设置");
        //     return;
        // }

        // // 激活选项面板
        // optionsPanel.SetActive(true);

        // // 清理现有选项
        // foreach (Transform child in optionsContainer)
        // {
        //     Destroy(child.gameObject);
        // }

        // // 生成选项按钮,需要添加grid 那啥组件，就是能让组件并列或者竖列的组件(我不用，所以没测试略略略)
        // foreach (var option in branchOptionsList)
        // {
        //     if (option.dialogueData != null)
        //     {
        //         GameObject buttonObj = Instantiate(optionButtonPrefab, optionsContainer);
        //         Button button = buttonObj.GetComponent<Button>();
        //         TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        //         if (buttonText != null)
        //         {
        //             buttonText.text = option.optionText;
        //         }

        //         if (button != null)
        //         {
        //             string optionKey = option.optionKey;
        //             button.onClick.AddListener(() =>
        //             {
        //                 ChooseOption(optionKey);
        //                 HideDialogueOptions();
        //             });
        //         }
        //     }
        // }
    }
    // 隐藏对话选项面板
    public void HideDialogueOptions(){
    // {
    //     if (optionsPanel != null)
    //     {
    //         optionsPanel.SetActive(false);
    //     }
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
        // if (branchOptions.ContainsKey(optionKey))
        // {
        //     DialogueOption option = branchOptions[optionKey];
        //     if (option != null && option.dialogueData != null)
        //     {
        //         dialogueControl.SetDialogueSO(option.dialogueData);
        //         dialogueControl.ShowDialogue();
        //     }
        //     else
        //     {
        //         Debug.LogError("选项键值存在，但对话数据为空：" + optionKey);
        //     }
        // }
        // else
        // {
        //     Debug.LogError("无效选项键值：" + optionKey);
        // }
    }

    // 在对话结束时自动显示选项
    // 可以在适当的地方（如订阅对话结束事件）调用此方法
    public void ShowOptionsAfterDialogue()
    {
        // if (dialogueControl != null)
        // {
        //     dialogueControl.OnDialogueEnded += (sender, args) =>
        //     {
        //         ShowDialogueOptions();
        //     };
        // }
    }
}