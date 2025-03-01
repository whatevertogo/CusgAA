using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Dialogue_SO", menuName = "Scriptable Objects/Dialogue_SO")]
public class Dialogue_SO : ScriptableObject
{
    public string characterName;
    public string characterName2;
    public Sprite Character_Image;
    public Sprite Character_Image2;
    public List<string> dialoguelinesList;
}
