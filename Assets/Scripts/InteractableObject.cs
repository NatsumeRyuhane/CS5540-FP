using System.Collections;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    protected LevelUIManager LevelUIManager;
    protected PlayerController PlayerController;

    private void Awake()
    {
        PlayerController = FindFirstObjectByType<PlayerController>();
        LevelUIManager = FindFirstObjectByType<LevelUIManager>();
    }

    public abstract void Interact();
    protected abstract void InteractEffect();

    protected bool IsPlayerLookingAtThis()
    {
        if (PlayerController == null)
            return false;

        return (PlayerController.GetPlayerLookingAt() == gameObject);
    }

    protected IEnumerator DoLongHold(float duration)
    {
        float timer = 0f;
        float progress = 0f;

        LevelUIManager?.FadeInActionCastBar();

        while (timer < duration)
        {
            if (Input.GetKey(KeyCode.E) && IsPlayerLookingAtThis())
            {
                timer += Time.deltaTime;
                progress = timer / duration;
                LevelUIManager?.UpdateActionCastBar(progress);
                yield return null;
            }
            else
            {
                LevelUIManager?.FadeOutActionCastBar();
                yield break;
            }
        }

        LevelUIManager?.FadeOutActionCastBar();
        InteractEffect();
    }
}