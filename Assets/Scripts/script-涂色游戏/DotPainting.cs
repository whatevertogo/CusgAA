using UnityEngine;
using UnityEngine.EventSystems;

public class DotPainting : MonoBehaviour, IPointerClickHandler
{
    private CustomCursorChanger cursorChanger; // �Զ��������������

    [System.Obsolete]
    private void Awake()
    {
        // ��ȡ�Զ��������������
        cursorChanger = FindObjectOfType<CustomCursorChanger>();

        // ����Ƿ�ɹ���ȡ����
        if (cursorChanger != null)
        {
            Debug.Log("�ɹ���ȡ CustomCursorChanger ����");
        }
        else
        {
            Debug.LogError("δ�ҵ� CustomCursorChanger �ű�ʵ��");
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cursorChanger.hasSelectedBrush)
        {
            // ��ȡ���λ��
            Vector2 clickPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                cursorChanger.drawingArea.rectTransform,
                eventData.position,
                cursorChanger.drawingArea.canvas.worldCamera,
                out clickPosition
            );

            // ������־�������λ��
            Debug.Log("Click Position: " + clickPosition);

            cursorChanger.DotPaint(clickPosition);
        }
        else
        {
            // ������־������Ƿ�ѡ���˻���
            Debug.Log("No brush selected");
        }
    }
}