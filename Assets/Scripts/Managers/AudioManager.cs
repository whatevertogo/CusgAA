using UnityEngine;
using System.Collections.Generic;
using Managers;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AudioManager : Singleton<AudioManager>
{
    #region 音量
    [Header("音量数据")]
    // 音量属性，用于与UI滑动条绑定
    [Tooltip("主音量")]
    [SerializeField] private float _masterVolume = 1f;
    [Tooltip("背景音乐音量")]
    [SerializeField] private float _bgmVolume = 1f;
    [Tooltip("音效音量")]
    [SerializeField] private float _sfxVolume = 1f;
    #endregion
    [Header("音频源/播放器")]
    [Tooltip("背景音乐播放器")]
    [SerializeField] private AudioSource bgmSource;
    [Tooltip("音效播放器")]
    [SerializeField] private AudioSource sfxSource;
    [Header("音效音乐列表")]
    [Tooltip("音效列表")]
    public List<AudioSource> sfxSourceList;
    [Tooltip("BGM音乐列表")]
    private Dictionary<string, AudioClip> BGMClipDictionary = new();

    [Header("绑定UI条")]
    [SerializeField ] private Slider masterVolumeSlider;
    [SerializeField ] private Slider bgmVolumeSlider;

    // 音量改变事件，用于通知UI更新
    public event EventHandler<EventArgs> onVolumeChanged;//TODO-留下空间给UI更新音量的事件


    [System.Serializable]
    public class AudioClipData
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] private List<AudioClipData> audioClipDataList = new List<AudioClipData>();




    protected override void Awake()
    {
        base.Awake();
    }


    private void Start()
    {
        // 初始化BGM音频源
        InitializeBGMAudioSources();
    }


    private void Update(){
        //todo-把音量和滚动条绑定
        // 更新音量
        if (masterVolumeSlider != null)
        {
            MasterVolume = masterVolumeSlider.value;
        }

        if (bgmVolumeSlider != null)
        {
            BGMVolume = bgmVolumeSlider.value;
        }
    }

/// <summary>
/// 初始化音频源
/// </summary>
    private void InitializeBGMAudioSources()
    {
        foreach (var audioClipData in audioClipDataList)
        {
            BGMClipDictionary[audioClipData.name] = audioClipData.clip;
        }
    }

    /// <summary>
    /// 主音量
    /// </summary>
    /// <value></value>
    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = Mathf.Clamp01(value);
            UpdateVolumes();
            onVolumeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 通过属性得到背景音量
    /// </summary>
    /// <value></value>
    public float BGMVolume
    {
        get => _bgmVolume;
        set
        {
            _bgmVolume = Mathf.Clamp01(value);
            UpdateVolumes();
            onVolumeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 属性得到音乐音量
    /// </summary>
    /// <value></value>
    public float SFXVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            UpdateVolumes();
           onVolumeChanged?.Invoke(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.volume = _masterVolume * _bgmVolume;
        bgmSource.Play();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // 创建新的音效音源
        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.clip = clip;
        sfxSource.volume = _masterVolume * _sfxVolume;
        sfxSource.Play();

        sfxSourceList.Add(sfxSource);

        // 播放完成后销毁
        float clipLength = clip.length;
        Destroy(sfxSource, clipLength);
        sfxSourceList.Remove(sfxSource);
    }

    /// <summary>
    /// 更新所有音源的音量
    /// </summary>
    private void UpdateVolumes()
    {
        // 更新BGM音量
        if (bgmSource != null)
        {
            bgmSource.volume = _masterVolume * _bgmVolume;
        }

        // 更新所有音效音量
        foreach (var source in sfxSourceList)
        {
            if (source != null)
            {
                source.volume = _masterVolume * _sfxVolume;
            }
        }
    }

    /// <summary>
    /// 改变音频源的片段
    /// </summary>
    /// <param name="audioSource">目标音频源</param>
    /// <param name="clip">要播放的音频片段</param>
    private void ChangeBGMClip(AudioSource audioSource = default, string name = null)
    {
        if (audioSource == null) return;
        if (BGMClipDictionary.TryGetValue(name, out AudioClip Nowclip))
        {
            audioSource.clip = Nowclip;
        }
        else
        {
            Debug.Log("字典中没有clip值考虑是否没有在编辑器添加,或者并没有输入名字和audioSource，或者名字错误");
        }
    }

}
