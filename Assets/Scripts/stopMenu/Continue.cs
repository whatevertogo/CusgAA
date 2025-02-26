using UnityEngine;

public class Continue : StopMenuButton
{
    protected override void OnButtonClicked()
    {
        base.OnButtonClicked();
        Continue_Game();
    }

    private void Continue_Game()
    {
        
        Time.timeScale = 1f;
        stopMenuPanel.SetActive(false);
        
    }
}
