using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [SerializeField] private NavMeshAgent m_Agent;

    protected virtual void Awake()
    {
        
    }

    protected virtual void OnDestroy()
    {
        
    }

    protected virtual void SetDestination(Vector3 position)
    {
        m_Agent.isStopped = false;
        m_Agent.SetDestination(position);

        //if (m_Agent.pathStatus == NavMeshPathStatus.PathPartial)
        //{
            
        //}
    }

    protected virtual void Stop()
    {
        m_Agent.isStopped = true;
    }
}
