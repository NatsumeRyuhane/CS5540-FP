using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class MuseumKnockingOnDoorAnomaly : AnomalyBehavior
{
    [FormerlySerializedAs("_knockSound")] public AudioClip knockSound;
    private AudioSource _audioSource;
    protected override void OnAwake()
    {
        _anomalyName = "Museum Knocking On Door Anomaly";
    }
    
    protected override void OnStart()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    protected override void OnAnomalyEffectStart()
    {
        var museum = LevelManager.Instance.GetCurrentStageController().museum;
        var museumDoor = LevelManager.Instance.GetCurrentStageController().museumDoor.GetComponent<DoorBehavior>();
        museum.SetActive(false);
        museumDoor.allowInteract = false;
        _audioSource.clip = knockSound;
        _audioSource.Play();
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
