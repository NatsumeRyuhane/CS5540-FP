using UnityEngine;

public class GameEndTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FinalSequenceController.Instance.DoEndGameSequence();
            
            Destroy(gameObject);
        }
    }
}
