using UnityEngine;
using System.Collections;

/// <summary>
/// Controls smooth horizontal movement of a wall or platform along a curve
/// </summary>
public class WallBezierMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Distance the wall moves to the right")]
    public float moveDistance = 5f;
    
    [Tooltip("Time in seconds for one-way movement")]
    public float moveDuration = 2f;
    
    [Tooltip("Time in seconds to wait at furthest point")]
    public float waitTime = 1f;
    
    [Tooltip("Whether to automatically loop the animation")]
    public bool loopAnimation = true;
    
    // Store initial position
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    
    // Is animation currently playing
    private bool isMoving = false;
    
    void Start()
    {
        // Save initial position
        initialPosition = transform.position;
        // Calculate target position (move right)
        targetPosition = initialPosition + new Vector3(moveDistance, 0, 0);
        
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
        
        while (true)
        {
            // Move right
            yield return StartCoroutine(MoveWithEasing(initialPosition, targetPosition, moveDuration));
            
            // Wait at rightmost position
            yield return new WaitForSeconds(waitTime);
            
            // Move left (return)
            yield return StartCoroutine(MoveWithEasing(targetPosition, initialPosition, moveDuration));
            
            // Wait at initial position
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
            // SmoothStep formula: 3t² - 2t³, provides smooth acceleration and deceleration
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
}