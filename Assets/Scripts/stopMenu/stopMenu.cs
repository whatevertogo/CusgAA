using UnityEngine;
using UnityEngine.UI;
public class stopMenu : MonoBehaviour
{
    [SerializeField] private Button goOn, reStart, set, exit;
    [SerializeField]private GameObject menu;
    private bool _key=true;
    

    private void Start()
    {                                                           
        AddListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&_key)
        {
            menu.SetActive(true);
            _key=false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape)&&!_key)
        {
            menu.SetActive(false);
            _key=true;
        }
    }

    private void HandleButtongoOn()
    {
        menu.SetActive(false);
        _key=true;
    }
    private void HandleButtonreStart(){/*重新开始游戏，但不知道流程，可能改为从某一节点开始重新开始或其他*/}
    
    private void HandleButtonSet(){/*设置，可能包含调节音量等功能*/}
    
    private void HandleButtonExit(){/*根据需求返回主菜单或者退出游戏*/}

    private void AddListeners()
    {                                   //lambda闭包
        goOn.onClick.AddListener(() => HandleButtongoOn());
        reStart.onClick.AddListener(() => HandleButtonreStart());
        set.onClick.AddListener((() => HandleButtonSet()));
        exit.onClick.AddListener(()=>HandleButtonExit());
    }
   
}
