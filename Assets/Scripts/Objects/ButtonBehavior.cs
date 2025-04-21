using UnityEngine;

public class ButtonBehavior : InteractableObject
{
    public AudioClip buttonPressSFX;
    
    public override void Interact()
    {
        InteractEffect();
    }
    
    protected override void InteractEffect()
    {
        if (LevelManager.Instance.WasButtonPressed)
            return;
        
        LevelManager.Instance?.SetButtonPressed();
        
        if (buttonPressSFX != null)
        {
            AudioSource.PlayClipAtPoint(buttonPressSFX, transform.position);
        }
    }
}