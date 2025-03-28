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
            // instantiate the loop prefab
            Vector3 spawnPos = pivotNext.transform.position;
            spawnPos.y += 1;
            GameObject newInstance = Instantiate(_levelManager.StagePrefab, spawnPos, pivotNext.transform.rotation);
            newInstance.GetComponent<StageController>().SetLastInstance(this.gameObject);
            _levelManager.MoveObjects(spawnPos - transform.position);
            exitDoor.GetComponent<DoorBehavior>().Close();
            exitDoor.GetComponent<DoorBehavior>().allowInteract = false;
        }
    }
    
    public void OnOffloadTriggered()
    {
        Destroy(_lastInstance, 2f);
        entryDoor.GetComponent<DoorBehavior>().Close();
        entryDoor.GetComponent<DoorBehavior>().allowInteract = false;
    }
}
