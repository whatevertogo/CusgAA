using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

// 背包UI管理器
// 说明：管理背包界面的显示、隐藏和更新
// 功能：
// 1. 显示和隐藏背包界面
// 2. 动态更新背包物品显示
// 3. 处理背包相关的输入事件
public class ItemsManagerUI : MonoBehaviour
{
    [SerializeField] private Transform AllItems;  // 所有物品的容器
    [SerializeField] private GameObject itemContainerPrefab;  // 物品容器预制体
    [SerializeField] private GameObject InventoryBackGround;
    bool _done = true;
    
    [Header("动画")]
    private Coroutine _currentCoroutine;
    [SerializeField] private RectMask2D itemMask;
    [SerializeField] private float targetBottom = 320f;
    [SerializeField] private float duration = 1f;
    
    private Tweener currentTweener;

    
   
    

    // 初始化背包UI系统
    // 说明：
    // 1. 初始化背包界面显示
    // 2. 订阅背包更新事件
    // 3. 订阅背包开关事件
    // 4. 初始设置物品容器为隐藏状态
    void Start()
    {
        EventManager.Instance.OnInventoryUpdated += (sender, args) => UpdateVisual(); // 监听背包更新事件
        GameInput.Instance.OnOpenInventoryAction += InventoryManager_OnOpenInventoryAction; // 监听背包开关事件
        AllItems.gameObject.SetActive(false); // 初始设置所有物品容器为隐藏状态
        InventoryBackGround.SetActive(false); // 初始设置背包背景为隐藏状态
        UpdateVisual();
    }

    // 处理背包开关事件
    // 参数：
    // - sender: 事件发送者
    // - e: 事件参数
    // 说明：响应输入系统的背包开关命令
    //GameInput的事件我没放在EventManager里面
    private void InventoryManager_OnOpenInventoryAction(object sender, EventArgs e)
    {
        Open_CloseInventory();
    }

    // 切换背包显示状态
    // 说明：根据当前状态打开或关闭背包界面
    public void Open_CloseInventory()
    {
        if (AllItems.gameObject.activeSelf)
        {
            HideInventory();
        }
        else
        {
            ShowInventory();
        }
    }

    // 显示背包界面
    // 说明：
    // 1. 激活背包界面
    // 2. 更新物品显示
    public void ShowInventory()
    {
        DoKill();
        AllItems.gameObject.SetActive(true);
        InventoryBackGround.SetActive(true); // 显示背包背景
        UpdateVisual();
        PlayMaskAnimation(targetBottom, 0f);
    }

    // 隐藏背包界面
    // 说明：禁用背包界面的显示
    public void HideInventory()
    {
        DoKill();
        PlayMaskAnimation(0f, targetBottom).OnComplete(() => {
            AllItems.gameObject.SetActive(false);
            InventoryBackGround.SetActive(false); // 隐藏背包背景
        });
    }

    // 更新背包界面显示
    // 说明：
    // 1. 清理现有的物品显示
    // 2. 为每个背包中的物品创建显示容器
    // 3. 设置物品图片和名称
    // 4. 可选：添加物品使用按钮
    public void UpdateVisual()
    {
        // 清空当前的UI元素
        foreach (Transform child in AllItems)
        {
            Destroy(child.gameObject);
        }

        // 载入所有背包物品
        foreach (var item in InventoryManager.Instance.items)
        {
            // 创建物品容器
            GameObject newItemContainer = Instantiate(itemContainerPrefab, AllItems);

            // 设置物品图片
            Image itemImage = newItemContainer.transform.Find("Image").GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = item.itemImage;
            }

            // 设置物品名称
            Text itemNameText = newItemContainer.transform.Find("Text").GetComponent<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName;
            }
            
            #region 可选按钮
            // 为物品添加使用功能（启用）
             Button itemButton = newItemContainer.transform.Find("Button")?.GetComponent<Button>();
             if (itemButton != null)
             {
                 itemButton.onClick.AddListener(() => UseItem(item));
             }
            #endregion
        }
        
        
    }
    //TODO-写一个更好的视觉效果
    
    private Tweener PlayMaskAnimation(float from, float to)
    {
        itemMask.padding = new Vector4(0, 0, 0, from);
        return DOTween.To(() => itemMask.padding, x => itemMask.padding = x, new Vector4(0, 0, 0, to), duration);
    }
    
    private void DoKill()
    {
        if (currentTweener != null && currentTweener.IsActive())
        {
            currentTweener.Kill();
        }
    }
    private void UseItem(ItemSO item)
    {

        #region 操作光标
         // 先取反
        if (_done)
        {
            SetCursorTexture(item);
            _done = !_done;
        }
        else
        {
            ResetCursorTexture();
            _done = !_done;
        }
        #endregion
    }

    #region 光标设置
    private void SetCursorTexture(ItemSO item)
    {
        Texture2D texture = item.itemImage.texture;//转换类型
        
        // 设置自定义光标
        Vector2 hotspot = new Vector2(texture.width /2f,texture.height / 2f);
        Cursor.SetCursor(texture, hotspot, CursorMode.ForceSoftware);
       
    }
    
    private void ResetCursorTexture()
    {
        //TODO-（或者再次点击时调用）
        Cursor.SetCursor(null,Vector2.zero, CursorMode.Auto);
    }
    #endregion
}
