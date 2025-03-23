using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorBehavior : InteractableObject
{
    public bool allowInteract = true;
 
    [Header("Sound")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip lockedSound;
    
    private GameObject _player;
    private Quaternion _closedRotation;
    private LevelUIManager _UIManager;
    private AudioSource _audioSource;
    private bool _isOpen;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _UIManager = FindFirstObjectByType<LevelUIManager>();
        _closedRotation = transform.rotation;
        _audioSource = GetComponent<AudioSource>();
    }
    public override void Interact()
    {
        if (allowInteract)
        {
            InteractEffect();
        }
        else
        {
            StartCoroutine(_UIManager.FirePlayerMessage("Seems to be locked..."));
            _audioSource.PlayOneShot(lockedSound);
        }
    }
    
    public override void InteractEffect()
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
        StartCoroutine(OpenAnimation());
    }

    IEnumerator OpenAnimation(float duration = 0.5f)
    {
        float timer = 0f;
        Quaternion initialRotation = transform.rotation;
        
        // First, determine which direction the player is approaching from
        var vectorToPlayer = _player.transform.position - transform.position;
        vectorToPlayer.y = 0; // Ignore height differences

        // Calculate dot product with the door's forward direction to see which side the player is on
        float dotProduct = Vector3.Dot(transform.forward, vectorToPlayer.normalized);

        // Determine which way to rotate (clockwise or counter-clockwise) based on player position
        float rotationAngle = dotProduct > 0 ? -90f : 90f;

        // Apply rotation around the door's y-axis (vertical axis)
        var targetRotation = transform.rotation * Quaternion.Euler(0, rotationAngle, 0);
        
        while (timer < duration)
        {
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    
    public void Close()
    {
        _isOpen = false;
        StartCoroutine(CloseAnimation());
    }
    
    IEnumerator CloseAnimation(float duration = 0.5f)
    {
        float timer = 0f;
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = _closedRotation;

        while (timer < duration)
        {
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
    
}