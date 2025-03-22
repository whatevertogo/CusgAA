using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Scriptable Objects/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    [Header("未使用")]
    public string characterName;
    public string characterName2;
    public Sprite Character_Image;
    public Sprite Character_Image2;
    [Header("对话内容")]
    public List<string> dialoguelinesList;
    [TextArea]
    public string 对话介绍;

}