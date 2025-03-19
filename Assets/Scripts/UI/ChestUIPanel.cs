using System;
using UnityEngine;
using UnityEngine.UI;

public class ChestUIPanel : MonoBehaviour
{
    [SerializeField] private PasswordChest PasswordChest;
    [SerializeField] private Button button;
    private void Start(){
        button.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
        Debug.Log("关闭密码箱UI面板");
    }



}
