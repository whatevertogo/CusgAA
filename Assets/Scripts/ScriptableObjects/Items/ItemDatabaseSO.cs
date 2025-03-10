using System.Collections.Generic;
using UnityEngine;

// 物品数据库配置类
// 说明：管理所有游戏物品的集中存储
// 用途：
// 1. 在Unity编辑器中统一管理所有物品数据
// 2. 提供物品数据的集中访问点
// 3. 便于物品系统的扩展和维护
// 4. 支持在运行时动态管理物品列表
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabaseSO : ScriptableObject
{
    // 游戏中所有物品的列表
    // 说明：存储所有可用物品的配置数据
    // 用途：
    // 1. 在编辑器中配置所有游戏物品
    // 2. 运行时提供物品查询和管理功能
    // 3. 支持动态添加和移除物品
    [Tooltip("游戏中所有可用物品的列表")] public List<ItemSO> itemsList = new();
}