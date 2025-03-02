using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory_SO", menuName = "Scriptable Objects/Inventory_SO")]
public class Inventory_SO : ScriptableObject
{
    public List<Items_SO> items = new List<Items_SO>();




}
