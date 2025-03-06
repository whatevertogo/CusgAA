/*
 * 存储物品的背包scriptObject类
 */
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> itemsList=new();
}
