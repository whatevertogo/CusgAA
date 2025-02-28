using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextEffectsController : MonoBehaviour
{
    [SerializeField] private DialogueTextEffects textEffects;
    
    [Header("打字效果控制")]
    [SerializeField] private Slider typeSpeedSlider;
    [SerializeField] private Slider volumeSlider;
    
    [Header("颤抖效果控制")]
    [SerializeField] private Slider shakeIntensitySlider;
    [SerializeField] private Slider shakeSpeedSlider;
    
    [Header("波浪效果控制")]
    [SerializeField] private Slider waveIntensitySlider;
    [SerializeField] private Slider waveSpeedSlider;
    
    [Header("测试")]
    [SerializeField] private Button testButton;
    [TextArea(3, 5)]
    [SerializeField] private string testText = "普通文字<effect=shake>颤抖文字</effect>正常<effect=wave>波浪效果</effect>";

    private void Start()
    {
        if (textEffects == null)
        {
            Debug.LogError("请设置TextEffects引用!");
            return;
        }

        // 设置滑块初始值
        if (typeSpeedSlider != null)
        {
            typeSpeedSlider.value = textEffects.DefaultTypeSpeed;
            typeSpeedSlider.onValueChanged.AddListener(val => textEffects.DefaultTypeSpeed = val);
        }
        
        if (volumeSlider != null)
        {
            volumeSlider.value = textEffects.TypingSoundVolume;
            volumeSlider.onValueChanged.AddListener(val => textEffects.TypingSoundVolume = val);
        }
        
        if (shakeIntensitySlider != null)
        {
            shakeIntensitySlider.value = textEffects.ShakeIntensity;
            shakeIntensitySlider.onValueChanged.AddListener(val => textEffects.ShakeIntensity = val);
        }
        
        if (shakeSpeedSlider != null)
        {
            shakeSpeedSlider.value = textEffects.ShakeSpeed;
            shakeSpeedSlider.onValueChanged.AddListener(val => textEffects.ShakeSpeed = val);
        }
        
        if (waveIntensitySlider != null)
        {
            waveIntensitySlider.value = textEffects.WaveIntensity;
            waveIntensitySlider.onValueChanged.AddListener(val => textEffects.WaveIntensity = val);
        }
        
        if (waveSpeedSlider != null)
        {
            waveSpeedSlider.value = textEffects.WaveSpeed;
            waveSpeedSlider.onValueChanged.AddListener(val => textEffects.WaveSpeed = val);
        }
        
        if (testButton != null)
        {
            testButton.onClick.AddListener(TestTextEffects);
        }
    }
    
    public void TestTextEffects()
    {
        if (textEffects != null)
        {
            textEffects.StartTyping(testText);
        }
    }
}