using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameObject persistentObjectsAnchor;
    private readonly HashSet<TargetBehavior> _targets = new HashSet<TargetBehavior>();
    private readonly List<Objects.Objective> _objectives = new List<Objects.Objective>();
    
    [Header("Level Info")]
    public string levelDisplayName;
    public string nextLevelName;

    [Header("Level Config")] 
    public float levelTime;
    
    public AnomalyBehavior[] anomalies;
    

    [FormerlySerializedAs("StagePrefab")] public GameObject stagePrefab;
    
    [HideInInspector] public float LevelTimer { get; private set; }
    [HideInInspector] public bool IsLevelComplete { get; private set; }
    [HideInInspector] public bool IsLevelFailed { get; private set; }
    [HideInInspector] public bool AllowPlayerControl { get; private set; }

    [HideInInspector] public bool WasButtonPressed {get; private set;}


    private LevelUIManager _UIManager;
    private void Start()
    {
        _UIManager = LevelUIManager.Instance;
        _UIManager.UpdateLevelName(levelDisplayName);
        _UIManager.UpdateTime("12 AM");
        StartCoroutine(HideGuideText());
        
        SetAllowPlayerControl(true);
        _UIManager.FirePlayerMessage("I need to get out before the clock strikes 6 AM.");
        
        CreateLevelObjective("Find and report any anomaly.");
    }

    public void CreateLevelObjective(String objective)
    {
        var obj = new Objects.Objective()
        {
            Description = objective,
            IsCompleted = false,
            IsActive = true,
            IsFailed = false
        };
        
        obj.UIElement = _UIManager.CreateObjective(objective);
        _objectives.Add(obj);
    }
    
    public void UpdateLevelObjective()
    {
        if (_objectives.Count == 0)
            return;
        
        var obj = _objectives[0];
        
        var completed = 0;
        var total = _targets.Count;
        foreach (var t in _targets)
        {
            if (t.Completed)
            {
                completed++;
            }
        }
        
        var objDesc = "Find and interact with all anomaly. (" + completed + "/" + total + ")";
        _UIManager.UpdateObjective(objDesc, obj.UIElement);
    }
    
    IEnumerator HideGuideText()
    {
        yield return new WaitForSeconds(10);
        _UIManager.SetGuideTextDisplay(false);
    }

    void Update()
    {
        if (IsLevelFailed || IsLevelComplete)
        {
            return;
        }
        
        LevelTimer += Time.deltaTime;
        _UIManager.UpdateTime(CalculateInGameTime(LevelTimer)+ " AM");
        if (LevelTimer >= levelTime)
        {
            LevelFail();
        } 
    }
    
    String CalculateInGameTime(float time)
    {
        var t = (int) Mathf.Floor(LevelTimer / (levelTime/6));
        if (t == 0)
        {
            return "12";
        }
        else
        {
            return t.ToString();
        }
    }

    public void CheckLevelCompleteCondition()
    { 
        UpdateLevelObjective();
       foreach (var t in _targets)
       {
           if (!t.Completed)
           {
                IsLevelComplete = false;
               return;
           }
       }
       
       
       IsLevelComplete = true;
       return;
    }
    
    public void LevelComplete()
    {
        IsLevelComplete = true;
        SetAllowPlayerControl(false);
        _UIManager.DoLevelCompleteSequence(3);
    }

    public void LevelFail()
    {
        IsLevelFailed = true;
        SetAllowPlayerControl(false);
        _UIManager.DoLevelFailSequence(3);
    }
    
    public void SetAllowPlayerControl(bool allow)
    {
        AllowPlayerControl = allow;
        Cursor.visible = !allow;
        Cursor.lockState = allow ? CursorLockMode.Locked : CursorLockMode.None;
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
        foreach (var t in _targets)
        {
            t.Reset();
        }
        
        foreach (var a in anomalies)
        {
            a.Reset();
        }
    }
    
    public void ReloadLevel()
    {
        // Reload the level
        LoadingManager.Instance.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    public void ExitToMainMenu()
    {
        // Load the main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    public void RegisterTarget(TargetBehavior target)
    {
        _targets.Add(target);
        CheckLevelCompleteCondition();
        UpdateLevelObjective();
    }
    
    public void LoadNextLevel()
    {
        // Load the next level
        LoadingManager.Instance.LoadScene(nextLevelName);
    }
}
