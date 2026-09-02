using UnityEngine;

public class ChoiceButton : MonoBehaviour
{
    private DialogueChoice choice;

    public void SetChoice(DialogueChoice newChoice)
    {
        choice = newChoice;
    }

    public DialogueChoice GetChoice()
    {
        return choice;
    }
}