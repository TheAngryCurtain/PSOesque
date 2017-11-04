using UnityEngine;

public class NonPlayableCharacter : Character
{
    //public abstract class State
    //{
    //    public abstract void Enter(Character c);
    //    public abstract State Update(Character c);
    //    public abstract void Exit(Character c);
    //}

    //protected State m_PreviousState;
    //protected State m_CurrentState;

    //protected virtual void Update()
    //{
    //    if (m_CurrentState != null)
    //    {
    //        if (m_PreviousState != null && m_CurrentState != m_PreviousState)
    //        {
    //            m_PreviousState.Exit(this);
    //            m_CurrentState.Enter(this);
    //        }

    //        m_PreviousState = m_CurrentState;
    //        m_CurrentState = m_CurrentState.Update(this);
    //    }
    //}

    [SerializeField] protected float m_AttackRange = 1f;
    [SerializeField] protected float m_AttackCooldownDelay = 1f;
    [SerializeField] protected float m_MaxAttackAngle = 45f;
    [SerializeField] protected float m_StoppingDistance = 1f;

    protected Transform m_Target;
    protected Vector3 m_Destination;

    protected float m_AttackRangeSqr;
    protected float m_StoppedDistSqr;
    protected float m_CurrentAttackTime = 0;
    protected bool m_Dead = false;

    protected override void Awake()
    {
        base.Awake();

        VSEventManager.Instance.AddListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
        VSEventManager.Instance.AddListener<GameEvents.PlayerExitedRoomEvent>(OnPlayerExitedRoom);

        m_AttackRangeSqr = m_AttackRange * m_AttackRange;
        m_StoppedDistSqr = m_StoppingDistance * m_StoppingDistance;
    }

    protected virtual void OnPlayerEnteredRoom(GameEvents.PlayerEnteredRoomEvent e)
    {
        
    }

    protected virtual void OnPlayerExitedRoom(GameEvents.PlayerExitedRoomEvent e)
    {
        
    }

    public void SetTarget(Transform t)
    {
        m_Target = t;
    }

    public void SetDestination(Vector3 position)
    {
        m_Destination = position;
    }

    public void MoveToPosition()
    {
        if (m_Destination != Vector3.zero)
        {
            Vector3 toDest = m_Destination - m_Transform.position;
            float sqrDistToDest = toDest.sqrMagnitude;

            if (sqrDistToDest < m_StoppedDistSqr)
            {
                // in range, stop
                return;
            }

            Move(toDest.normalized);
        }
    }

    public void FollowTarget()
    {
        if (m_Target != null)
        {
            bool canAttack = (Time.time - m_CurrentAttackTime > m_AttackCooldownDelay);
            if (canAttack)
            {
                Vector3 toTarget = m_Target.position - m_Transform.position;
                float sqrDistToTarget = toTarget.sqrMagnitude;
                float angle = Vector3.Angle(toTarget, m_Transform.forward);

                if (sqrDistToTarget < m_AttackRangeSqr && angle < m_MaxAttackAngle)
                {
                    Attack();
                    return;
                }

                Move(toTarget.normalized);
            }
        }
    }

    protected virtual void Attack()
    {
        m_CurrentAttackTime = Time.time;

        // TODO determine what would be a good way to do this
        Debug.LogFormat("{0} Attacks!", gameObject.name);
    }
}
