using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScriptableObjects
{
    /// <summary>
    /// 场景加载配置的ScriptableObject
    /// 方便在编辑器中配置和重用
    /// </summary>
    [CreateAssetMenu(fileName = "NewSceneLoadingConfig", menuName = "游戏配置/场景加载配置", order = 1)]
    public class SceneLoadingConfig : ScriptableObject
    {
        [Header("界面设置")]
        [Tooltip("是否使用加载界面")]
        public bool useLoadingScreen = true;
        
        [Tooltip("是否显示进度条")]
        public bool showProgressBar = true;
        
        [Header("加载设置")]
        [Tooltip("最小加载时间（秒）")]
        [Range(0f, 5f)]
        public float minimumLoadingTime = 0.5f;
        
        [Tooltip("场景加载模式")]
        public LoadSceneMode loadMode = LoadSceneMode.Single;
        
        [Header("过渡设置")]
        [Tooltip("淡入淡出时间（秒）")]
        [Range(0f, 2f)]
        public float fadeTime = 0.5f;
        
        [Tooltip("加载屏幕颜色")]
        public Color loadingScreenColor = Color.black;
        
        [Header("重置设置")]
        [Tooltip("是否重置玩家位置")]
        public bool resetPlayerPosition = true;
        
        [Tooltip("是否重置游戏状态")]
        public bool resetGameState = true;
    }
}
