using System;
using UnityEngine;

public class StopMenuPanel : MonoBehaviour
{

    [SerializeField] private GameObject panel;


    private void Update()
    {
        if (panel.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        panel.SetActive(true);
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        panel.SetActive(false);
        
    }
}