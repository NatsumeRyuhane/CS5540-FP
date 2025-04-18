using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Level Info")]
    public string levelDisplayName;
    public string nextLevelName;

    [Header("Level Config")] 
    public float levelTime;
    public AnomalyBehavior[] anomalies;
    [FormerlySerializedAs("StagePrefab")] 
    public GameObject stagePrefab;
    [SerializeField] private GameObject persistentObjectsAnchor;
    
    // State properties
    [HideInInspector] public float LevelTimer { get; private set; }
    [HideInInspector] public bool IsLevelComplete { get; private set; }
    [HideInInspector] public bool IsLevelFailed { get; private set; }
    [HideInInspector] public bool AllowPlayerControl { get; private set; }
    [HideInInspector] public bool WasButtonPressed { get; private set; }

    // Private fields
    private readonly HashSet<TargetBehavior> _targets = new HashSet<TargetBehavior>();
    private readonly List<Objects.Objective> _objectives = new List<Objects.Objective>();
    private LevelUIManager _uiManager;

    private void Start()
    {
        _uiManager = LevelUIManager.Instance;
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        _uiManager.UpdateLevelName(levelDisplayName);
        _uiManager.UpdateTime("12 AM");
        StartCoroutine(HideGuideText());
        
        SetAllowPlayerControl(true);
        _uiManager.FirePlayerMessage("I need to get out before the clock strikes 6 AM.");
        
        CreateLevelObjective("Find and report any anomaly.");
    }

    private void Update()
    {
        if (IsLevelFailed || IsLevelComplete)
            return;
        
        UpdateLevelTimer();
    }

    private void UpdateLevelTimer()
    {
        LevelTimer += Time.deltaTime;
        _uiManager.UpdateTime(CalculateInGameTime(LevelTimer) + " AM");
        
        if (LevelTimer >= levelTime)
            LevelFail();
    }

    // Objective management
    public void CreateLevelObjective(string objective)
    {
        var obj = new Objects.Objective
        {
            Description = objective,
            IsCompleted = false,
            IsActive = true,
            IsFailed = false
        };
        
        obj.UIElement = _uiManager.CreateObjective(objective);
        _objectives.Add(obj);
    }
    
    public void UpdateLevelObjective()
    {
        if (_objectives.Count == 0)
            return;
        
        var obj = _objectives[0];
        
        int completed = 0;
        int total = _targets.Count;
        
        foreach (var target in _targets)
        {
            if (target.Completed)
                completed++;
        }
        
        string objDesc = $"Find and interact with all anomaly. ({completed}/{total})";
        _uiManager.UpdateObjective(objDesc, obj.UIElement);
    }

    // Level state management
    public void CheckLevelCompleteCondition()
    { 
        UpdateLevelObjective();
        
        bool allTargetsCompleted = true;
        foreach (var target in _targets)
        {
            if (!target.Completed)
            {
                allTargetsCompleted = false;
                break;
            }
        }
       
        IsLevelComplete = allTargetsCompleted;
    }
    
    public void LevelComplete()
    {
        IsLevelComplete = true;
        SetAllowPlayerControl(false);
        _uiManager.DoLevelCompleteSequence(3);
    }

    public void LevelFail()
    {
        IsLevelFailed = true;
        SetAllowPlayerControl(false);
        _uiManager.DoLevelFailSequence(3);
    }
    
    public void SetAllowPlayerControl(bool allow)
    {
        AllowPlayerControl = allow;
        Cursor.visible = !allow;
        Cursor.lockState = allow ? CursorLockMode.Locked : CursorLockMode.None;
    }

    // Level utility methods
    private string CalculateInGameTime(float time)
    {
        int hour = (int)Mathf.Floor(LevelTimer / (levelTime/6));
        return hour == 0 ? "12" : hour.ToString();
    }
    
    private IEnumerator HideGuideText()
    {
        yield return new WaitForSeconds(10);
        _uiManager.SetGuideTextDisplay(false);
    }

    public void MoveObjects(Vector3 direction)
    {
       persistentObjectsAnchor.transform.position += direction;
    }
    
    public void SetButtonPressed(bool pressed)
    {
        WasButtonPressed = pressed;
    }

    public void ResetLevelProgress()
    {
        foreach (var target in _targets)
        {
            target.Reset();
        }
        
        foreach (var anomaly in anomalies)
        {
            anomaly.Reset();
        }
    }
    
    // Level navigation
    public void ReloadLevel()
    {
        LoadingManager.Instance.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    public void ExitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    public void LoadNextLevel()
    {
        LoadingManager.Instance.LoadScene(nextLevelName);
    }
    
    // Target registration
    public void RegisterTarget(TargetBehavior target)
    {
        _targets.Add(target);
        CheckLevelCompleteCondition();
        UpdateLevelObjective();
    }
}
