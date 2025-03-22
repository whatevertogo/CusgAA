using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class Brush : MonoBehaviour,IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler//用IPointerDownHandler、IBeginDragHandler、IDragHandler、IEndDragHandler接口，用于处理鼠标事件。
{
    public Texture2D texture2D;
    public Texture2D copyTextrue2D;//复制的纹理，用于绘图
    public RawImage image;//显示纹理的 RawImage 组件
    public Color[] colors;
    public int brushSize = 10;
    public Color brushColor= Color.red;
    private int width;
    private int height;
    private List<int> colorArea=new List<int>();//存储非透明像素的索引列表
    void Start()
    {
        colors=texture2D.GetPixels();
        width=texture2D.width;
        height=texture2D.height;
        copyTextrue2D=new Texture2D(width,height);
        copyTextrue2D.SetPixels(colors);
        copyTextrue2D.Apply();
        //遍历像素
        for (int i = 0; i < height; i++)
        {
            for(int j= 0; j < width; j++)
            {
                if (colors[i * width + j].a != 0)
                {
                    colorArea.Add(i * width + j);//如果像素不透明（alpha 不为 0），将其索引添加到 colorArea 列表中
                }
                else
                {
                    colors[i * width +j] = Color.green;//如果像素透明，将其颜色设置为绿色
                }
            }
        }
        //将修改后的像素数组应用到 copyTextrue2D 并更新 RawImage 的纹理
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
        //转换坐标
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
                                Debug.LogError("上色完成");
                            }
                        }
                    }
                }
            }
            copyTextrue2D.SetPixels(colors);//上色
            copyTextrue2D.Apply(); //更新 copyTextrue2D 的像素数据并应用更改
            image.texture = copyTextrue2D;//更新 RawImage 的纹理以显示最新的图像
        }

    }

}
