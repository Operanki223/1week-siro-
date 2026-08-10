using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueNode> nodes = new List<DialogueNode>();
}


[System.Serializable]
public class DialogueNode
{
    [Header("会話")]
    public string speaker;

    [TextArea(2, 5)]
    public string text;


    [Header("選択肢")]
    public List<DialogueChoice> choices = new List<DialogueChoice>();


    [Header("選択肢パネル")]
    [Tooltip("DialogueManagerのChoice Panel Listの番号")]
    public int choicePanelIndex = 0;


    [Header("選択肢がない場合の次のノード")]
    public int nextNode = -1;
}


[System.Serializable]
public class DialogueChoice
{
    [Header("選択肢")]
    public string text;


    [Header("分岐先")]
    public int nextNode = -1;


    [Header("選択時に実行する処理")]
    public UnityEvent onSelected;
}