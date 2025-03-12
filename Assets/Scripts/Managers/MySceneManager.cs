using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjects;

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
            public float LoadTime;
            public string SceneName;
        }

        public event EventHandler<OnLoadingProgressChangedEventArgs> OnLoadingProgressChanged;
        public event EventHandler<OnLoadingStartedEventArgs> OnLoadingStarted;
        public event EventHandler<OnLoadingCompletedEventArgs> OnLoadingCompleted;

        #endregion

        #region 场景加载配置

        [Serializable]
        public class LoadingConfig
        {
            public bool useLoadingScreen = true; // 是否使用加载界面
            public bool showProgressBar = true; // 是否显示进度条
            public float minimumLoadingTime = 0.5f; // 最小加载时间
            public LoadSceneMode loadMode = LoadSceneMode.Single; // 加载模式
        }

        [SerializeField] private SceneLoadingConfig defaultConfig;
        public bool IsLoading { get; private set; }

        #endregion

        #region 生命周期函数

        // 初始化场景管理器
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("场景管理器初始化完成");
            
            // 如果没有配置，创建默认配置
            if (defaultConfig == null)
            {
                Debug.LogWarning("未找到默认场景加载配置，使用内置默认值");
            }
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

        #endregion

        #region 场景加载方法

        /// <summary>
        ///     同步加载场景（不推荐在生产环境使用）
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
                HandleSceneLoadError(sceneName, e);
            }
        }

        /// <summary>
        ///     处理场景加载错误
        /// </summary>
        private void HandleSceneLoadError(string sceneName, Exception error)
        {
            Debug.LogError($"场景加载错误处理: {sceneName}, 错误类型: {error.GetType().Name}");
            
            // 检查是否是场景不存在的错误
            if (error.Message.Contains("not found") || error.Message.Contains("couldn't be loaded"))
            {
                Debug.LogError($"场景 '{sceneName}' 不存在，请检查场景名称和构建设置");
                // 可以加载默认的错误场景或者显示错误UI
            }
            else
            {
                Debug.LogError($"加载场景时发生未知错误: {error.Message}\n{error.StackTrace}");
            }
            
            IsLoading = false;
        }

        /// <summary>
        ///     场景加载完成回调的事件参数
        /// </summary>
        public class OnSceneLoadCompleteEventArgs : EventArgs
        {
            public string SceneName { get; set; }
            public float LoadTime { get; set; }
        }

        /// <summary>
        ///     异步加载场景（推荐使用）
        /// </summary>
        public void LoadSceneAsync(string sceneName, LoadingConfig config = null,
            EventHandler<OnSceneLoadCompleteEventArgs> onComplete = null)
        {
            // 错误检查：无效的场景名
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("场景名称不能为空");
                return;
            }
            
            // 已经在加载中，防止重复加载
            if (IsLoading)
            {
                Debug.LogWarning("场景正在加载中，请等待当前加载完成");
                return;
            }

            try
            {
                StartCoroutine(LoadSceneAsyncCoroutine(sceneName, config, onComplete));
            }
            catch (Exception e)
            {
                Debug.LogError($"启动场景加载协程失败: {e.Message}");
                HandleSceneLoadError(sceneName, e);
            }
        }

        /// <summary>
        ///     异步加载场景的协程实现
        /// </summary>
        private IEnumerator LoadSceneAsyncCoroutine(string sceneName, LoadingConfig config,
            EventHandler<OnSceneLoadCompleteEventArgs> onComplete)
        {
            IsLoading = true; // 标记正在加载中
            var startTime = Time.time; // 记录开始加载时间
            
            // 使用默认配置
            config ??= new LoadingConfig();
            
            // 触发加载开始事件
            TriggerLoadingStarted(sceneName);
            
            // 显示加载UI
            yield return ShowLoadingUI(config);
            
            // 执行场景加载
            yield return LoadSceneOperation(sceneName, config, startTime);
            
            // 完成加载过程
            FinishLoading(sceneName, startTime, onComplete);
        }
        
        /// <summary>
        /// 触发加载开始事件
        /// </summary>
        private void TriggerLoadingStarted(string sceneName)
        {
            Debug.Log($"开始异步加载场景: {sceneName}");
            OnLoadingStarted?.Invoke(this, new OnLoadingStartedEventArgs
            {
                SceneName = sceneName
            });
        }
        
        /// <summary>
        /// 显示加载界面
        /// </summary>
        private IEnumerator ShowLoadingUI(LoadingConfig config)
        {
            if (config.useLoadingScreen)
            {
                // 这里可以实现显示加载UI的逻辑
                // 例如：
                // loadingScreenUI.SetActive(true);
                // loadingScreenUI.GetComponent<LoadingUI>().ResetProgress();
            }
            
            // 给UI一点时间来显示
            yield return new WaitForSeconds(0.1f);
        }
        
        /// <summary>
        /// 执行场景加载操作
        /// </summary>
        private IEnumerator LoadSceneOperation(string sceneName, LoadingConfig config, float startTime)
        {
            AsyncOperation asyncOperation = null;
            
            try
            {
                asyncOperation = SceneManager.LoadSceneAsync(sceneName, config.loadMode);
                
                if (asyncOperation == null)
                {
                    Debug.LogError($"创建异步加载操作失败: {sceneName}");
                    yield break;
                }
                
                asyncOperation.allowSceneActivation = false; // 暂时不允许场景激活
            }
            catch (Exception e)
            {
                Debug.LogError($"场景异步加载过程中发生错误: {e.Message}");
                HandleSceneLoadError(sceneName, e);
                yield break;
            }
            
            
            yield return MonitorLoadingProgress(asyncOperation, config, startTime);
        }
        
        /// <summary>
        /// 监控加载进度
        /// </summary>
        private IEnumerator MonitorLoadingProgress(AsyncOperation asyncOperation, LoadingConfig config, float startTime)
        {
            float progress = 0;
            
            while (!asyncOperation.isDone)
            {
                // 更新加载进度（0-1）
                progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                
                // 触发进度更新事件
                OnLoadingProgressChanged?.Invoke(this, new OnLoadingProgressChangedEventArgs
                {
                    Progress = progress
                });
                
                if (config.showProgressBar)
                {
                    Debug.Log($"加载进度: {progress:P}");
                    // 这里可以更新UI进度条：
                    // loadingScreenUI.GetComponent<LoadingUI>().UpdateProgress(progress);
                }
                
                // 当加载进度达到90%且满足最小加载时间时，允许场景激活
                if (asyncOperation.progress >= 0.9f && Time.time - startTime >= config.minimumLoadingTime)
                {
                    asyncOperation.allowSceneActivation = true;
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// 完成加载过程
        /// </summary>
        private void FinishLoading(string sceneName, float startTime, EventHandler<OnSceneLoadCompleteEventArgs> onComplete)
        {
            var loadTime = Time.time - startTime; // 计算加载耗时
            IsLoading = false; // 标记加载完成
            
            // 隐藏加载UI
            // if (loadingScreenUI != null)
            // {
            //     loadingScreenUI.SetActive(false);
            // }
            
            // 触发通用的加载完成事件
            OnLoadingCompleted?.Invoke(this, new OnLoadingCompletedEventArgs
            {
                SceneName = sceneName,
                LoadTime = loadTime
            });
            
            // 触发一次性的完成回调
            onComplete?.Invoke(this, new OnSceneLoadCompleteEventArgs
            {
                SceneName = sceneName,
                LoadTime = loadTime
            });
            
            Debug.Log($"场景 {sceneName} 加载完成，耗时: {loadTime:F2}秒");
        }

        /// <summary>
        ///     重新加载当前场景
        /// </summary>
        public void ReloadCurrentScene(LoadingConfig config = null,
            EventHandler<OnSceneLoadCompleteEventArgs> onComplete = null)
        {
            var currentScene = SceneManager.GetActiveScene();
            LoadSceneAsync(currentScene.name, config, onComplete);
        }

        #endregion

        #region 场景事件处理

        /// <summary>
        ///     场景加载完成事件处理
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"场景 {scene.name} 加载完成，加载模式: {mode}");
        }

        /// <summary>
        ///     场景卸载完成事件处理
        /// </summary>
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"场景 {scene.name} 已卸载");
        }

        #endregion

        #region 场景预加载

        // 预留用于实现场景预加载功能
        public void PreloadScene(string sceneName)
        {
            // 未来实现场景预加载逻辑
            Debug.Log($"场景预加载功能尚未实现: {sceneName}");
        }

        #endregion

        #region 场景重置

        /// <summary>
        ///     重置场景配置
        /// </summary>
        [Serializable]
        public class ResetConfig
        {
            public bool resetPlayerPosition = true; // 是否重置玩家位置
            public bool resetGameState = true; // 是否重置游戏状态
            public bool showTransition = true; // 是否显示过渡动画
            public float transitionTime = 0.5f; // 过渡动画时间
        }

        /// <summary>
        ///     重置当前场景
        /// </summary>
        public void ResetCurrentScene(ResetConfig resetConfig = null,
            EventHandler<OnSceneLoadCompleteEventArgs> onComplete = null)
        {
            // 使用默认配置
            resetConfig ??= new ResetConfig();

            var loadConfig = new LoadingConfig
            {
                useLoadingScreen = resetConfig.showTransition,
                showProgressBar = false,
                minimumLoadingTime = resetConfig.transitionTime,
                loadMode = LoadSceneMode.Single
            };

            // 如果需要重置游戏状态
            if (resetConfig.resetGameState)
            {
                ResetGameState();
            }

            var currentScene = SceneManager.GetActiveScene();
            LoadSceneAsync(currentScene.name, loadConfig, (sender, args) =>
            {
                // 如果需要重置玩家位置
                if (resetConfig.resetPlayerPosition)
                {
                    ResetPlayerPosition();
                }
                
                // 调用完成回调
                onComplete?.Invoke(sender, args);
            });

            Debug.Log("开始重置场景");
        }
        
        /// <summary>
        /// 重置游戏状态
        /// </summary>
        private void ResetGameState()
        {
            Debug.Log("重置游戏状态");
            // 这里可以调用GameManager重置游戏状态
            // 例如：GameManager.Instance.ResetGameState();
        }
        
        /// <summary>
        /// 重置玩家位置
        /// </summary>
        private void ResetPlayerPosition()
        {
            Debug.Log("重置玩家位置");
            // 可以通过GameManager或其他方式获取玩家并重置位置
            // 例如：
            // if (GameManager.Instance.Player != null)
            // {
            //     GameManager.Instance.Player.transform.position = Vector3.zero;
            // }
        }

        /// <summary>
        ///     快速重置场景（使用默认配置）
        /// </summary>
        public void QuickReset(EventHandler<OnSceneLoadCompleteEventArgs> onComplete = null)
        {
            var config = new ResetConfig
            {
                resetPlayerPosition = true,
                resetGameState = true,
                showTransition = false,
                transitionTime = 0f
            };
            ResetCurrentScene(config, onComplete);
        }

        #endregion
    }
}