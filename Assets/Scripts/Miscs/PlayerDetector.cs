using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    // Reference to the elevator door controller script
    public ElevatorDoorController doorController;
    
    // Optional: Set a delay time for automatic door closing
    public float autoCloseDelay = 3.0f;
    
    // Track whether the player is inside the trigger zone
    private bool playerInTriggerZone = false;
    
    // Called when another collider enters this trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered elevator area");
            playerInTriggerZone = true;
            
            // Open the elevator doors
            if (doorController != null)
            {
                doorController.OpenDoors();
            }
        }
    }
    
    // Called when another collider exits this trigger zone
    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left elevator area");
            playerInTriggerZone = false;
            
            // Delay closing the elevator doors
            if (doorController != null)
            {
                Invoke("CloseDoors", autoCloseDelay);
            }
        }
    }
    
    // Method to close doors after specified delay
    private void CloseDoors()
    {
        // Only close doors if player is still outside the trigger zone
        if (!playerInTriggerZone && doorController != null)
        {
            doorController.CloseDoors();
        }
    }
}