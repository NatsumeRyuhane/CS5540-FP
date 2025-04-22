using UnityEngine;
using DG.Tweening;

public class VoidMuseumAnomaly : AnomalyBehavior
{
    private GameObject _museum;
    private DoorBehavior _museumDoor;
    
    protected override void OnAwake()
    {
        _anomalyName = "Void Museum Anomaly";
    }
    
    protected override void OnStart()
    {
        _museum = LevelManager.Instance.stagePrefab.GetComponent<StageController>().museum;
        _museumDoor = LevelManager.Instance.stagePrefab.GetComponent<StageController>().museumDoor.GetComponent<DoorBehavior>();
    }
    
    protected override void OnAnomalyEffectStart()
    {
        _museum.SetActive(false);
        _museumDoor.Open();
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
