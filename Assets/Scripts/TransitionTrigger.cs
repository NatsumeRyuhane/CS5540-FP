using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    private StageController _stageController;
    
    public void SetStageController(StageController instance)
    {
        _stageController = instance;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _stageController.OnStateTransitionTriggered();
            Destroy(gameObject);
        }
    }
}
