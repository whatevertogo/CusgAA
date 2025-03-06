using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MySceneManager : Singleton<MySceneManager>
    {
        #region 事件
        public class OnLoadingProgressChangedEventArgs : EventArgs
        {
            public float Progress;
        }

        public class OnLoadingStartedEventArgs : EventArgs
        {
            public string SceneName;
        }

        public class OnLoadingCompletedEventArgs : EventArgs
        {
            public string SceneName;
            public float LoadTime;
        }

        public event EventHandler<OnLoadingProgressChangedEventArgs> OnLoadingProgressChanged;
        public event EventHandler<OnLoadingStartedEventArgs> OnLoadingStarted;
        public event EventHandler<OnLoadingCompletedEventArgs> OnLoadingCompleted;
        #endregion

        #region 场景加载配置
        [System.Serializable]
        public class LoadingConfig
        {
            public bool useLoadingScreen = true;              // 是否使用加载界面
            public bool showProgressBar = true;               // 是否显示进度条
            public float minimumLoadingTime = 0.5f;          // 最小加载时间
            public LoadSceneMode loadMode = LoadSceneMode.Single; // 加载模式
        }

        private bool isLoading = false;
        public bool IsLoading => isLoading;
        #endregion

        // 初始化场景管理器
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("场景管理器初始化完成");
        }

        private void OnEnable()
        {
            // 订阅场景加载事件
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            // 取消订阅场景加载事件
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        #region 场景加载方法

        /// <summary>
        /// 同步加载场景（不推荐在生产环境使用）
        /// </summary>
        public void LoadSceneByName(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            try
            {
                Debug.Log($"开始加载场景: {sceneName}");
                SceneManager.LoadScene(sceneName, mode);
            }
            catch (Exception e)
            {
                Debug.LogError($"加载场景失败: {sceneName}, 错误: {e.Message}");
            }
        }

        /// <summary>
        /// 异步加载场景（推荐使用）
        /// </summary>
        public void LoadSceneAsync(string sceneName, LoadingConfig config = null, Action onComplete = null)
        {
            if (isLoading)
            {
                Debug.LogWarning("场景正在加载中，请等待当前加载完成");
                return;
            }
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName, config, onComplete));
        }

        /// <summary>
        /// 异步加载场景的协程实现
        /// </summary>
        private IEnumerator LoadSceneAsyncCoroutine(string sceneName, LoadingConfig config, Action onComplete)
        {
            isLoading = true;
            float startTime = Time.time;
            OnLoadingStarted?.Invoke(this, new OnLoadingStartedEventArgs 
            { 
                SceneName = sceneName 
            });
            Debug.Log($"开始异步加载场景: {sceneName}");

            // 使用默认配置
            config ??= new LoadingConfig();

            // 显示加载界面
            if (config.useLoadingScreen)
            {
                // TODO: 显示加载UI
                Debug.Log("显示加载界面");
            }

            yield return new WaitForSeconds(0.1f); // 给UI一点时间来显示

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, config.loadMode);
            asyncOperation.allowSceneActivation = false; // 暂时不允许场景激活

            float progress = 0;

            while (!asyncOperation.isDone)
            {
                // 更新加载进度（0-1）
                progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                OnLoadingProgressChanged?.Invoke(this, new OnLoadingProgressChangedEventArgs 
                { 
                    Progress = progress 
                });

                if (config.showProgressBar)
                {
                    Debug.Log($"加载进度: {progress:P}");
                }

                // 当加载进度达到90%且满足最小加载时间时，允许场景激活
                if (asyncOperation.progress >= 0.9f && Time.time - startTime >= config.minimumLoadingTime)
                {
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            float loadTime = Time.time - startTime;
            isLoading = false;
            OnLoadingCompleted?.Invoke(this, new OnLoadingCompletedEventArgs 
            { 
                SceneName = sceneName,
                LoadTime = loadTime
            });
            onComplete?.Invoke();
            Debug.Log($"场景 {sceneName} 加载完成，耗时: {loadTime:F2}秒");
        }

        /// <summary>
        /// 重新加载当前场景
        /// </summary>
        public void ReloadCurrentScene(LoadingConfig config = null, Action onComplete = null)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            LoadSceneAsync(currentScene.name, config, onComplete);
        }

        #endregion

        #region 场景事件处理

        /// <summary>
        /// 场景加载完成事件处理
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"场景 {scene.name} 加载完成，加载模式: {mode}");
        }

        /// <summary>
        /// 场景卸载完成事件处理
        /// </summary>
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"场景 {scene.name} 已卸载");
        }

        #endregion

        #region 场景预加载（TODO：实现场景预加载功能）
        
        // public void PreloadScene(string sceneName)
        // {
        //     // TODO: 实现场景预加载逻辑
        // }

        #endregion


    }
}
