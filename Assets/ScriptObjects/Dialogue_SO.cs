using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue_SO : ScriptableObject
{
    [Header("对话基本信息")]
    public string characterName;               // 角色名称
    public Sprite characterSprite;             // 角色头像
    public List<string> dialogueLines = new List<string>(); // 对话内容列表
    
    [Header("对话高级设置")]
    public float typingSpeed = 0.05f;          // 打字速度
    public AudioClip voiceClip;                // 语音片段
    public bool autoAdvance = false;           // 是否自动进行下一句
    public float autoAdvanceDelay = 2.0f;      // 自动进行下一句的延迟时间
    
    [Header("对话分支选项")]
    public bool hasBranch = false;             // 是否有分支选择
    public List<DialogueBranch> branches = new List<DialogueBranch>(); // 分支选项列表
    
    [Header("事件触发")]
    public string eventID;                     // 事件ID, 用于触发特定的游戏事件
    
    [Header("情感表现")]
    public string emotion = "normal";          // 角色情感状态，如happy, sad, angry等
    
    [System.Serializable]
    public class DialogueBranch
    {
        public string optionText;              // 选项文本
        public Dialogue_SO nextDialogue;       // 选择后跳转的对话
        public string requiredFlag;            // 显示此选项需要的游戏标记
    }
    
    // 快速创建单行对话的辅助方法
    public static Dialogue_SO CreateSimple(string character, string text)
    {
        Dialogue_SO dialogue = CreateInstance<Dialogue_SO>();
        dialogue.characterName = character;
        dialogue.dialogueLines.Add(text);
        return dialogue;
    }
    
    // 快速添加对话行
    public void AddLine(string line)
    {
        dialogueLines.Add(line);
    }
    
    // 添加分支选项
    public void AddBranch(string option, Dialogue_SO next, string flag = "")
    {
        DialogueBranch branch = new DialogueBranch
        {
            optionText = option,
            nextDialogue = next,
            requiredFlag = flag
        };
        
        branches.Add(branch);
        hasBranch = true;
    }
}
