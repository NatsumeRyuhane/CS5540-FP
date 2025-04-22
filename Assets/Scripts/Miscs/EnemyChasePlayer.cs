using UnityEngine;

public class EnemyChasePlayer : MonoBehaviour
{
    [Header("Chase Settings")]
    [Tooltip("Enemy movement speed")]
    public float moveSpeed = 3.0f;
    
    [Tooltip("Stop moving when closer than this distance to player")]
    public float minDistanceToPlayer = 0.2f;
    
    [Tooltip("Acceleration factor - higher values mean faster acceleration")]
    public float accelerationFactor = 2.0f;
    
    [Tooltip("Maximum speed multiplier - limits the top speed")]
    public float maxSpeedMultiplier = 1.5f;
    
    [Header("References")]
    [Tooltip("Player's Transform component, if empty will auto-find object tagged 'Player'")]
    public Transform playerTransform;
    
    // Current speed
    private float currentSpeed = 0f;
    // Whether the enemy is currently chasing
    private bool isChasing = true;
    // Latest movement direction
    private Vector3 moveDirection;
    
    void Start()
    {
        // If player not specified, automatically find object tagged "Player"
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Cannot find player object. Make sure it's tagged 'Player' or manually assign playerTransform");
                isChasing = false;
            }
        }
    }
    
    void Update()
    {
        if (!isChasing || playerTransform == null)
            return;
            
        // Calculate distance to player (only considering horizontal X and Z)
        Vector3 playerPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        Vector3 directionToPlayer = playerPosition - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        // If player is too close, stop moving
        if (distanceToPlayer < minDistanceToPlayer)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * accelerationFactor);
            
            // Optional: Make enemy face the player
            if (directionToPlayer != Vector3.zero)
            {
                transform.forward = directionToPlayer.normalized;
            }
            
            return;
        }
        
        // Calculate movement direction (ignoring Y axis)
        moveDirection = directionToPlayer.normalized;
        
        // Smoothly increase speed based on distance
        float targetSpeed = moveSpeed;
        // Optional: Adjust speed based on distance to player
        // float distanceFactor = Mathf.Clamp01(distanceToPlayer / 10f); // 10f is reference distance
        // targetSpeed = moveSpeed * (1f + distanceFactor * (maxSpeedMultiplier - 1f));
        
        // Smoothly adjust current speed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationFactor);
        
        // Move the enemy
        transform.position += moveDirection * currentSpeed * Time.deltaTime;
        
        // Make enemy face movement direction
        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }
    }
    
    // Public method: Start chasing
    public void StartChasing()
    {
        isChasing = true;
    }
    
    // Public method: Stop chasing
    public void StopChasing()
    {
        isChasing = false;
        currentSpeed = 0f;
    }
    
    // Optional: Display debug lines in Scene view
    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            // Draw a line to the player
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
            
            // Draw the minimum distance range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minDistanceToPlayer);
        }
    }
}