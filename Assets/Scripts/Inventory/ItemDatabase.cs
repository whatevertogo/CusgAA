using System.Collections.Generic;
using UnityEngine;

/*
 * 存储物品的背包scriptObject类
 */

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemSO> itemsList=new();
}
