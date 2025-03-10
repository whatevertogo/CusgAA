using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Scriptable Objects/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public string characterName;
    public string characterName2;
    public Sprite Character_Image;
    public Sprite Character_Image2;
    public List<string> dialoguelinesList;
}