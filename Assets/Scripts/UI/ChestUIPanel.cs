using System;
using UnityEngine;
using UnityEngine.UI;

public class ChestUIPanel : MonoBehaviour
{
    [SerializeField] private PasswordChest PasswordChest;
    [SerializeField] private Button button;
    private void Start(){
        PasswordChest.PasswordChestUI_Open += OpenPanel;
        button.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
        Debug.Log("关闭密码箱UI面板");
    }

    private void OpenPanel(object sender, EventArgs e)
    {
        //TODO-打开密码箱UI面板
        gameObject.SetActive(true);
        Debug.Log("打开密码箱UI面板");
    }



}
