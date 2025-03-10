using UnityEngine;
using UnityEngine.UI;

// 物品按钮控制器
// 说明：控制打开/关闭背包界面的按钮行为
// 用途：
// 1. 为背包按钮提供交互功能
// 2. 关联物品管理器UI组件
public class ItemButton : MonoBehaviour
{
    [SerializeField] private ItemsManagerUI itemsManagerUI;

    // 初始化按钮行为
    // 说明：
    // 1. 获取Button组件
    // 2. 添加点击事件监听
    // 3. 绑定背包界面的开关功能
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => itemsManagerUI.Open_CloseInventory());
    }
}