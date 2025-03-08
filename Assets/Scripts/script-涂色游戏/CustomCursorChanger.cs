using UnityEngine;
using UnityEngine.UI;

public class CustomCursorChanger : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture; // 设置图片
     private Button _button;//看看能不能和背包里面的物品结合使用

     private void Awake()
     {
         _button=GetComponent<Button>();
     }

     private void Start()
    {
        // 为按钮的点击事件添加监听器
        _button.onClick.AddListener(ChangeCursor);
    }

    private void ChangeCursor()
    {
        // TODO-if在进入绘制模式=true时进行光标更改
        Vector2 hotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    
    public void ResetCursor()
    {        // TODO-如果需要在其他地方恢复默认光标(在退出绘制模式以后)，可以调用这个方法
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}