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
    public bool isLastLevel;

    [Header("Level Config")]
    public float levelTime;
    public AnomalyBehavior[] anomalies;
    public int anomalyGenerationChance;
    public GameObject stagePrefab;
    [SerializeField] private GameObject persistentObjectsAnchor;

    [HideInInspector] public float LevelTimer { get; private set; }
    [HideInInspector] public bool IsLevelComplete { get; private set; }
    [HideInInspector] public bool IsLevelFailed { get; private set; }
    [HideInInspector] public bool WasButtonPressed { get; private set; }
    

    private readonly HashSet<TargetBehavior> _targets = new HashSet<TargetBehavior>();
    private readonly List<Objects.Objective> _objectives = new List<Objects.Objective>();
    private int _anomalyIndex = -1;
    private bool _isAnomalyActive;

    private void Start()
    {
        // warn about potentially wrong isLastLevel setting
        if (isLastLevel && !string.IsNullOrEmpty(nextLevelName))
        {
            Debug.LogWarning("isLastLevel is set to true, but nextLevelName is not empty. " +
                             "Next Level will not load properly.");
        }
        
        InitializeLevel();
        Debug.Log($"Level Initialized with name: {levelDisplayName}, " +
                  $"anomaly generation chance: {anomalyGenerationChance}, " +
                  $"anomaly count: {anomalies.Length}, " +
                  $"target registration count: {_targets.Count}");
    }

    private void InitializeLevel()
    {
        LevelUIManager.Instance.UpdateLevelName(levelDisplayName);
        LevelUIManager.Instance.UpdateTime("12 AM");
        StartCoroutine(HideGuideText());

        PlayerController.Instance.SetAllowPlayerControl(true);
        LevelUIManager.Instance.FirePlayerMessage("I need to get out before the clock strikes 6 AM.");

        ResetLevelProgress();
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
        LevelUIManager.Instance.UpdateTime(CalculateInGameTime(LevelTimer) + " AM");

        if (LevelTimer >= levelTime)
            LevelFail();
    }

    private string CalculateInGameTime(float time)
    {
        int hour = (int)Mathf.Floor(LevelTimer / (levelTime / 6));
        return hour == 0 ? "12" : hour.ToString();
    }

    private IEnumerator HideGuideText()
    {
        yield return new WaitForSeconds(10);
        LevelUIManager.Instance.SetGuideTextDisplay(false);
    }

    public void CreateLevelObjective(string objective)
    {
        var obj = new Objects.Objective
        {
            Description = objective,
            IsCompleted = false,
            IsActive = true,
            IsFailed = false
        };

        obj.UIElement = LevelUIManager.Instance.CreateObjective(objective);
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
        LevelUIManager.Instance.UpdateObjective(objDesc, obj.UIElement);
    }
    public void CheckLevelCompleteCondition()
    {
        UpdateLevelObjective();

        var allTargetsCompleted = true;
        var completedTargets = 0;
        foreach (var target in _targets)
        {
            if (!target.Completed)
            {
                allTargetsCompleted = false;
            } else
            {
                completedTargets++;
            }
        }
        
        Debug.Log($"Completed targets: {completedTargets}/{_targets.Count}");
        IsLevelComplete = allTargetsCompleted;
    }

    public void LevelComplete()
    {
        IsLevelComplete = true;
        PlayerController.Instance.SetAllowPlayerControl(false);
        LevelUIManager.Instance.DoLevelCompleteSequence(3);
    }

    public void LevelFail()
    {
        IsLevelFailed = true;
        PlayerController.Instance.SetAllowPlayerControl(false);
        LevelUIManager.Instance.DoLevelFailSequence(3);
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

        // shuffle the anomalies array
        for (int i = 0; i < anomalies.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, anomalies.Length);
            (anomalies[i], anomalies[randomIndex]) = (anomalies[randomIndex], anomalies[i]);
        }
        
        _anomalyIndex = -1;
    }

    public void MoveObjects(Vector3 direction)
    {
        persistentObjectsAnchor.transform.position += direction;
    }

    public void SetButtonPressed(bool pressed)
    {
        WasButtonPressed = pressed;
    }

    public void SetButtonPressed()
    {
        WasButtonPressed = true;
    }

    public AnomalyBehavior GenerateAnomaly()
    {
        if (_anomalyIndex >= anomalies.Length - 1)
        {
            ClearAnomaly();            
            return null;
        }

        _anomalyIndex++;
        _isAnomalyActive = true;
        return anomalies[_anomalyIndex];
    }

    public void ClearAnomaly()
    {
        _isAnomalyActive = false;
    }
    
    public AnomalyBehavior GetActiveAnomaly()
    {
        if (_anomalyIndex < 0 || _anomalyIndex >= anomalies.Length)
            return null;
        
        if (_isAnomalyActive)
            return anomalies[_anomalyIndex];
        else
            return null;
    }

    public void RegisterTarget(TargetBehavior target)
    {
        _targets.Add(target);
        CheckLevelCompleteCondition();
        UpdateLevelObjective();
    }

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
        if (isLastLevel)
        {
            Debug.Log("This is the last level. No next level to load.");
            ExitToMainMenu();
            return;
        }
        
        LoadingManager.Instance.LoadScene(nextLevelName);
    }
}
