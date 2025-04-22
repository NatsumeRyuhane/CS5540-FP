using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(AudioSource))]
public class DoorBehavior : InteractableObject
{
    public bool allowInteract = true;

    [Header("Sound")] public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip lockedSound;
    
    [Header("Navigation")]
    public bool doLinkNavMesh = false;
    public NavMeshLink navMeshLink;

    [Header("Animation")]
    public float doorAnimationDuration = 0.5f;
    public Ease doorAnimationEase = Ease.InOutQuad;

    private GameObject _player;
    private Quaternion _closedRotation;
    private AudioSource _audioSource;
    private bool _isOpen;
    private Tween _currentTween;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _closedRotation = transform.rotation;
        _audioSource = GetComponent<AudioSource>();
        
        if (_isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    public override void Interact()
    {
        if (allowInteract)
        {
            InteractEffect();
        }
        else
        {
            UIManager.FirePlayerMessage("Seems to be locked...");
            _audioSource.PlayOneShot(lockedSound);
        }
    }

    protected override void InteractEffect()
    {
        _isOpen = !_isOpen;

        if (_isOpen) Open();
        else Close();

        // Play sound
        _audioSource.PlayOneShot(_isOpen ? openSound : closeSound);
    }

    public void Open()
    {
        _isOpen = true;
        if (doLinkNavMesh)
        {
            navMeshLink.enabled = true;
        }
        
        // Kill any ongoing animation
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }
        
        var rotationAngle = 0f;
        
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        
        // Determine which direction the player is approaching from
        var vectorToPlayer = _player.transform.position - transform.position;
        vectorToPlayer.y = 0; // Ignore height differences

        // Calculate dot product with the door's forward direction to see which side the player is on
        var dotProduct = Vector3.Dot(transform.forward, vectorToPlayer.normalized);

        // Determine which way to rotate (clockwise or counter-clockwise) based on player position
        rotationAngle = dotProduct > 0 ? -90f : 90f;
        

        // Apply rotation around the door's y-axis (vertical axis)
        var targetRotation = transform.rotation * Quaternion.Euler(0, rotationAngle, 0);
        
        // Animate using DOTween
        _currentTween = transform.DORotateQuaternion(targetRotation, doorAnimationDuration)
            .SetEase(doorAnimationEase);
    }

    public void Close()
    {
        _isOpen = false;
        if (doLinkNavMesh)
        {
            navMeshLink.enabled = false;
        }
        
        // Kill any ongoing animation
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }
        
        // Animate using DOTween
        _currentTween = transform.DORotateQuaternion(_closedRotation, doorAnimationDuration)
            .SetEase(doorAnimationEase);
    }
}
