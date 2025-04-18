using UnityEngine;

public class StageController : MonoBehaviour
{
    [Header("Doors")]
    [SerializeField] private GameObject entryDoor;
    [SerializeField] private GameObject exitDoor;
    
    [Header("Stage Prefab")]
    [SerializeField] private GameObject endAreaPrefab;
    
    [Header("Pivots")]
    [SerializeField] private GameObject pivotNext;
    
    private LevelManager _levelManager;
    private GameObject _lastInstance;
    
    private TransitionTrigger _transitionTrigger;
    private OffloadTrigger _offloadTrigger;
    
    private bool _containsAnomaly;
    
    private void Start()
    {
        _levelManager = FindFirstObjectByType<LevelManager>();
        _transitionTrigger = GetComponentInChildren<TransitionTrigger>();
        _transitionTrigger?.SetStageController(this);
        _offloadTrigger = GetComponentInChildren<OffloadTrigger>();
        _offloadTrigger?.SetStageController(this);
        
        entryDoor.GetComponent<DoorBehavior>().allowInteract = true;
        exitDoor.GetComponent<DoorBehavior>().allowInteract = true;
    }
    
    public void SetLastInstance(GameObject instance)
    {
        _lastInstance = instance;
    }
    
    public void SetContainsAnomaly(bool containsAnomaly)
    {
        _containsAnomaly = containsAnomaly;
    }
    
    public void OnStateTransitionTriggered()
    {
        if (_levelManager.IsLevelComplete)
        {
            Vector3 spawnPos = pivotNext.transform.position;
            spawnPos.y += 1;
            Instantiate(endAreaPrefab, spawnPos, pivotNext.transform.rotation);
        }
        else
        {
            Vector3 spawnPos = pivotNext.transform.position;
            spawnPos.y += 1;
            GameObject newInstance = Instantiate(_levelManager.stagePrefab, spawnPos, pivotNext.transform.rotation);
            newInstance.GetComponent<StageController>().SetLastInstance(this.gameObject);
            _levelManager.MoveObjects(spawnPos - transform.position);
            
            exitDoor.GetComponent<DoorBehavior>().Close();
            exitDoor.GetComponent<DoorBehavior>().allowInteract = false;
        }
    }
    
    public void OnOffloadTriggered()
    {
        entryDoor.GetComponent<DoorBehavior>().Close();
        entryDoor.GetComponent<DoorBehavior>().allowInteract = false;
        
        // check if the anomaly in last instance is resolved correctly
        if (_lastInstance == null)
        {
            return;
        }
        
        var lastInstanceContainsAnomaly = _lastInstance.GetComponent<StageController>()._containsAnomaly;
        var wasButtonPressed = _levelManager.WasButtonPressed;
        if (wasButtonPressed ^ lastInstanceContainsAnomaly)
        {
            // do nothing
        }
        else
        {
            _levelManager.ResetLevelProgress();
        }
        
        
        Destroy(_lastInstance, 2f);
    }
}
