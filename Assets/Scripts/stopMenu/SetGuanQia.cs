using UnityEngine;
using UnityEngine.UI;

public class SetGuanQia : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        Managers.MySceneManager.Instance.LoadSceneByName(sceneName);
    }
}
