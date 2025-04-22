using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Handles target object behavior for level objectives
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TargetBehavior : InteractableObject
{
    [Header("Target Settings")] [Tooltip("Sound played when the target is completed")]
    public AudioClip completedSound;

    [Tooltip("Color to change to when the target is completed")]
    public Color completedColor = Color.green;

    [Tooltip("Duration for fade in/out animations")]
    public float fadeAnimationDuration = 1f;

    [Tooltip("Particle system to play when completed")]
    public ParticleSystem completionParticles;

    /// <summary>
    /// Whether this target has been completed
    /// </summary>
    [HideInInspector]
    public bool Completed { get; private set; }

    // Private fields
    private LevelManager _levelManager;
    private AudioSource _audioSource;
    private Renderer _renderer;
    private bool _isInitialActive;
    private Color _initialColor;
    private Tween _currentFadeTween;

    // Unity Lifecycle Methods

    protected override void OnAwake()
    {
        _levelManager = LevelManager.Instance;
        _levelManager.RegisterTarget(this);
        _renderer = GetComponent<Renderer>();
        Completed = false;
    }

    public void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _isInitialActive = gameObject.activeSelf;
        _initialColor = _renderer.material.color;
    }

    private void OnEnable()
    {
        FadeIn();
    }

    // Interaction Methods

    public override void Interact()
    {
        StartCoroutine(base.DoLongHold(3));
    }

    protected override void InteractEffect()
    {
        Completed = true;
        _renderer.material.color = completedColor;
        _levelManager.CheckLevelCompleteCondition();
        
        // Play particle effect if assigned
        if (completionParticles != null)
        {
            completionParticles.Play();
        }
        
        DoPostCompleteAnim(3600f);
    }

    /// <summary>
    /// Performs completion animation with fade out and rotation
    /// </summary>
    private void DoPostCompleteAnim(float duration)
    {
        Color initialColor = _renderer.material.color;
        _audioSource.PlayOneShot(completedSound);

        // Create a sequence for the animations
        Sequence sequence = DOTween.Sequence();

        // Phase 1: Spin-up effect for 1 second with acceleration
        sequence.Append(transform.DORotate(new Vector3(0, 180, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.InExpo)
            .SetRelative(true));

        // Phase 2: Constant high-speed rotation for the remaining duration
        sequence.Append(transform
            .DORotate(new Vector3(0, 360 * (duration - 1f), 0), duration - 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetRelative(true));

        // Add fade-out effect spanning the entire duration
        sequence.Insert(0, DOTween.To(
            () => _renderer.material.color,
            color => _renderer.material.color = color,
            new Color(initialColor.r, initialColor.g, initialColor.b, 0),
            duration));

        // When complete, deactivate the object
        sequence.OnComplete(() => gameObject.SetActive(false));
    }

    /// <summary>
    /// Fades in the target object
    /// </summary>
    public void FadeIn()
    {
        // Kill any existing tween
        if (_currentFadeTween != null && _currentFadeTween.IsActive())
            _currentFadeTween.Kill();
            
        // Get current color and set alpha to 0
        Color currentColor = _renderer.material.color;
        _renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
        
        // Animate alpha back to 1
        _currentFadeTween = DOTween.To(
            () => _renderer.material.color,
            color => _renderer.material.color = color,
            new Color(currentColor.r, currentColor.g, currentColor.b, 1),
            fadeAnimationDuration)
            .SetEase(Ease.OutQuad);
    }
    
    /// <summary>
    /// Fades out the target object and then disables it
    /// </summary>
    public void FadeOutAndDisable()
    {
        // Kill any existing tween
        if (_currentFadeTween != null && _currentFadeTween.IsActive())
            _currentFadeTween.Kill();
            
        Color currentColor = _renderer.material.color;
        
        // Animate to alpha 0
        _currentFadeTween = DOTween.To(
            () => _renderer.material.color,
            color => _renderer.material.color = color,
            new Color(currentColor.r, currentColor.g, currentColor.b, 0),
            fadeAnimationDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => {
                // Disable the object when fade completes
                gameObject.SetActive(false);
                // Reset alpha for next enable
                _renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
            });
    }

    /// <summary>
    /// Resets the target to its initial state
    /// </summary>
    public void Reset()
    {
        Completed = false;
        _renderer.material.color = _initialColor;
        
        // Stop particle system if it's playing
        if (completionParticles != null)
        {
            completionParticles.Stop();
            completionParticles.Clear();
        }
        
        if (_isInitialActive && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else if (!_isInitialActive && gameObject.activeSelf)
        {
            FadeOutAndDisable();
        }
    }
}
