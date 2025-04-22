using UnityEngine;

public class StageController : MonoBehaviour
{
    [Header("Doors")]
    [SerializeField] private DoorBehavior entryDoor;
    [SerializeField] private DoorBehavior exitDoor;

    [Header("Stage Components")] 
    public GameObject museumDoor;
    public GameObject museum;
    
    
    [Header("Stage Prefab")]
    [SerializeField] private GameObject endAreaPrefab;
    
    [Header("Pivots")]
    [SerializeField] private GameObject pivotNext;
    
    private GameObject _lastInstance;
    
    private TransitionTrigger _transitionTrigger;
    private OffloadTrigger _offloadTrigger;
    
    private void Start()
    {

        _transitionTrigger = GetComponentInChildren<TransitionTrigger>();
        _transitionTrigger?.SetStageController(this);
        _offloadTrigger = GetComponentInChildren<OffloadTrigger>();
        _offloadTrigger?.SetStageController(this);
        
        entryDoor.allowInteract = true;
        exitDoor.allowInteract = true;
    }
    
    private void SetLastInstance(GameObject instance)
    {
        _lastInstance = instance;
    }
    
    public void OnStateTransitionTriggered()
    {
        if (LevelManager.Instance.IsLevelComplete)
        {
            Vector3 spawnPos = pivotNext.transform.position;
            spawnPos.y += 1;
            Instantiate(endAreaPrefab, spawnPos, pivotNext.transform.rotation);
        }
        else
        {
            Vector3 spawnPos = pivotNext.transform.position;
            spawnPos.y += 1;
            GameObject newInstance = Instantiate(LevelManager.Instance.stagePrefab, spawnPos, pivotNext.transform.rotation);
            newInstance.GetComponent<StageController>().SetLastInstance(this.gameObject);
            LevelManager.Instance.MoveObjects(spawnPos - transform.position);
            LevelManager.Instance.SetNextStage(newInstance.GetComponent<StageController>());
            
            exitDoor.Close();
            exitDoor.allowInteract = false;
        }
        
        // end the effect of the active anomaly, if any
        if (LevelManager.Instance.GetActiveAnomaly() != null)
        {
            LevelManager.Instance.GetActiveAnomaly().Deactivate();
        }
    }
    
    public void OnOffloadTriggered()
    {
        LevelManager.Instance.AdvanceStage();
        entryDoor.Close();
        entryDoor.allowInteract = false;
        
        // check if the anomaly in last instance is resolved correctly
        if (_lastInstance == null)
        {
            return;
        }
        
        if (LevelManager.Instance.WasButtonPressed ^ LevelManager.Instance.GetActiveAnomaly() != null)
        {
            if (LevelManager.Instance.WasButtonPressed)
            {
                Debug.Log("Button was pressed, but no anomaly was present. Resetting level progress.");
            } else
            {
                Debug.Log("Anomaly was present, but button was not pressed. Resetting level progress.");
            }
            
            LevelManager.Instance.ResetLevelProgress();
        }
        else
        {
            LevelManager.Instance.GetActiveAnomaly()?.SetSolved(true);
        }
        
        LevelManager.Instance.GetActiveAnomaly()?.Deactivate();
        
        // based on level config, determine if the next stage should contain an anomaly
        var anomalyGenerationCheck = Random.Range(0, 100);
        if (anomalyGenerationCheck <= LevelManager.Instance.anomalyGenerationChance)
        {
            var anomaly = LevelManager.Instance.GenerateAnomaly();
            if (anomaly == null)
            {
                Debug.LogWarning("Anomaly generation failed.");
                LevelManager.Instance.ClearAnomaly();
                return;
            }
            else
            {
                Debug.Log($"Anomaly generated: {anomaly.GetAnomalyName()}");
                anomaly.Activate();
            }
        }
        else
        {
            Debug.Log("No anomaly generated.");
            LevelManager.Instance.ClearAnomaly();
        }
        
        LevelManager.Instance.SetButtonPressed(false);
        Destroy(_lastInstance);
    }
}
