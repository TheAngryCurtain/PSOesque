using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float m_AttackRange = 1f;
    [SerializeField] private float m_AttackCooldownDelay = 2f;

    private Transform m_Target;
    private float m_AttackRangeSqr;
    private float m_CurrentAttackTime = 0;
    private int m_HomeRoomID;
    private Vector3 m_SpawnPos;

    protected override void Awake()
    {
        base.Awake();

        m_SpawnPos = m_Transform.position;

        VSEventManager.Instance.AddListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
        VSEventManager.Instance.AddListener<GameEvents.PlayerExitedRoomEvent>(OnPlayerExitedRoom);

        m_AttackRangeSqr = m_AttackRange * m_AttackRange;
    }

    protected override void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
        VSEventManager.Instance.RemoveListener<GameEvents.PlayerExitedRoomEvent>(OnPlayerExitedRoom);
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

    // TODO
    // figure out how to handle attacking/damaging. Perhaps an IDamagable interface
    // when an enemy is destory, fire the enemydefeated event
    // create another type of door that listens to that event and can unlock when they are all defeated
    // will likely need to fire a setup event to let that door know how many enemies were spawned
    // will likey need to spawn that door along with the spawner
}
