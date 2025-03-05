/*播放对话的同时会播放声音
    目前需要手动添加声音片段
    添加一个AudioSource组件
*/

using UnityEngine;

public class VoiceDialogueController : DialogueController
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] voiceClips;

    protected override void Awake()
    {
        base.Awake();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public override void StartDialogue()
    {
        base.StartDialogue();
        PlayVoice();
    }

    private void PlayVoice()
    {
        if (audioSource != null && voiceClips.Length > 0)
        {
            audioSource.clip = voiceClips[0]; // 播放第一段语音
            audioSource.Play();
        }
    }
}