using UnityEngine;
using DG.Tweening;

public class PaintAnomaly : AnomalyBehavior
{
    protected override void OnAwake()
    {
        _anomalyName = "PaintAnomaly";
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
