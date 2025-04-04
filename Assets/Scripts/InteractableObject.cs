using System.Collections;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    private LevelUIManager _levelUIManager;
    
    public abstract void Interact();
    public abstract void InteractEffect();
    
    protected IEnumerator DoLongHold(float duration)
    {
        float timer = 0f;
        float progress = 0f;
        
        if (_levelUIManager == null)
        {
            _levelUIManager = FindFirstObjectByType<LevelUIManager>();
        }
        _levelUIManager.FadeInActionCastBar();
        
        while (timer < duration)
        {
            if (Input.GetKey(KeyCode.E))
            {
                timer += Time.deltaTime;
                progress = timer / duration;
                _levelUIManager.UpdateActionCastBar(progress);
                yield return null;
            }
            else
            {
                _levelUIManager.FadeOutActionCastBar();
                yield break;
            }
        }
        _levelUIManager.FadeOutActionCastBar();
        InteractEffect();
    }
}