using UnityEngine;
using System.Collections;

/// <summary>
/// Controls smooth vertical movement of a wall or platform
/// </summary>
public class WallVerticalMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Distance the wall moves upward")]
    public float moveDistance = 5f;
    
    [Tooltip("Time in seconds for one-way movement")]
    public float moveDuration = 2f;
    
    [Tooltip("Time in seconds to wait at highest/lowest point")]
    public float waitTime = 1f;
    
    [Tooltip("Whether to automatically loop the animation")]
    public bool loopAnimation = true;
    
    [Tooltip("Whether initial movement direction is upward")]
    public bool startMovingUp = true;
    
    // Store initial position
    private Vector3 initialPosition;
    private Vector3 topPosition;
    
    // Is animation currently playing
    private bool isMoving = false;
    
    void Start()
    {
        // Save initial position
        initialPosition = transform.position;
        // Calculate top position (move up)
        topPosition = initialPosition + new Vector3(0, moveDistance, 0);
        
        // Auto-start animation
        if (loopAnimation)
            StartMovement();
    }
    
    // Start movement cycle
    public void StartMovement()
    {
        if (!isMoving)
            StartCoroutine(MovementCycle());
    }
    
    IEnumerator MovementCycle()
    {
        isMoving = true;
        
        // Determine starting position and direction
        Vector3 startPos = startMovingUp ? initialPosition : topPosition;
        Vector3 endPos = startMovingUp ? topPosition : initialPosition;
        
        while (true)
        {
            // First movement (may be up or down, depending on startMovingUp)
            yield return StartCoroutine(MoveWithEasing(startPos, endPos, moveDuration));
            
            // Wait at endpoint
            yield return new WaitForSeconds(waitTime);
            
            // Return movement
            yield return StartCoroutine(MoveWithEasing(endPos, startPos, moveDuration));
            
            // Wait at other endpoint
            yield return new WaitForSeconds(waitTime);
            
            // Exit if not looping
            if (!loopAnimation)
                break;
        }
        
        isMoving = false;
    }
    
    // Smooth movement coroutine
    IEnumerator MoveWithEasing(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            // Calculate elapsed time ratio
            float t = elapsedTime / duration;
            
            // Apply smooth interpolation - SmoothStep for ease-in/out effect
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            // Update position
            transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            
            // Increment time
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure exact arrival at destination
        transform.position = endPos;
    }
    
    // Public method to manually trigger movement
    public void TriggerMove()
    {
        if (!isMoving)
            StartCoroutine(MovementCycle());
    }
    
    // Public method to manually stop movement
    public void StopMovement()
    {
        StopAllCoroutines();
        isMoving = false;
    }
    
    // Public method to set movement direction
    public void SetMovementDirection(bool moveUp)
    {
        startMovingUp = moveUp;
    }
}