using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerMessageAnomaly : AnomalyBehavior
{
    private string[] messages =
    {
        "- you left something behind -",
        "- it's getting harder to remember -",
        "- this place remembers you -",
        "- they're not gone. just quiet. -",
        "- the sky changed again. did you notice? -",
        "- we tried to warn you -",
        "- don't trust the lights -",
        "- did you think it was your idea? -",
        "- you've only made it worse -",
        "- return to the origin point -",
        "- connection lost. but you're still here. -",
        "- time since last incident: UNKNOWN -",
        "- subject has deviated from expected pattern -",
        "- oversight node offline. recalculating reality -",
        "- report filed: containment unsuccessful -",
        "- proximity alert: self detected -",
        "- system integrity: deteriorating -",
        "- last safe moment: corrupted -",
        "- you are not synced -",
        "- memory slot 03: empty (wasn't it full?) -"
    };
    
    protected override void OnAwake()
    {
        _anomalyName = "PlayerMessageAnomaly";
    }
    
    private IEnumerator FirePlayerMessagePeriodically(float duration)
    {
        while (IsActive)
        {
            if (messages.Length > 0)
            {
                int randomIndex = Random.Range(0, messages.Length);
                string message = messages[randomIndex];
                LevelUIManager.Instance.FirePlayerMessage(message, duration * 0.8f);
            }
            
            yield return new WaitForSeconds(duration);
        }
    }
    
    protected override void OnAnomalyEffectStart()
    {
        // Start the coroutine to fire messages periodically
        StartCoroutine(FirePlayerMessagePeriodically(5f));
    }
    
    public override void Interact()
    {
        return;
    }

    protected override void InteractEffect()
    {
        return;
    }
}
