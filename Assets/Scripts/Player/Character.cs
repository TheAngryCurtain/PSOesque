using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    //[SerializeField] private NavMeshAgent m_Agent;
    [SerializeField] protected Transform m_Transform;
    [SerializeField] protected Rigidbody m_Rigidbody;

    [SerializeField] protected float m_MoveSpeed = 3f;
    [SerializeField] protected float m_RotateSpeed = 3f;

    [SerializeField] protected Transform[] m_EquipTransforms = new Transform[6];

	protected float m_ModifiableMoveSpeed = 0f;

    protected virtual void Awake()
    {
		m_ModifiableMoveSpeed = m_MoveSpeed;
    }

    protected virtual void OnDestroy()
    {
        
    }

    //protected virtual void SetDestination(Vector3 position)
    //{
    //    m_Agent.isStopped = false;
    //    m_Agent.SetDestination(position);

    //    //if (m_Agent.pathStatus == NavMeshPathStatus.PathPartial)
    //    //{
            
    //    //}
    //}

    //protected virtual void Stop()
    //{
    //    m_Agent.isStopped = true;
    //}

	protected virtual void Move(Vector3 direction, float distanceFromObjective = 0f, float maxDistance = 0f)
    {
		if (maxDistance > 0f)
		{
			// adjust speed based on distance to target (e.g. full speed when far away, slow down to a stop as you reach dest)
			float distRatio = distanceFromObjective / maxDistance;
			m_ModifiableMoveSpeed = Mathf.Clamp(distRatio, 0.1f, m_MoveSpeed);
		}

		m_Rigidbody.MovePosition(m_Transform.position + direction * m_ModifiableMoveSpeed * Time.fixedDeltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, rotation, Time.deltaTime * m_RotateSpeed));
        }
    }
}
