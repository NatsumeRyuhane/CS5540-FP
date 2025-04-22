using UnityEngine;
using DG.Tweening;

/// <summary>
/// Creates a walking camera shake effect based on player movement
/// </summary>
[RequireComponent(typeof(Camera))]
public class WalkCameraShake : MonoBehaviour
{
    [Header("Shake Settings")] 
    [Tooltip("How long each shake cycle lasts")]
    public float shakeDuration = 0.3f;

    [Tooltip("How strong the shake is along each axis")]
    public Vector3 shakeStrength = new Vector3(0.05f, 0.07f, 0f);

    [Tooltip("How many shakes per cycle")] 
    public int vibrato = 5;

    [Tooltip("How random the shake motion is")] [Range(0, 180)]
    public float randomness = 70f;
    
    [Header("Advanced Settings")]
    [Tooltip("How fast the shake fades in when starting to walk")]
    public float fadeInTime = 0.3f;
    
    [Tooltip("How fast the shake fades out when stopping")]
    public float fadeOutTime = 0.2f;
    
    [Tooltip("Multiplies shake strength based on movement speed")]
    public float speedInfluence = 0.5f;
    
    [Tooltip("Minimum speed to start shaking")]
    public float minSpeedThreshold = 1.0f;
    
    [Tooltip("Maximum speed for shake calculation")]
    public float maxSpeedReference = 5.0f;

    private Tween walkShakeTween;
    private CharacterController controller;
    private float currentShakeStrength = 0f;
    private Vector3 originalPosition;

    void Awake()
    {
        controller = FindObjectOfType<CharacterController>();
        if (controller == null)
            Debug.LogWarning("No CharacterController found in scene!");
        
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (controller == null) return;
        
        float speed = controller.velocity.magnitude;
        bool isWalking = controller.isGrounded && speed > minSpeedThreshold;

        // Calculate desired shake strength based on movement speed
        float targetStrength = 0f;
        if (isWalking)
        {
            float speedFactor = Mathf.Clamp01(speed / maxSpeedReference);
            targetStrength = 1f + (speedFactor * speedInfluence);
        }

        // Smooth transition between strengths
        if (Mathf.Abs(currentShakeStrength - targetStrength) > 0.01f)
        {
            float transitionSpeed = targetStrength > currentShakeStrength ? (1f / fadeInTime) : (1f / fadeOutTime);
            currentShakeStrength = Mathf.Lerp(currentShakeStrength, targetStrength, Time.deltaTime * transitionSpeed);
            
            // Start shake if needed
            if (currentShakeStrength > 0.01f && walkShakeTween == null)
            {
                StartWalkShake();
            }
            // Update shake strength if needed
            else if (walkShakeTween != null && currentShakeStrength > 0.01f)
            {
                StopWalkShake();
                StartWalkShake();
            }
            
            // Stop shake if strength is nearly zero
            if (currentShakeStrength < 0.01f && walkShakeTween != null)
            {
                StopWalkShake();
            }
        }
    }

    private void StartWalkShake()
    {
        Vector3 initialStrength = shakeStrength * currentShakeStrength;
        
        walkShakeTween = transform.DOShakePosition(
                duration: shakeDuration,
                strength: initialStrength,
                vibrato: vibrato,
                randomness: randomness)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject);
    }

    private void StopWalkShake()
    {
        walkShakeTween.Kill();
        walkShakeTween = null;
        
        // Smoothly return to original position
        transform.DOLocalMove(originalPosition, fadeOutTime)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
    }

    void OnDisable()
    {
        // clean up if the script/component is turned off
        if (walkShakeTween != null)
            walkShakeTween.Kill();
        transform.localPosition = originalPosition;
    }
}
