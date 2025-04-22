using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public abstract class AnomalyBehavior : InteractableObject
{
    public GameObject anomalyPrefab;
    public GameObject targetPrefab;

    protected String _anomalyName;
    
    protected bool IsActive { get; private set; }
    protected bool IsSolved { get; private set; }
    
    private void Start()
    {
        if (targetPrefab == null)
            throw new ArgumentNullException(nameof(targetPrefab), "TargetPrefab cannot be null");
        
        Reset();
        OnStart();
    }
    
    public void AddTargetToLevelManager()
    {
        if (targetPrefab == null)
            throw new ArgumentNullException(nameof(targetPrefab), "TargetPrefab cannot be null");
        
        LevelManager.Instance.RegisterTarget(targetPrefab.GetComponent<TargetBehavior>());
    }
    
    protected virtual void OnStart() 
    {
        // Override this method to implement specific anomaly effects
    }
    
    protected bool IsThisVisibleInMainCamera()
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
        IsActive = isActive;
    }
    
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        OnAnomalyEffectStart();
        anomalyPrefab.SetActive(true);
    }
    
    public void Deactivate()
    {
        if (!IsActive)
            return;
        
        OnAnomalyEffectEnd();
        IsActive = false;
        anomalyPrefab.SetActive(false);
    }

    protected virtual void OnAnomalyEffectStart()
    {
        // Override this method to implement specific anomaly effects
    }
    
    protected virtual void OnAnomalyEffectEnd()
    {
        // Override this method to implement specific anomaly effects
    }
    
    public void SetSolved(bool isSolved)
    {
        IsSolved = isSolved;
        targetPrefab.SetActive(isSolved);
    }

    public void Reset()
    {
        IsActive = false;
        anomalyPrefab.SetActive(false);
        targetPrefab.SetActive(false);
        IsSolved = false;
    }
    public string GetAnomalyName()
    {
        if (string.IsNullOrEmpty(_anomalyName))
        {
            Debug.LogWarning($"Anomaly name is not set for {gameObject.name}");
            return "Unnamed Anomaly";
        }
        
        return _anomalyName;
    }
}