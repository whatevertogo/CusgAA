/*播放对话的同时会播放声音
    目前需要手动添加声音片段
    添加一个AudioSource组件
*/

using UnityEngine;

public class VoiceDialogueController : DialogueController
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] voiceClips;

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
        PlayVoice();
    }

    // 播放语音片段
    // 说明：
    // 1. 检查AudioSource组件和语音片段是否存在
    // 2. 设置并播放第一段语音
    // TODO: 可以扩展为根据对话进度播放不同的语音片段
    private void PlayVoice()
    {
        if (audioSource != null && voiceClips.Length > 0)
        {
            audioSource.clip = voiceClips[0]; // 播放第一段语音
            audioSource.Play();
        }
    }
}
