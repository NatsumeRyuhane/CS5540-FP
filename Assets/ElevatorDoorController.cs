using UnityEngine;
using DG.Tweening;

public class ElevatorDoorController : MonoBehaviour
{
    [Header("Door References")]
    public Transform leftDoor;  // Reference to Door-Left.001_0
    public Transform rightDoor; // Reference to Door-Right.001_1
    
    [Header("Door Movement Settings")]
    public float openDistance = 1.0f; // How far each door should slide
    public float animationDuration = 1.0f; // Duration of door opening/closing
    public float doorDelay = 0.5f; // Delay before doors start moving
    public Ease easeType = Ease.InOutQuad; // DOTween easing function
    
    // Door state tracking
    private bool doorsOpen = false;
    private Vector3 leftDoorClosedPosition;
    private Vector3 rightDoorClosedPosition;
    
    
    private void Start()
    {
        // Store initial positions of doors
        if (leftDoor != null)
            leftDoorClosedPosition = leftDoor.localPosition;
        if (rightDoor != null)
            rightDoorClosedPosition = rightDoor.localPosition;
            
        // Make sure DOTween is initialized
        DOTween.Init();
    }
    private void Update()
    {
        // Press to test the door
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleDoors();
        }
    }
    // Call this method to toggle the doors
    public void ToggleDoors()
    {
        if (doorsOpen)
            CloseDoors();
        else
            OpenDoors();
    }
    
    // Opens both doors
    public void OpenDoors()
    {
        if (doorsOpen) return;
        
        // Kill any ongoing tweens on the doors
        if (leftDoor != null) leftDoor.DOKill();
        if (rightDoor != null) rightDoor.DOKill();
        
        // Calculate target positions
        Vector3 leftDoorOpenPosition = leftDoorClosedPosition + new Vector3(-openDistance, 0, 0);
        Vector3 rightDoorOpenPosition = rightDoorClosedPosition + new Vector3(openDistance, 0, 0);
        
        // Animate left door
        if (leftDoor != null)
        {
            leftDoor.DOLocalMove(leftDoorOpenPosition, animationDuration)
                .SetDelay(doorDelay)
                .SetEase(easeType);
        }
        
        // Animate right door
        if (rightDoor != null)
        {
            rightDoor.DOLocalMove(rightDoorOpenPosition, animationDuration)
                .SetDelay(doorDelay)
                .SetEase(easeType);
        }
        
        doorsOpen = true;
    }
    
    // Closes both doors
    public void CloseDoors()
    {
        if (!doorsOpen) return;
        
        // Kill any ongoing tweens on the doors
        if (leftDoor != null) leftDoor.DOKill();
        if (rightDoor != null) rightDoor.DOKill();
        
        // Animate left door
        if (leftDoor != null)
        {
            leftDoor.DOLocalMove(leftDoorClosedPosition, animationDuration)
                .SetDelay(doorDelay)
                .SetEase(easeType);
        }
        
        // Animate right door
        if (rightDoor != null)
        {
            rightDoor.DOLocalMove(rightDoorClosedPosition, animationDuration)
                .SetDelay(doorDelay)
                .SetEase(easeType);
        }
        
        doorsOpen = false;
    }
    
    // Optional: Clean up tweens when the object is destroyed
    private void OnDestroy()
    {
        if (leftDoor != null) leftDoor.DOKill();
        if (rightDoor != null) rightDoor.DOKill();
    }
}