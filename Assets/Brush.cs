using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class Brush : MonoBehaviour,IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler//��IPointerDownHandler��IBeginDragHandler��IDragHandler��IEndDragHandler�ӿڣ����ڴ�������¼���
{
    public Texture2D texture2D;
    public Texture2D copyTextrue2D;//���Ƶ��������ڻ�ͼ
    public RawImage image;//��ʾ����� RawImage ���
    public Color[] colors;
    public int brushSize = 10;
    public Color brushColor= Color.red;
    private int width;
    private int height;
    private List<int> colorArea=new List<int>();//�洢��͸�����ص������б�
    void Start()
    {
        colors=texture2D.GetPixels();
        width=texture2D.width;
        height=texture2D.height;
        copyTextrue2D=new Texture2D(width,height);
        copyTextrue2D.SetPixels(colors);
        copyTextrue2D.Apply();
        //��������
        for (int i = 0; i < height; i++)
        {
            for(int j= 0; j < width; j++)
            {
                if (colors[i * width + j].a != 0)
                {
                    colorArea.Add(i * width + j);//������ز�͸����alpha ��Ϊ 0��������������ӵ� colorArea �б���
                }
                else
                {
                    colors[i * width +j] = Color.green;//�������͸����������ɫ����Ϊ��ɫ
                }
            }
        }
        //���޸ĺ����������Ӧ�õ� copyTextrue2D ������ RawImage ������
        copyTextrue2D.SetPixels(colors);
        copyTextrue2D.Apply ();
        image.texture = copyTextrue2D;

    }


    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Brushcolor(eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Brushcolor(eventData.position);
    }
    private void Brushcolor(Vector2 pos)
    {
        Vector2 localPos;
        //ת������
        RectTransformUtility.ScreenPointToLocalPointInRectangle(image.GetComponent<RectTransform>(),pos, null, out localPos);
        if (GetComponent<RectTransform>().rect.Contains(localPos)) 
        {
            int x = (int)localPos.x;
            int y = (int)localPos.y;
            int index = y * width + x;
            for(int i = x-brushSize; i < x+brushSize; i++) 
            {
                for(int j = y-brushSize; j < y+brushSize; j++)
                {
                    if(Vector2.SqrMagnitude(new Vector2 (i,j)-new Vector2(x, y))< brushSize * brushSize)
                    {
                        index = j * width + i;
                        colors[index] = brushColor;
                        if (colorArea.Contains(index))
                        {
                            colorArea.Remove(index);
                            if (colorArea.Count < 7000)
                            {
                                Debug.LogError("��ɫ���");
                            }
                        }
                    }
                }
            }
            copyTextrue2D.SetPixels(colors);//��ɫ
            copyTextrue2D.Apply(); //���� copyTextrue2D ���������ݲ�Ӧ�ø���
            image.texture = copyTextrue2D;//���� RawImage ����������ʾ���µ�ͼ��
        }

    }

}
