using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentPlacer : MonoBehaviour
{
    private NavMeshAgent agent;
    public float maxPlacementDistance = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        PlaceAgentOnNavMesh();
    }

    void PlaceAgentOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, maxPlacementDistance, NavMesh.AllAreas))
        {
            Debug.Log("Placed agent on NavMesh at: " + hit.position);
            transform.position = hit.position;
            agent.Warp(hit.position); // Important to update the agent's internal position
        }
        else
        {
            Debug.LogError("Could not place agent on NavMesh within " + maxPlacementDistance + " units!");
        }
    }
}