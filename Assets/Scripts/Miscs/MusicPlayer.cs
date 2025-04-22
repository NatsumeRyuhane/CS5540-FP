using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A spatial-aware music player that can simulate wall occlusion effects and distance attenuation
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Music clip to loop")]
    public AudioClip musicClip;
    
    [Tooltip("Base volume")]
    [Range(0f, 1f)]
    public float baseVolume = 0.7f;
    
    [Tooltip("Automatically start playing")]
    public bool playOnStart = false;
    
    [Header("Spatial Audio Settings")]
    [Tooltip("Spatial blend (0=2D, 1=fully 3D)")]
    [Range(0f, 1f)]
    public float spatialBlend = 1f;
    
    [Tooltip("Minimum distance for sound (volume remains maximum within this distance)")]
    public float minDistance = 5f;
    
    [Tooltip("Maximum distance for sound (sound won't be heard beyond this)")]
    public float maxDistance = 50f;
    
    [Tooltip("Volume rolloff curve (1=linear, 2=square inverse...)")]
    public float rolloffFactor = 1f;
    
    [Header("Wall Occlusion Settings")]
    [Tooltip("Enable wall occlusion effects")]
    public bool enableOcclusion = true;
    
    [Tooltip("Ray detection frequency (check every X seconds)")]
    public float occlusionCheckInterval = 0.2f;
    
    [Tooltip("Volume attenuation factor per wall")]
    [Range(0f, 1f)]
    public float wallAttenuationFactor = 0.5f;
    
    [Tooltip("Layers that can block sound")]
    public LayerMask wallLayers;
    
    [Tooltip("Maximum volume attenuation ratio per wall")]
    [Range(0f, 1f)]
    public float maxWallAttenuation = 0.2f;
    
    // Private variables
    private AudioSource _audioSource;
    private Transform _listenerTransform;
    private int _wallsBetweenPlayerAndSource = 0;
    private float _currentOcclusionVolumeFactor = 1f;
    private float _targetVolume = 0f;
    
    private void Awake()
    {
        // Get or add AudioSource component
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure basic AudioSource settings
        _audioSource.clip = musicClip;
        _audioSource.loop = true;
        _audioSource.volume = baseVolume;
        _audioSource.playOnAwake = false;
        
        // Configure 3D sound settings
        _audioSource.spatialBlend = spatialBlend;
        _audioSource.minDistance = minDistance;
        _audioSource.maxDistance = maxDistance;
        _audioSource.rolloffMode = AudioRolloffMode.Custom;
        
        // Set volume attenuation curve
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);  // Maximum volume at minimum distance
        curve.AddKey(1f, 0f);  // Zero volume at maximum distance
        
        for (float t = 0.1f; t < 0.9f; t += 0.1f)
        {
            float volume = Mathf.Pow(1f - t, rolloffFactor);
            curve.AddKey(t, volume);
        }
        
        _audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
        
        _targetVolume = baseVolume;
    }
    
    private void Start()
    {
        // Get main audio listener
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            _listenerTransform = listener.transform;
        }
        else
        {
            // If no audio listener found, it's usually on the main camera
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                _listenerTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("No AudioListener or main camera found, spatial audio may not work properly!");
            }
        }
        
        // If set to auto-play, start playing
        if (playOnStart && musicClip != null)
        {
            PlayMusic();
        }
        
        // Start wall detection
        if (enableOcclusion && _listenerTransform != null)
        {
            StartCoroutine(CheckOcclusion());
        }
    }
    
    private void Update()
    {
        // Apply wall attenuation effect
        if (enableOcclusion)
        {
            _audioSource.volume = baseVolume * _currentOcclusionVolumeFactor;
        }
    }
    
    /// <summary>
    /// Check if there are walls between the player and the listener
    /// </summary>
    private IEnumerator CheckOcclusion()
    {
        while (true)
        {
            if (_listenerTransform != null)
            {
                Vector3 directionToListener = (_listenerTransform.position - transform.position).normalized;
                float distanceToListener = Vector3.Distance(transform.position, _listenerTransform.position);
                
                // Use raycasting to detect walls
                RaycastHit[] hits = Physics.RaycastAll(
                    transform.position, 
                    directionToListener, 
                    distanceToListener, 
                    wallLayers
                );
                
                // Calculate the number of walls encountered
                _wallsBetweenPlayerAndSource = hits.Length;
                
                // Calculate attenuation factor
                float attenuationFactor = 1f;
                
                for (int i = 0; i < _wallsBetweenPlayerAndSource; i++)
                {
                    attenuationFactor *= (1f - wallAttenuationFactor);
                }
                
                // Ensure it's not less than maximum attenuation value
                attenuationFactor = Mathf.Max(attenuationFactor, maxWallAttenuation);
                
                // Smoothly transition to new attenuation value
                _currentOcclusionVolumeFactor = Mathf.Lerp(
                    _currentOcclusionVolumeFactor, 
                    attenuationFactor, 
                    0.2f
                );
                
                // Optional: Debug ray
                Debug.DrawRay(transform.position, directionToListener * distanceToListener, 
                    _wallsBetweenPlayerAndSource > 0 ? Color.red : Color.green, 
                    occlusionCheckInterval);
            }
            
            yield return new WaitForSeconds(occlusionCheckInterval);
        }
    }
    
    /// <summary>
    /// Start playing music
    /// </summary>
    public void PlayMusic()
    {
        if (musicClip == null)
        {
            Debug.LogWarning("No music clip specified!");
            return;
        }
        
        _audioSource.Play();
    }
    
    /// <summary>
    /// Stop playing music
    /// </summary>
    public void StopMusic()
    {
        _audioSource.Stop();
    }
    
    /// <summary>
    /// Set music volume
    /// </summary>
    public void SetVolume(float newVolume)
    {
        baseVolume = Mathf.Clamp01(newVolume);
    }
    
    /// <summary>
    /// Get current wall count (for debugging)
    /// </summary>
    public int GetWallCount()
    {
        return _wallsBetweenPlayerAndSource;
    }
    
    /// <summary>
    /// Draw Debug information
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw minimum and maximum distance ranges
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, minDistance);
        
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}