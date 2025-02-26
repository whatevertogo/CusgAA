using UnityEngine;

public class SetGuanQia : StopMenuButton
{
    [SerializeField] private string sceneName;
    protected override void OnButtonClicked()
    {
        //base.OnButtonClicked();可以调用基类的方法
        Set_Guan_Qia();
    }

    private void Set_Guan_Qia()
    {
        Time.timeScale = 1f;
        stopMenuPanel.SetActive(false);
        
       
        
        Managers.MySceneManager.Instance.LoadSceneByName(sceneName);
        //后面我们还是最好单独搞个选择关卡的关卡，当然也可以重构
    }



}
