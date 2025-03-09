using UnityEngine;
using UnityEngine.EventSystems;

public class DotPainting : MonoBehaviour, IPointerClickHandler
{
    private CustomCursorChanger cursorChanger; // 自定义光标更改器引用

    [System.Obsolete]
    private void Awake()
    {
        // 获取自定义光标更改器引用
        cursorChanger = FindObjectOfType<CustomCursorChanger>();

        // 检查是否成功获取引用
        if (cursorChanger != null)
        {
            Debug.Log("成功获取 CustomCursorChanger 引用");
        }
        else
        {
            Debug.LogError("未找到 CustomCursorChanger 脚本实例");
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cursorChanger.hasSelectedBrush)
        {
            // 获取点击位置
            Vector2 clickPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                cursorChanger.drawingArea.rectTransform,
                eventData.position,
                cursorChanger.drawingArea.canvas.worldCamera,
                out clickPosition
            );

            // 调试日志，检查点击位置
            Debug.Log("Click Position: " + clickPosition);

            cursorChanger.DotPaint(clickPosition);
        }
        else
        {
            // 调试日志，检查是否选择了画笔
            Debug.Log("No brush selected");
        }
    }
}