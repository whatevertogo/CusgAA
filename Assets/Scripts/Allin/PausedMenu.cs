using System;
using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///     暂停面板
///     继续，退出，重置，选择关卡
/// </summary>
public class PausedMenu : MonoBehaviour
{
    [Header("UI")] [SerializeField] private GameObject pauseMenuPanel;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button reloadLevelButton;
    [SerializeField] private Button exitButton;

    [Header("Settings")] [SerializeField] private string sceneName;

    private void Awake()
    {
        //绑定按钮
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetGame);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }

        if (reloadLevelButton != null)
        {
            reloadLevelButton.onClick.AddListener(ReloadCurrentLevel);
        }
    }


    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        GameInput.Instance.OnEscapeAction += PausedMenuOpen_Close;
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnEscapeAction -= PausedMenuOpen_Close;
        }
    }

    private void PausedMenuOpen_Close(object sender, EventArgs e)
    {
        if (pauseMenuPanel.activeSelf)
            ContinueGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0;
    }

    private void ContinueGame()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }

    private void ResetGame()
    {
        Debug.Log("ResetGame");
        Time.timeScale = 1f;
        MySceneManager.Instance.QuickReset(
            (Sender, args) => { Debug.Log($"场景 {args.SceneName} ,耗时:{args.LoadTime}秒"); });
    }

    private void ReloadCurrentLevel()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);

        // 获取当前场景名
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // 使用异步加载以获得更好的用户体验
        MySceneManager.Instance.LoadSceneAsync(currentSceneName, null, 
            (sender, args) => { Debug.Log($"场景 {args.SceneName} 重新加载完成，耗时:{args.LoadTime}秒"); });
    }

    private void ExitGame()
    {
        Debug.Log("ExitGame");
        Application.Quit();

        // If running in editor, stop play mode
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}