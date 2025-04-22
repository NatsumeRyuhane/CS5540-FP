using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowPlayer : MonoBehaviour
{
    
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float minDistanceToPlayer = 1f;
    [SerializeField] private float updatePathInterval = 0.2f;
    
    private NavMeshAgent agent;
    private float timer;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= updatePathInterval)
        {
            timer = 0;
            UpdateDestination();
        }
    }
    
    void UpdateDestination()
    {
        if (playerTransform == null || agent == null) return;
        
        // First check if the agent is on a NavMesh
        if (!agent.isOnNavMesh)
        {
            return;
        }
        
        var targetPosition = playerTransform.position;
        targetPosition.y = transform.position.y; // Keep the same height
        
        // Calculate horizontal distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, targetPosition);
        
        if (distanceToPlayer > minDistanceToPlayer)
        {
            // Set destination to player's position
            agent.SetDestination(targetPosition);
            agent.isStopped = false;
        }
        else
        {
            // Stop the agent when close enough
            agent.isStopped = true;
        }
    }
}