using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class StopMenuButton : MonoBehaviour
{
     private Button _button;
    [SerializeField] protected GameObject stopMenuPanel;
    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            Debug.Log("绑定了");
            _button.onClick.AddListener(OnButtonClicked);
        }
    }

    protected virtual void OnButtonClicked()
    {
        Debug.Log("Button clicked But in baseButton");
    }
    
}
