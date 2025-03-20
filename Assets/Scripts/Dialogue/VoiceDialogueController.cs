/*播放对话的同时会播放声音
    目前需要手动添加声音片段
    添加一个AudioSource组件
*/

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceDialogueController : DialogueController
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> voiceClips;

    // 初始化语音对话控制器
    // 说明：
    // 1. 调用基类的Awake方法初始化基础对话组件
    // 2. 检查并获取AudioSource组件
    // 用途：确保语音播放所需的组件都已准备就绪
    protected override void Awake()
    {
        base.Awake();
        if (audioSource == null) 
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("没有找到 AudioSource 组件，正在自动添加");
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    // 开始带语音的对话
    // 说明：
    // 1. 调用基类的对话开始方法
    // 2. 开始播放对应的语音片段
    // 用途：开始一段带有配音的对话内容
    public override void StartDialogue()
    {
        base.StartDialogue();
        // 订阅对话行变更事件，以便播放相应的语音
        if (dialogueControl != null)
        {
            dialogueControl.OnDialogueLineChanged += OnDialogueLineChanged;
            dialogueControl.OnDialogueEnded += OnDialogueEnded;
        }
    }
    
    // 处理对话行变更事件
    private void OnDialogueLineChanged(object sender, DialogueControl.DialogueLineChangedEventArgs e)
    {
        // 播放与当前行对应的语音
        PlayVoice(e.LineIndex);
    }
    
    // 处理对话结束事件
    private void OnDialogueEnded(object sender, System.EventArgs e)
    {
        // 停止当前播放的语音
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        // 取消事件订阅
        if (dialogueControl != null)
        {
            dialogueControl.OnDialogueLineChanged -= OnDialogueLineChanged;
            dialogueControl.OnDialogueEnded -= OnDialogueEnded;
        }
    }

    // 根据对话行索引播放相应的语音片段
    private void PlayVoice(int lineIndex = 0)
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource 组件不存在，无法播放语音");
            return;
        }
        
        if (voiceClips == null || voiceClips.Count == 0)
        {
            Debug.LogWarning("没有可用的语音片段");
            return;
        }
        
        // 检查索引是否有效
        if (lineIndex >= 0 && lineIndex < voiceClips.Count)
        {
            // 停止当前播放的语音
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            // 播放对应索引的语音
            audioSource.clip = voiceClips[lineIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"语音片段索引 {lineIndex} 超出范围 (0-{voiceClips.Count - 1})");
        }
    }
    
    private void OnDisable()
    {
        // 确保在组件禁用时取消事件订阅
        if (dialogueControl != null)
        {
            dialogueControl.OnDialogueLineChanged -= OnDialogueLineChanged;
            dialogueControl.OnDialogueEnded -= OnDialogueEnded;
        }
    }
}