using UnityEngine;
using UnityEngine.UI;
using Managers;

public class StopMenu : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private Button resumeButton;    // 继续游戏按钮
    [SerializeField] private Button resetButton;     // 重置场景按钮
    [SerializeField] private Button levelSelectButton; // 选择关卡按钮
    [SerializeField] private Button quitButton;      // 退出游戏按钮
    
    [Header("设置")]
    [SerializeField] private string levelSelectSceneName = "LevelSelect"; // 关卡选择场景的名称
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;          // 暂停键
    
    private Canvas menuCanvas;
    private bool isPaused = false;

    private void Awake()
    {
        menuCanvas = GetComponent<Canvas>();
        
        // 确保获取到Canvas组件
        if (menuCanvas == null)
        {
            Debug.LogError("StopMenu: 未找到Canvas组件！");
            return;
        }
        
        // 初始化时隐藏菜单
        menuCanvas.enabled = false;
    }

    private void Start()
    {
        // 注册按钮事件
        if (resumeButton) resumeButton.onClick.AddListener(ResumeGame);
        if (resetButton) resetButton.onClick.AddListener(ResetScene);
        if (levelSelectButton) levelSelectButton.onClick.AddListener(GoToLevelSelect);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        // 检测暂停键
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (menuCanvas.gameObject.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // 暂停游戏时间
        menuCanvas.gameObject.SetActive(true);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        menuCanvas.gameObject.SetActive(false);
    }

    private void ResetScene()
    {
        // 恢复时间缩放
        Time.timeScale = 1f;
        menuCanvas.gameObject.SetActive(false);
        
        // 使用场景管理器重置当前场景
        MySceneManager.Instance.QuickReset((sender, args) =>
        {
            Debug.Log($"场景重置完成，耗时：{args.LoadTime}秒");
        });
    }

    private void GoToLevelSelect()
    {
        // 恢复时间缩放
        Time.timeScale = 1f;
        menuCanvas.gameObject.SetActive(false);
        
        // 使用场景管理器加载关卡选择场景
        MySceneManager.Instance.LoadSceneAsync(levelSelectSceneName, new MySceneManager.LoadingConfig
        {
            useLoadingScreen = true,
            showProgressBar = true,
            minimumLoadingTime = 0.5f
        });
    }

    private void QuitGame()
    {
        // 在编辑器中停止播放
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        // 清理按钮事件
        if (resumeButton) resumeButton.onClick.RemoveListener(ResumeGame);
        if (resetButton) resetButton.onClick.RemoveListener(ResetScene);
        if (levelSelectButton) levelSelectButton.onClick.RemoveListener(GoToLevelSelect);
        if (quitButton) quitButton.onClick.RemoveListener(QuitGame);
        
        // 确保退出时恢复时间缩放
        Time.timeScale = 1f;
    }
}
