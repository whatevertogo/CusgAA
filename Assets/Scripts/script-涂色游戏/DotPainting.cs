using UnityEngine;
using UnityEngine.EventSystems;
//TODO-添加到要涂色的物品上
/*
public class HideImageOnClick : MonoBehaviour, IPointerClickHandler
{
    
    // 引用图片的GameObject
    public GameObject imageObject;
    [SerializeField] private CustomCursorChanger cursorChanger;

    void Start()
    {
        if (imageObject == null)
        {
            Debug.LogError("Image Object is not assigned.");
        }

        
    }

    void Update()
    {
        
    }
    // 实现IPointerClickHandler接口的OnPointerClick方法
    public void OnPointerClick(PointerEventData eventData)
    {
        
        
        if (imageObject != null)
        {
            // 隐藏图片
            switch (cursorChanger.name)
            {
                case 1 :
                 imageObject.SetActive(false);
                 break;//TODO-根据变量对应颜色板块变色（待写）
            }
            
        }
    }
    //TODO-当对应板块全部消失获得物品到背包
    */
    //TODO-这里写一个派生类吧