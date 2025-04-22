using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DementorBehavior : MonoBehaviour {
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LevelManager.Instance.LevelFail();
        }
    }
}