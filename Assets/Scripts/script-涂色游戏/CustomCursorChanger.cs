using UnityEngine;
using UnityEngine.UI;

public class CustomCursorChanger : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture; // 设置图片
    private Button _button; // 画笔按钮
    protected int _name;//应该要优化，变量不应该到处飞

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        // 为按钮的点击事件添加监听器
        _button.onClick.AddListener(SetCursorTexture);
    }

    private void SetCursorTexture()
    {
        // 设置自定义光标
        Vector2 hotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        if (_button.name == "ToggleButton")
        {
            _name = 1;
        }
    }

    public void ResetCursorTexture()
    {
        //TODO-当关闭涂色模式时候调用（或者再次点击笔时也可以调用）
        Cursor.SetCursor(null,Vector2.zero, CursorMode.Auto);
    }

    #region MyRegion

    


    /*   private Color GetBrushColor()
{
    // 获取按钮的标签
    string buttonTag = _button.gameObject.tag;

        // 根据按钮标签设置不同的颜色
        switch (buttonTag)
    {
        case "RedBrush":
            return Color.red; // 红色
        case "BlueBrush":
            return Color.blue; // 蓝色
        case "GreenBrush":
            return Color.green; // 绿色
        case "YellowBrush":
            return Color.yellow; // 黄色
        case "BlackBrush":
            return Color.black; // 黑色
        case "WhiteBrush":
            return Color.white; // 白色
        default:
            return Color.black; // 默认黑色
    }
}
    public void DotPaint(Vector2 position)
    {
        if (!hasSelectedBrush) return;

        // 创建一个点对象
        GameObject dotObj = new GameObject("Dot");
        dotObj.transform.SetParent(drawingArea.transform);
        // 添加Image组件并设置颜色和大小
        Image dotImage = dotObj.AddComponent<Image>();
        dotImage.color = currentColor;
        dotImage.rectTransform.sizeDelta = new Vector2(30f, 30f); // 点的大小
        dotImage.rectTransform.anchoredPosition = position;
    }
*/
    #endregion
}