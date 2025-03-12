using UnityEngine;
using System.Collections.Generic;
using Managers;
using System;

public class AudioManager : Singleton<AudioManager>
{

    // 音量属性，用于与UI滑动条绑定
    [SerializeField] private float _masterVolume = 1f;
    [SerializeField] private float _bgmVolume = 1f;
    [SerializeField] private float _sfxVolume = 1f;

    // 音量改变事件，用于通知UI更新
    public System.Action onVolumeChanged;

    [SerializeField] private AudioSource bgmSource;

    [SerializeField] private AudioSource sfxSourcePrefab;
    private List<AudioSource> sfxSources;

    [System.Serializable]
    public class AudioClipData
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] private List<AudioClipData> audioClipDataList = new List<AudioClipData>();




    // private void Awake()
    // {
    //     base.Awake();
    //Todo->将音量和滚动条什么之类的绑定
    // }


    // private void Start(){
    //     ChangeBGMClip(bgmSource, audioClipDataList[0].clip);
    // }//例子



    private void Start()
    {
        // 初始化音频源
        InitializeAudioSources();
    }

    private void InitializeAudioSources()
    {
        // 初始化BGM音源
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        // 初始化音效列表
        sfxSources = new List<AudioSource>();
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
            onVolumeChanged?.Invoke();
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
            onVolumeChanged?.Invoke();
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
            onVolumeChanged?.Invoke();
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

        sfxSources.Add(sfxSource);

        // 播放完成后销毁
        float clipLength = clip.length;
        Destroy(sfxSource, clipLength);
        sfxSources.Remove(sfxSource);
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
        foreach (var source in sfxSources)
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
    private void ChangeBGMClip(AudioSource audioSource, AudioClip clip)
    {
        if (audioSource == null) return;
        audioSource.clip = clip;
    }





}
