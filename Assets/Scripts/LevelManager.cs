using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject persistentObjectsAnchor;
    private HashSet<TargetBehavior> _targets = new HashSet<TargetBehavior>();
    private List<Objects.Objective> _objectives = new List<Objects.Objective>();
    
    
    [Header("Level Info")]
    public string levelName;

    [Header("Level Config")] 
    public float levelTime;

    public GameObject StagePrefab;
    
    [HideInInspector] public float LevelTimer { get; private set; }
    [HideInInspector] public bool IsLevelComplete { get; private set; }
    [HideInInspector] public bool IsLevelFailed { get; private set; }
    [HideInInspector] public bool AllowPlayerControl { get; private set; }


    private LevelUIManager _UIManager;
    private void Start()
    {
        _UIManager = FindFirstObjectByType<LevelUIManager>();
        _UIManager.UpdateLevelName(levelName);
        _UIManager.UpdateTime("12 AM");
        StartCoroutine(HideGuideText());
        
        SetAllowPlayerControl(true);
        StartCoroutine(_UIManager.FirePlayerMessage("I need to get out before the clock strikes 6 AM."));
        
        CreateLevelObjective("Find and interact with all anomallies (cubes, for now).");
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
        
        var objDesc = "Find and interact with all anomallies (cubes, for now). (" + completed + "/" + total + ")";
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

    public void CheckLevelComplete()
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

    void LevelFail()
    {
        IsLevelFailed = true;
        SetAllowPlayerControl(false);
        StartCoroutine(_UIManager.DoLevelFailSequence(3));
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
    
    public void ReloadLevel()
    {
        // Reload the level
        LoadingManager.instance.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    public void ExitToMainMenu()
    {
        // Load the main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    public void RegisterTarget(TargetBehavior target)
    {
        _targets.Add(target);
        CheckLevelComplete();
        UpdateLevelObjective();
    }
}
