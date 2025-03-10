using UnityEngine;

// 物品类型枚举
// 说明：定义游戏中不同类型的物品
// 用途：用于物品的分类和管理
public enum ItemType
{
    Key, // 关键剧情道具    
    Puzzle, // 解谜道具
    Common // 普通道具
}

// 物品数据配置类
// 说明：定义物品的基本属性和行为
// 用途：
// 1. 在Unity编辑器中创建可配置的物品数据
// 2. 存储物品的基本信息和状态
// 3. 提供物品相关的游戏逻辑支持
[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemsSO")]
public class ItemSO : ScriptableObject
{
    [Tooltip("在UI中显示的物品图像")] public Sprite itemImage; // 物品的显示图片

    [Tooltip("物品在游戏中的显示名称")] public string itemName; // 物品的名称

    [Tooltip("用于确定物品的分类：关键道具、解谜道具或普通道具")] public ItemType itemType; // 物品的类型

    [Tooltip("标记物品是否已被玩家收集")] public bool isCollected; // 收集状态

    [Tooltip("标记物品是否可以被使用")] public bool canBeUsed; // 使用状态

    [Tooltip("与该物品关联的游戏物体，可用于触发特定事件或效果")] public GameObject triggerGameObject; // 关联的触发物体

    [Tooltip("物品的使用说明或描述文本")] [TextArea] public string useDescription; // 使用说明
}