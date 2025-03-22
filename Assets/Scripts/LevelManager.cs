using System;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject objects;
    [SerializeField] private TargetBehavior[] targets;
    
    [Header("Level Info")]
    public string levelName;
    
    [Header("Level Config")]

    [Header("UI Elements")]
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelNameText;

    private void Start()
    {
        timeText.text = "12 AM";
        levelNameText.text = levelName;
        objectiveText.text = "No active objectives";
    }

    public bool LevelComplete()
    {
       foreach (var t in targets)
       { 
           if (!t.Completed) return false;
       }
       
       return true;
    }

    public void MoveObjects(Vector3 direction)
    {
       objects.transform.position += direction;
    }
}
