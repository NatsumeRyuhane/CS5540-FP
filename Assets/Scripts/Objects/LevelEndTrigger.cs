using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.LevelComplete();
            Destroy(gameObject);
        }
    }
}
