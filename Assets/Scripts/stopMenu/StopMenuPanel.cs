using System;
using UnityEngine;

public class StopMenuPanel : MonoBehaviour
{
    private bool _isOpen;
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        _isOpen = false;
    }

    private void Update()
    {
        MenuPanel();
    }

    private void MenuPanel()
    {
        // 检查是否按下了 ESC 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isOpen = !_isOpen;
            // 切换面板的激活状态
            panel.SetActive(_isOpen);
            
        }
    }
}