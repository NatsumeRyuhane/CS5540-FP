using UnityEngine;


public class DialogueObject : InteractableObject
{
    [Header("Dialogue Settings")]
    public string[] dialogueTexts;
    private int _currentDialogueIndex;
    
    public override void Interact()
    {
        InteractEffect();
    }

    protected override void InteractEffect()
    {
        if (dialogueTexts == null || dialogueTexts.Length == 0)
        {
            Debug.LogWarning($"No dialogue texts assigned on {gameObject.name}");
            return;
        }
        
        if (_currentDialogueIndex < dialogueTexts.Length)
        {
            // Display the current dialogue text
            LevelUIManager.Instance.FirePlayerMessage(dialogueTexts[_currentDialogueIndex]);
            _currentDialogueIndex++;
        }
        else
        {
            // Reset the dialogue index to loop through the dialogues again
            _currentDialogueIndex = 0;
        }
    }
}