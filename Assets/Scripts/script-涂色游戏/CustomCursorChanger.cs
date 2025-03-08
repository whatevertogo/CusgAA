using UnityEngine;
using UnityEngine.EventSystems;

public class CustomCursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D cursorTexture; // 在Inspector中指定每个方块对应的光标纹理

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 当鼠标进入方块时，设置自定义光标
        Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 当鼠标离开方块时，恢复默认光标
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}