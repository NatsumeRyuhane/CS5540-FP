using System.Collections;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    protected LevelUIManager UIManager;
    protected PlayerController Player;

    private void Awake()
    {
        Player = PlayerController.Instance;
        UIManager = LevelUIManager.Instance;
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }

    public abstract void Interact();
    protected abstract void InteractEffect();

    protected bool IsPlayerLookingAtThis()
    {
        if (Player == null)
            return false;

        return (Player.GetPlayerLookingAt() == gameObject);
    }

    protected IEnumerator DoLongHold(float duration)
    {
        var timer = 0f;

        UIManager?.FadeInActionCastBar();

        while (timer < duration)
        {
            if (Input.GetKey(KeyCode.E) && IsPlayerLookingAtThis())
            {
                timer += Time.deltaTime;
                var progress = timer / duration;
                UIManager?.UpdateActionCastBar(progress);
                yield return null;
            }
            else
            {
                UIManager?.FadeOutActionCastBar();
                yield break;
            }
        }

        UIManager?.FadeOutActionCastBar();
        InteractEffect();
    }
}