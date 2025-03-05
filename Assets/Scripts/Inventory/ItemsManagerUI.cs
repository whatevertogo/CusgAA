using System;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;

public class ItemsManagerUI : MonoBehaviour
{
    [SerializeField] private Transform AllItems;
    [SerializeField] private GameObject itemsContainerFirst;
    [SerializeField] private GameObject itemContainerPrefab;
    [Header("fade")]
    [SerializeField] private CanvasGroup uiPanel;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private RectTransform uiPanelRectTransform; // Add this line
    [SerializeField] private float startY = 0f; // Starting Y position for fade in
    [SerializeField] private float endY = 100f; // Ending Y position for fade out
    private bool _canFade;
    private bool _isAnimating;

    void Start()
    {
        UpdateVisual();
        GameInput.Instance.OnOpenInventoryAction += InventoryManager_OnOpenInventoryAction; //通过GameInput的事件来打开背包
        itemsContainerFirst.gameObject.SetActive(false);
        uiPanelRectTransform.anchoredPosition = new Vector2(uiPanelRectTransform.anchoredPosition.x, startY); // Set initial position
    }

    private void InventoryManager_OnOpenInventoryAction(object sender, EventArgs e) //通过GameInput的事件来打开背包
    {
        Open_CloseInventory();
    }

    public void Open_CloseInventory()
    {
        if (AllItems.gameObject.activeSelf)
        {
            HideInventory();
            ToggleFade();
        }
        else
        {
            ShowInventory();
            ToggleFade();
        }
    }

    public void ShowInventory()
    {
        AllItems.gameObject.SetActive(true);
        UpdateVisual();
    }

    public void HideInventory()
    {
        AllItems.gameObject.SetActive(false);
    }

    public void UpdateVisual()
    {
        // 清空当前的 UI 元素（销毁之前的物品容器）
        foreach (Transform child in AllItems)
        {
            Destroy(child.gameObject);
        }

        // 遍历背包中的所有物品，实例化并显示它们
        foreach (var item in InventoryManager.Instance.items)
        {
            // 实例化一个新的 ItemContainer
            GameObject newItemContainer = Instantiate(itemContainerPrefab, AllItems);

            // 获取容器中的 Image 组件
            Image itemImage = newItemContainer.transform.Find("Image").GetComponent<Image>();
            Text itemNameText = newItemContainer.transform.Find("Text").GetComponent<Text>(); // 获取物品名称的 Text 组件

            // 设置 Image 组件的图片为物品的图片
            if (itemImage != null)
            {
                itemImage.sprite = item.itemImage; // 设置物品的图片
            }

            // 设置物品名称文本
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName; // 显示物品名称
            }

            #region 可选按钮

            // // 可选：为按钮添加事件（例如点击使用物品）
            // Button itemButton = newItemContainer.transform.Find("Button")?.GetComponent<Button>();
            // if (itemButton != null)
            // {
            //     itemButton.onClick.AddListener(() => UseItem(item)); // 为按钮绑定点击事件
            // }

            #endregion
        }
    }

    #region 渐隐方法
    public void ToggleFade()
    {
        if (_isAnimating) return; // 如果动画正在进行，阻止新的淡入淡出操作

        if (_canFade)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }

    private void FadeIn()
    {
        // 启用面板并设置渐入效果
        _isAnimating = true;
        uiPanel.gameObject.SetActive(true);
        Sequence fadeInSequence = DOTween.Sequence();
        fadeInSequence.Append(uiPanelRectTransform.DOAnchorPosY(endY, fadeDuration))
                      .Join(uiPanel.DOFade(1, fadeDuration))
                      .OnStart(() =>
                      {
                          uiPanel.blocksRaycasts = true;
                      })
                      .OnComplete(() =>
                      {
                          _canFade = false; // 渐入完成后切换状态
                          _isAnimating = false; // 动画完成，允许新的淡入淡出操作
                      });
        fadeInSequence.Play();
    }

    private void FadeOut()
    {
        // 渐出效果并在完成时禁用面板
        _isAnimating = true;
        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Append(uiPanelRectTransform.DOAnchorPosY(startY, fadeDuration))
                       .Join(uiPanel.DOFade(0, fadeDuration))
                       .OnComplete(() =>
                       {
                           uiPanel.blocksRaycasts = false;
                           uiPanel.gameObject.SetActive(false);
                           _canFade = true; // 渐出完成后切换状态
                           _isAnimating = false; // 动画完成，允许新的淡入淡出操作
                       });
        fadeOutSequence.Play();
    }
    #endregion
}