using UnityEngine;

/// <summary>
/// Detects player presence and can trigger actions when player enters or exits a defined area
/// </summary>
public class PlayerDetector : MonoBehaviour
{
    [Header("Response Settings")]
    [Tooltip("Object to notify when player is detected")]
    public MonoBehaviour targetScript;
    
    [Tooltip("Name of the method to call when player enters")]
    public string onEnterMethodName = "OnPlayerEnter";
    
    [Tooltip("Name of the method to call when player exits")]
    public string onExitMethodName = "OnPlayerExit";
    
    [Tooltip("Time delay before triggering exit event")]
    public float exitDelay = 0.5f;
    
    [Header("Debug Options")]
    [Tooltip("Enable console logging of events")]
    public bool enableDebugLogs = false;
    
    // Track whether the player is inside the trigger zone
    private bool playerInTriggerZone = false;
    
    // Called when another collider enters this trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            if (enableDebugLogs)
                Debug.Log("Player entered detection area");
            
            playerInTriggerZone = true;
            
            // Notify target script if available
            if (targetScript != null)
            {
                targetScript.SendMessage(onEnterMethodName, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    // Called when another collider exits this trigger zone
    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            if (enableDebugLogs)
                Debug.Log("Player left detection area");
            
            playerInTriggerZone = false;
            
            // Delay exit notification
            if (targetScript != null && !string.IsNullOrEmpty(onExitMethodName))
            {
                Invoke("NotifyExit", exitDelay);
            }
        }
    }
    
    // Method to notify of player exit after delay
    private void NotifyExit()
    {
        // Only notify if player is still outside the trigger zone
        if (!playerInTriggerZone && targetScript != null)
        {
            targetScript.SendMessage(onExitMethodName, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    // Check if player is currently detected
    public bool IsPlayerDetected()
    {
        return playerInTriggerZone;
    }
}