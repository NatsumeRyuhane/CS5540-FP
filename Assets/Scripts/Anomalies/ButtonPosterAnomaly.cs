using UnityEngine;
using DG.Tweening;

public class ButtonPosterAnomaly : AnomalyBehavior
{
    protected override void OnAwake()
    {
        _anomalyName = "ButtonPosterAnomaly";
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
