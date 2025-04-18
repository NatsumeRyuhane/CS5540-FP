using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class AnomalyBehavior : InteractableObject
{
    protected PlayerController _player;
    protected bool _isActive { get; private set; }
    protected bool _isResolved { get; private set; }
    
    private void Awake()
    {
        _player = FindFirstObjectByType<PlayerController>();
        OnAwake();
    }
    
    protected bool IsThisVisibleInCamera()
    {
        var cam = Camera.main;
        if (cam == null)
            return false;
        
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        var bounds = GetComponent<Collider>().bounds;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(bounds.center) < -bounds.extents.magnitude)
                return false;
        }
        return true;
    }
    
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }
    
    public void SetResolved(bool isResolved)
    {
        _isResolved = isResolved;
    }

    protected abstract void OnAwake();
    
    protected abstract void OnAnomalyEffectEnd();

    public void Reset()
    {
        _isActive = false;
        _isResolved = false;
    }
}