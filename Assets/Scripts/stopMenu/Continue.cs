using UnityEngine;
using UnityEngine.UI;
public class Continue : MonoBehaviour
{
    private Button _button;
    [SerializeField] private GameObject stopMenuPanel;
    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            Debug.Log("绑定了");
            _button.onClick.AddListener(Continue_Game);
        }
    }

    private void Continue_Game()
    {

        Time.timeScale = 1f;
        stopMenuPanel.SetActive(false);

    }
}
