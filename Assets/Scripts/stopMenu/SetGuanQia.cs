using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SetGuanQia : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private Button _button;
    [SerializeField] private GameObject stopMenuPanel;
    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            Debug.Log("绑定了");
            _button.onClick.AddListener(Set_Guan_Qia);
        }
    }
    private void Set_Guan_Qia()
    {
        Time.timeScale = 1f;
        stopMenuPanel.SetActive(false);
        
       
        sceneName = SceneManager.GetActiveScene().name;
        Managers.MySceneManager.Instance.LoadSceneByName(sceneName);
        //后面我们还是最好单独搞个选择关卡的关卡，当然也可以重构
    }



}
