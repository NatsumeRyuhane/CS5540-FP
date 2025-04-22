using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (LevelManager.Instance.isLastLevel)
            {
                FinalSequenceController.Instance.Begin();
            }
            else
            {
                LevelManager.Instance.LevelComplete();    
            }
            
            Destroy(gameObject);
        }
    }
}
