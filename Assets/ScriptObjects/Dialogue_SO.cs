using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue_SO", menuName = "Scriptable Objects/Dialogue_SO")]
public class Dialogue_SO : ScriptableObject
{
    public string characterName;
    public List<string> dialoguelinesList;
}
