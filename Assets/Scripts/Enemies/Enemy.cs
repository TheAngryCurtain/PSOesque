using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float m_AttackRange = 1f;
    [SerializeField] private float m_AttackCooldownDelay = 2f;
    [SerializeField] private Enums.eEnemyType m_Type;

    private Transform m_Target;
    private float m_AttackRangeSqr;
    private float m_CurrentAttackTime = 0;
    private int m_HomeRoomID;
    private Vector3 m_SpawnPos;
    private bool m_Dead = false;

    protected override void Awake()
    {
        base.Awake();

        m_SpawnPos = m_Transform.position;

        VSEventManager.Instance.AddListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
        VSEventManager.Instance.AddListener<GameEvents.PlayerExitedRoomEvent>(OnPlayerExitedRoom);

        m_AttackRangeSqr = m_AttackRange * m_AttackRange;
    }

    private void OnPlayerEnteredRoom(GameEvents.PlayerEnteredRoomEvent e)
    {
        if (e.RoomID == m_HomeRoomID)
        {
            m_Target = e.Player;
        }
    }

    private void OnPlayerExitedRoom(GameEvents.PlayerExitedRoomEvent e)
    {
        if (e.RoomID == m_HomeRoomID)
        {
            m_Target = null;
        }
    }

    public void SetTarget(Transform t)
    {
        m_Target = t;
    }

    public void SetHomeRoom(int id)
    {
        m_HomeRoomID = id;
    }

    protected virtual void FixedUpdate()
    {
        if (m_Dead) return;

        if (m_Target != null)
        {
            bool inRange = MoveWithinRange();
            if (inRange)
            {
                Attack();
            }
        }
        else
        {
            // player has left room
            MoveWithinRange();
        }
    }

    protected virtual bool MoveWithinRange()
    {
        Vector3 toTarget = Vector3.zero;
        if (m_Target != null)
        {
            toTarget = m_Target.position - m_Transform.position;
        }
        else
        {
            toTarget = m_SpawnPos - m_Transform.position;
        }

        float distToTargetSqr = toTarget.sqrMagnitude;

        bool inRange = (distToTargetSqr < m_AttackRangeSqr);
        if (!inRange)
        {
            Move(toTarget.normalized);
        }

        return inRange;
    }

    protected virtual void Attack()
    {
        if (Time.time - m_CurrentAttackTime > m_AttackCooldownDelay)
        {
            m_CurrentAttackTime = Time.time;

            // TODO determine what would be a good way to do this
            Debug.Log("Attack");
        }
    }

    protected virtual void Kill()
    {
        if (m_Dead) return;
        
        VSEventManager.Instance.RemoveListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
        VSEventManager.Instance.RemoveListener<GameEvents.PlayerExitedRoomEvent>(OnPlayerExitedRoom);

        m_Dead = true;

        // make them fall, for now
        m_Rigidbody.constraints = RigidbodyConstraints.None;

        // for fun, push them away and watch them roll!
        if (m_Target != null)
        {
            Vector3 forceDirection = m_Transform.position - m_Target.position;
            m_Rigidbody.AddForce(forceDirection * 25f, ForceMode.Impulse);
        }

        VSEventManager.Instance.TriggerEvent(new GameEvents.EnemyDefeatedEvent(m_HomeRoomID));
        VSEventManager.Instance.TriggerEvent(new GameEvents.RequestItemSpawnEvent(m_Transform.position, Enums.eItemSource.Enemy, m_Type, (Enums.eCrateType)(-1)));
    }

    // TODO
    // figure out how to handle attacking/damaging. Perhaps an IDamagable interface
    // when an enemy is destory, fire the enemydefeated event

    // FOR TESTING
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Kill();
        }
    }
}
