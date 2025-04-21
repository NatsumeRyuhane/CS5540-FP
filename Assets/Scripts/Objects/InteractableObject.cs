using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all objects that can be interacted with by the player.
/// </summary>
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

    /// <summary>
    /// Virtual method called during Awake that can be overridden by derived classes.
    /// </summary>
    protected virtual void OnAwake()
    {
        // Override in child classes if needed
    }

    /// <summary>
    /// Called when the player attempts to interact with this object.
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// Implements the effect that happens when interaction is successful.
    /// </summary>
    protected abstract void InteractEffect();

    /// <summary>
    /// Checks if the player is currently looking at this object.
    /// </summary>
    /// <returns>True if the player is looking at this object, false otherwise.</returns>
    protected bool IsPlayerLookingAtThis()
    {
        if (Player == null)
        {
            Debug.LogWarning($"Player reference is null in {gameObject.name}");
            return false;
        }

        return Player.GetPlayerLookingAt() == gameObject;
    }

    /// <summary>
    /// Handles interaction that requires holding a button for a duration.
    /// Shows and updates a progress bar while the button is held.
    /// </summary>
    /// <param name="duration">The time in seconds the button must be held.</param>
    protected IEnumerator DoLongHold(float duration)
    {
        float timer = 0f;

        if (UIManager == null)
        {
            Debug.LogWarning($"UIManager reference is null in {gameObject.name}");
        }
        else
        {
            UIManager.FadeInActionCastBar();
        }

        while (timer < duration)
        {
            if (Input.GetKey(KeyCode.E) && IsPlayerLookingAtThis())
            {
                timer += Time.deltaTime;
                float progress = timer / duration;
                
                if (UIManager != null)
                {
                    UIManager.UpdateActionCastBar(progress);
                }
                
                yield return null;
            }
            else
            {
                if (UIManager != null)
                {
                    UIManager.FadeOutActionCastBar();
                }
                yield break;
            }
        }

        if (UIManager != null)
        {
            UIManager.FadeOutActionCastBar();
        }
        
        InteractEffect();
    }
}
