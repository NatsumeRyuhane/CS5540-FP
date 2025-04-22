using UnityEngine;
using DG.Tweening;

public class VoidMuseumAnomaly : AnomalyBehavior
{
    
    protected override void OnAwake()
    {
        _anomalyName = "Void Museum Anomaly";
    }
    
    protected override void OnAnomalyEffectStart()
    {
        var museum = LevelManager.Instance.GetCurrentStageController().museum;
        var museumDoor = LevelManager.Instance.GetCurrentStageController().museumDoor.GetComponent<DoorBehavior>();
        museum.SetActive(false);
        museumDoor.Open();
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
