using UnityEngine;

public enum ItemType
{
    Key, //关键剧情道具    
    Puzzle, //解谜道具
    Common //普通道具
}

[CreateAssetMenu(fileName = "Items_SO", menuName = "Scriptable Objects/Items_SO")]
public class Items_SO : ScriptableObject
{
    public Sprite itemImage; //物品图片
    public string itemName; //物品名称
    public ItemType itemType; //物品类型
    public bool isCollected; //是否被收集
    public bool canBeUsed; //是否可以使用
    [TextArea] public string useDescription; //使用描述
}