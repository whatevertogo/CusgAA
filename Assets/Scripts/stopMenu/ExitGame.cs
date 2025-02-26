using UnityEngine;

public class ExitGame : StopMenuButton
{
    protected override void OnButtonClicked()
    {
        Debug.Log("ExitGame");
        //base.OnButtonClicked();可以调用基类的方法
        Exit_Game();
    }

    private void Exit_Game()
    {
        Application.Quit(); // 退出游戏

        // 如果是在编辑器中运行，则停止播放模式
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


}
