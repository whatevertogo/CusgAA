using UnityEngine;
using UnityEngine.EventSystems;

public class CustomCursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D cursorTexture; // ��Inspector��ָ��ÿ�������Ӧ�Ĺ������

    public void OnPointerEnter(PointerEventData eventData)
    {
        // �������뷽��ʱ�������Զ�����
        Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ������뿪����ʱ���ָ�Ĭ�Ϲ��
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}