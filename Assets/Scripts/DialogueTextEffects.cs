using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueTextEffects : MonoBehaviour
{
    [Header("打字效果设置")]
    [SerializeField] private float defaultTypeSpeed = 0.05f;
    [SerializeField] private AudioSource typingSoundSource;
    [SerializeField] private List<AudioClip> typingSounds;
    [SerializeField] private float typingSoundVolume = 0.5f;
    [SerializeField] private int typingSoundFrequency = 2; // 每N个字符播放一次声音

    [Header("颤抖效果设置")]
    [SerializeField] private float shakeIntensity = 2f;
    [SerializeField] private float shakeSpeed = 50f;

    [Header("波浪效果设置")]
    [SerializeField] private float waveIntensity = 5f;
    [SerializeField] private float waveSpeed = 2f;

    // 添加运行时属性访问器
    #region 运行时参数访问
    // 打字效果参数
    public float DefaultTypeSpeed { get => defaultTypeSpeed; set => defaultTypeSpeed = value; }
    public float TypingSoundVolume { get => typingSoundVolume; set => typingSoundVolume = value; }
    public int TypingSoundFrequency { get => typingSoundFrequency; set => typingSoundFrequency = value; }
    
    // 颤抖效果参数
    public float ShakeIntensity { get => shakeIntensity; set => shakeIntensity = value; }
    public float ShakeSpeed { get => shakeSpeed; set => shakeSpeed = value; }
    
    // 波浪效果参数
    public float WaveIntensity { get => waveIntensity; set => waveIntensity = value; }
    public float WaveSpeed { get => waveSpeed; set => waveSpeed = value; }
    #endregion

    // 内部变量
    private TextMeshProUGUI textComponent;
    private string fullText = "";
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool skipTyping = false;
    private Dictionary<int, TextEffect> characterEffects = new Dictionary<int, TextEffect>();

    // 标签正则表达式
    private static readonly Regex ColorTagRegex = new Regex(@"<color[^>]*>(.*?)</color>", RegexOptions.Compiled);
    private static readonly Regex SizeTagRegex = new Regex(@"<size[^>]*>(.*?)</size>", RegexOptions.Compiled);
    private static readonly Regex CustomTagRegex = new Regex(@"<([a-zA-Z]+)>(.*?)</\1>", RegexOptions.Compiled);
    private static readonly Regex CustomEffectTagRegex = new Regex(@"<effect=([a-zA-Z]+)>(.*?)</effect>", RegexOptions.Compiled);
    private static readonly Regex SpeedTagRegex = new Regex(@"<speed=([0-9\.]+)>(.*?)</speed>", RegexOptions.Compiled);
    private static readonly Regex PauseTagRegex = new Regex(@"<pause=([0-9\.]+)>", RegexOptions.Compiled);

    /// <summary>
    /// 文本效果类型
    /// </summary>
    public enum EffectType
    {
        None,
        Speed,
        Pause,
        Shake,
        Wave
    }
    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// 开始打字效果
    /// </summary>
    /// <param name="text">需要显示的文本</param>
    /// <param name="speed">打字速度（0表示使用默认速度）</param>
    /// <returns></returns>
    public Coroutine StartTyping(string text, float speed = 0)
    {
        // 停止当前打字效果（如果有）
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 处理文本中的标签
        fullText = text;
        characterEffects.Clear();
        skipTyping = false;
        
        // 开始新的打字效果
        isTyping = true;
        typingCoroutine = StartCoroutine(TypeText(speed));
        return typingCoroutine;
    }

    /// <summary>
    /// 立即完成打字效果
    /// </summary>
    public void CompleteTyping()
    {
        if (isTyping)
        {
            skipTyping = true;
        }
    }

    /// <summary>
    /// 打字效果协程
    /// </summary>
    private IEnumerator TypeText(float speed)
    {
        // 处理标签
        string processedText = ProcessTags(fullText);
        
        // 设置打字速度
        float typeSpeed = speed > 0 ? speed : defaultTypeSpeed;
        
        // 清空文本
        textComponent.text = "";
        textComponent.maxVisibleCharacters = 0;
        
        // 设置完整文本（带标签）
        textComponent.text = processedText;
        
        // 逐字显示
        int totalChars = textComponent.textInfo.characterCount;
        int charCounter = 0;
        
        for (int i = 0; i <= totalChars; i++)
        {
            // 检查是否完成打字
            if (skipTyping)
            {
                textComponent.maxVisibleCharacters = totalChars;
                break;
            }

            textComponent.maxVisibleCharacters = i;
            
            // 如果当前字符不是空格和标点，播放打字音效
            if (i < totalChars && !char.IsWhiteSpace(processedText[charCounter]) && !char.IsPunctuation(processedText[charCounter]))
            {
                if (typingSoundSource != null && typingSounds.Count > 0 && i % typingSoundFrequency == 0)
                {
                    AudioClip clip = typingSounds[Random.Range(0, typingSounds.Count)];
                    typingSoundSource.PlayOneShot(clip, typingSoundVolume);
                }
            }
            
            // 检查是否有暂停标签
            if (characterEffects.TryGetValue(i, out TextEffect effect) && effect.type == EffectType.Pause)
            {
                yield return new WaitForSeconds(effect.value);
            }
            else
            {
                // 根据标签调整速度
                float currentSpeed = typeSpeed;
                if (characterEffects.TryGetValue(i, out TextEffect speedEffect) && speedEffect.type == EffectType.Speed)
                {
                    currentSpeed = speedEffect.value;
                }
                
                yield return new WaitForSeconds(currentSpeed);
            }
            
            charCounter++;
        }
        
        // 完成打字效果
        isTyping = false;
        typingCoroutine = null;
    }

    /// <summary>
    /// 处理文本中的标签
    /// </summary>
    private string ProcessTags(string input)
    {
        string result = input;

        // 处理速度标签
        result = SpeedTagRegex.Replace(result, match => {
            string speedText = match.Groups[1].Value;
            string content = match.Groups[2].Value;
            
            if (float.TryParse(speedText, out float speedValue))
            {
                // 添加到效果字典
                int startIndex = match.Index;
                for (int i = 0; i < content.Length; i++)
                {
                    characterEffects[startIndex + i] = new TextEffect { type = EffectType.Speed, value = speedValue };
                }
            }
            
            return content;
        });

        // 处理暂停标签
        result = PauseTagRegex.Replace(result, match => {
            string pauseText = match.Groups[1].Value;
            
            if (float.TryParse(pauseText, out float pauseValue))
            {
                // 添加到效果字典
                characterEffects[match.Index] = new TextEffect { type = EffectType.Pause, value = pauseValue };
            }
            
            return "";
        });

        // 处理效果标签
        result = CustomEffectTagRegex.Replace(result, match => {
            string effectType = match.Groups[1].Value.ToLower();
            string content = match.Groups[2].Value;
            
            EffectType type = EffectType.None;
            
            switch (effectType)
            {
                case "shake":
                    type = EffectType.Shake;
                    break;
                case "wave":
                    type = EffectType.Wave;
                    break;
                // 可以添加更多效果类型
            }
            
            if (type != EffectType.None)
            {
                // 添加到效果字典
                int startIndex = match.Index;
                for (int i = 0; i < content.Length; i++)
                {
                    characterEffects[startIndex + i] = new TextEffect { type = type };
                }
            }
            
            return content;
        });

        return result;
    }

    /// <summary>
    /// 每帧更新文本动画效果
    /// </summary>
    private void Update()
    {
        if (textComponent == null || textComponent.textInfo == null || textComponent.textInfo.characterInfo == null) return;
        
        // 应用文本效果（颤抖、波浪等）
        for (int i = 0; i < textComponent.textInfo.characterCount; i++)
        {
            // 跳过不可见字符
            if (!textComponent.textInfo.characterInfo[i].isVisible) continue;

            // 检查该字符是否有特效
            if (!characterEffects.TryGetValue(i, out TextEffect effect)) continue;

            // 应用效果
            TMP_CharacterInfo charInfo = textComponent.textInfo.characterInfo[i];
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            
            Vector3[] vertices = textComponent.textInfo.meshInfo[materialIndex].vertices;
            
            // 原始顶点位置
            Vector3 offset = Vector3.zero;
            
            // 根据效果类型应用相应效果
            switch (effect.type)
            {
                case EffectType.Shake:
                    offset = new Vector3(
                        Mathf.Sin(Time.time * shakeSpeed + i) * shakeIntensity,
                        Mathf.Cos(Time.time * shakeSpeed + i) * shakeIntensity,
                        0
                    );
                    break;
                    
                case EffectType.Wave:
                    float waveOffset = Mathf.Sin(Time.time * waveSpeed + i * 0.5f) * waveIntensity;
                    offset = new Vector3(0, waveOffset, 0);
                    break;
            }
            
            // 应用偏移量
            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }
        
        // 更新网格
        textComponent.UpdateVertexData();
    }

    /// <summary>
    /// 设置打字效果参数
    /// </summary>
    public void SetTypingParameters(float speed, float volume, int frequency)
    {
        defaultTypeSpeed = speed;
        typingSoundVolume = volume;
        typingSoundFrequency = frequency;
    }

    /// <summary>
    /// 设置颤抖效果参数
    /// </summary>
    public void SetShakeParameters(float intensity, float speed)
    {
        shakeIntensity = intensity;
        shakeSpeed = speed;
    }

    /// <summary>
    /// 设置波浪效果参数
    /// </summary>
    public void SetWaveParameters(float intensity, float speed)
    {
        waveIntensity = intensity;
        waveSpeed = speed;
    }

    /// <summary>
    /// 设置音效
    /// </summary>
    public void SetAudioSource(AudioSource source)
    {
        typingSoundSource = source;
    }

    /// <summary>
    /// 添加打字音效
    /// </summary>
    public void AddTypingSound(AudioClip sound)
    {
        if (typingSounds == null)
            typingSounds = new List<AudioClip>();
            
        if (sound != null && !typingSounds.Contains(sound))
            typingSounds.Add(sound);
    }

    /// <summary>
    /// 清除所有打字音效
    /// </summary>
    public void ClearTypingSounds()
    {
        if (typingSounds != null)
            typingSounds.Clear();
    }

    /// <summary>
    /// 文本效果数据结构
    /// </summary>
    public struct TextEffect
    {
        public EffectType type;
        public float value;
    }
}