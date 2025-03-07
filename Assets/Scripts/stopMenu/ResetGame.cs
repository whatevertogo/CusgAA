using UnityEngine;
using UnityEngine.UI;
public class ResetGame : MonoBehaviour
{
    private Button _button;
    [SerializeField] private string sceneName;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            Debug.Log("绑定了");
            _button.onClick.AddListener(Reset_Game);
        }
    }

    private void Reset_Game()//没办法了不能和类名相同，纠结了类名好久要逝去了捏
    {
        Debug.Log("ResetGame");
        Time.timeScale = 1f;
        Managers.MySceneManager.Instance.QuickReset((Sender, args) => { Debug.Log($"场景 {args.SceneName} ,耗时:{args.LoadTime}秒"); });


    }
}