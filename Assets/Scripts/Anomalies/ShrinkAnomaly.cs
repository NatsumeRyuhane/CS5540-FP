using UnityEngine;
using DG.Tweening;

public class ShrinkAnomaly : AnomalyBehavior
{
    [Tooltip("Duration of the shrinking effect in seconds.")]
    public float shrinkDuration = 60f;
    
    [Tooltip("Minimum scale value before the object stops shrinking.")]
    public float minScale = 0.01f;
    
    [Tooltip("Easing function for the shrinking effect.")]
    public Ease shrinkEase = Ease.Linear;
    
    private Vector3 _originalScale;
    private Tween _shrinkTween;
    private GameObject _player;
    

    protected override void OnStart()
    {
        // Find the player - assuming it has the "Player" tag
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogWarning("ShrinkAnomaly: Player not found. Ensure the player has the 'Player' tag.");
        }
    }
    
    protected override void OnAnomalyEffectStart()
    {
        if (_player == null)
        {
            // Try to find player again in case it wasn't available at start
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null) return; // Can't apply effect if no player found
        }
        
        // Save the player's scale when effect starts
        _originalScale = _player.transform.localScale;
        
        // Kill any existing tween
        if (_shrinkTween != null && _shrinkTween.IsActive())
        {
            _shrinkTween.Kill();
        }
        
        // Create a new tween to shrink the player
        _shrinkTween = _player.transform.DOScale(Vector3.one * minScale, shrinkDuration)
            .SetEase(shrinkEase);
    }
    
    protected override void OnAnomalyEffectEnd()
    {
        if (_player == null) return;
        
        // Kill the current tween if it exists
        if (_shrinkTween != null && _shrinkTween.IsActive())
        {
            _shrinkTween.Kill();
        }
        
        // Reset player to original size using DOTween
        _player.transform.DOScale(_originalScale, 0.5f);
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
