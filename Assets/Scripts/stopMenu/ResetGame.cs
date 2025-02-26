using UnityEngine;

public class ResetGame : StopMenuButton
{
    [SerializeField] private string sceneName;
    protected override void OnButtonClicked()
    {
        Debug.Log("ResetGame");
        //base.OnButtonClicked();可以调用基类的方法
        Reset_Game();
    }

    private void Reset_Game()//没办法了不能和类名相同，纠结了类名好久要逝去了捏
    {
        Time.timeScale = 1f;
        //暂时的
        sceneName = "First";
        Managers.MySceneManager.Instance.LoadSceneByName(sceneName);
        //todo-后期在mysceneManager里面写个重进场景的方法，现在懒得写，要不你写~(￣▽￣)~*
        //自己写sceneName相当于重新进入场景
        
    }
}
