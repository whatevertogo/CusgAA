
using UnityEngine;

public interface IVoiceDialogue
{
    // 处理对话行变更事件
    private void OnDialogueLineChanged(object sender, DialogueControl.DialogueLineChangedEventArgs e)
    {
        // // 播放与当前行对应的语音
        // PlayVoice(e.LineIndex);
    }

    // 处理对话结束事件
    private void OnDialogueEnded(object sender, System.EventArgs e)
    {
        // // 停止当前播放的语音
        // if (audioSource != null && audioSource.isPlaying)
        // {
        //     audioSource.Stop();
        // }

        // // 取消事件订阅
        // if (dialogueControl != null)
        // {
        //     dialogueControl.OnDialogueLineChanged -= OnDialogueLineChanged;
        //     dialogueControl.OnDialogueEnded -= OnDialogueEnded;
        // }
    }

    // 根据对话行索引播放相应的语音片段
    private void PlayVoice(int lineIndex = 0)
    {
        // if (audioSource == null)
        // {
        //     Debug.LogError("AudioSource 组件不存在，无法播放语音");
        //     return;
        // }

        // if (DialogueClipDictionary == null || DialogueClipDictionary.Count == 0)
        // {
        //     Debug.LogWarning("没有可用的语音片段");
        //     return;
        // }

        // // 检查字典中是否包含该索引的语音片段
        // if (DialogueClipDictionary.TryGetValue(lineIndex, out AudioClip clip))
        // {
        //     // 停止当前播放的语音
        //     if (audioSource.isPlaying)
        //     {
        //         audioSource.Stop();
        //     }

        //     // 播放对应索引的语音
        //     audioSource.clip = clip;
        //     audioSource.Play();
        // }
        // else
        // {
        //     Debug.LogWarning($"未找到索引为 {lineIndex} 的语音片段");
        // }
    }

    private void OnDisable()
    {
        // // 确保在组件禁用时取消事件订阅
        // if (dialogueControl != null)
        // {
        //     dialogueControl.OnDialogueLineChanged -= OnDialogueLineChanged;
        //     dialogueControl.OnDialogueEnded -= OnDialogueEnded;
        // }
    }
}