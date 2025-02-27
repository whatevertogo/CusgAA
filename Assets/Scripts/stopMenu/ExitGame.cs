using UnityEngine;
using UnityEngine.UI;
public class ExitGame : MonoBehaviour
{
    private Button _button;
   
    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            Debug.Log("绑定了");
            _button.onClick.AddListener(Exit_Game);
        }
    }

    private void Exit_Game()
    {
        Debug.Log("ExitGame");
        Application.Quit(); // 退出游戏

        // 如果是在编辑器中运行，则停止播放模式
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


}