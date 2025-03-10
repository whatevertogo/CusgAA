using UnityEngine;
using UnityEngine.UI;

public class ClosedThePanel : MonoBehaviour
{
    private Button _button;
    [SerializeField] private GameObject _closedPanel;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        _closedPanel.SetActive(false);
    }
}
