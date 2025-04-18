using UnityEngine;
using UnityEngine.AI;

public class NavMeshDebugger : MonoBehaviour
{
    public NavMeshAgent agent;

    void Start()
    {
        // Check if agent is on NavMesh
        if (!agent.isOnNavMesh)
        {
            // Try to find nearest NavMesh position
            NavMeshHit hit;
            if (NavMesh.SamplePosition(agent.transform.position, out hit, 1000f, NavMesh.AllAreas))
            {
                Debug.Log($"Nearest NavMesh point: {hit.position}, distance: {hit.distance}");
            }
            else
            {
                Debug.LogError("No NavMesh found within 10 units!");
            }
        }
    }

    void Update()
    {
        // Visualize agent path
        if (agent.hasPath)
        {
            Debug.DrawLine(agent.transform.position, agent.destination, Color.green);
        }
    }
}