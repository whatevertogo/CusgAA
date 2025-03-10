using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
///暂停面板
/// 继续，退出，重置，选择关卡
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button exitButton; 
    [SerializeField] private Button setLevelButton;
    
    [Header("Settings")]
    [SerializeField] private string sceneName;

    private void Awake()
    {
        //绑定按钮
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
            Debug.Log("绑定了");
        }
        
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetGame);
            Debug.Log("绑定了");
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
            Debug.Log("绑定了");
        }
        
        if (setLevelButton != null)
        {
            setLevelButton.onClick.AddListener(SetLevel);
            Debug.Log("绑定了");
        }
    }
    
    
    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        GameInput.Instance.OnEscapeAction+=PausedMenuOpen_Close;
    }

    private void PausedMenuOpen_Close(object sender, EventArgs e)
    {
        if (pauseMenuPanel.activeSelf)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
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
        MySceneManager.Instance.QuickReset((Sender, args) => { 
            Debug.Log($"场景 {args.SceneName} ,耗时:{args.LoadTime}秒"); 
        });
    }

    private void SetLevel()
    {
        if (sceneName == null)
        {
            sceneName = "First";
            Debug.LogError("找不到场景名字，已经设置为默认场景名字");
        }

        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        
        sceneName = SceneManager.GetActiveScene().name;
        MySceneManager.Instance.LoadSceneByName(sceneName);
    }

    private void ExitGame()
    {
        Debug.Log("ExitGame");
        Application.Quit();

        // If running in editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}