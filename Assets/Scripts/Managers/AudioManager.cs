using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    // 音量属性，用于与UI滑动条绑定
    [SerializeField] private float _masterVolume = 1f;
    [SerializeField] private float _bgmVolume = 1f;
    [SerializeField] private float _sfxVolume = 1f;

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

    // 音量改变事件，用于通知UI更新
    public System.Action onVolumeChanged;

    private AudioSource bgmSource;
    private List<AudioSource> sfxSources;

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // 初始化BGM音源
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        
        // 初始化音效列表
        sfxSources = new List<AudioSource>();
    }

    // 播放背景音乐
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        
        bgmSource.clip = clip;
        bgmSource.volume = _masterVolume * _bgmVolume;
        bgmSource.Play();
    }

    // 停止背景音乐
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // 播放音效
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

    // 更新所有音源的音量
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
    ///

    

}
