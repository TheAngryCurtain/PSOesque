﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NonPlayableCharacter
{
    [SerializeField] private Enums.eEnemyType m_Type;

    private int m_HomeRoomID;
    private Vector3 m_SpawnPos;

	private float m_WaitToReturnTime;
	private float m_CurrentWaitTime = 0f;

    protected override void Awake()
    {
        base.Awake();

        m_SpawnPos = m_Transform.position;
    }

    protected override void OnPlayerEnteredRoom(GameEvents.PlayerEnteredRoomEvent e)
    {
        base.OnPlayerEnteredRoom(e);

        if (e.RoomID == m_HomeRoomID)
        {
			m_CurrentWaitTime = 0f;
            m_Target = e.Player;
        }
    }

    protected override void OnPlayerExitedRoom(GameEvents.PlayerExitedRoomEvent e)
    {
        base.OnPlayerExitedRoom(e);

        if (e.RoomID == m_HomeRoomID)
        {
			m_WaitToReturnTime = UnityEngine.Random.Range(1f, 3f);
            m_Target = null;
        }
    }

    public void SetHomeRoom(int id)
    {
        m_HomeRoomID = id;
    }

    protected virtual void FixedUpdate()
    {
        if (!m_Dead)
        {
			if (m_Target != null)
			{
            	FollowTarget();
			}
			else
			{
				// no target, return to start position
				if (m_CurrentWaitTime < m_WaitToReturnTime)
				{
					m_CurrentWaitTime += Time.deltaTime;
				}
				else
				{
					if (m_Destination == Vector3.zero)
					{
						SetDestination(m_SpawnPos);
					}
					else
					{
						MoveToPosition();
					}
				}
			}

            Debug.DrawLine(m_Transform.position, m_Transform.position + m_Transform.forward * m_AttackRange, Color.red);
        }
    }

	protected virtual void Kill(int killingPlayerID)
    {
        if (m_Dead) return;

        VSEventManager.Instance.RemoveListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
        VSEventManager.Instance.RemoveListener<GameEvents.PlayerExitedRoomEvent>(OnPlayerExitedRoom);

        m_Dead = true;

        // death particles?
        VSEventManager.Instance.TriggerEvent(new GameEvents.AttackLandedEvent(this.transform.position));

        // make them fall, for now
        m_Rigidbody.constraints = RigidbodyConstraints.None;

        // for fun, push them away and watch them roll!
        if (m_Target != null)
        {
            Vector3 forceDirection = m_Transform.position - m_Target.position;
            m_Rigidbody.AddForce(forceDirection * 15f, ForceMode.Impulse);
        }

        VSEventManager.Instance.TriggerEvent(new GameEvents.EnemyDefeatedEvent(m_HomeRoomID));
        VSEventManager.Instance.TriggerEvent(new GameEvents.RequestItemSpawnEvent(m_Transform.position, Enums.eItemSource.Enemy, m_Type, (Enums.eCrateType)(-1)));

		// reward EXP
		if (killingPlayerID != -1)
		{
			// TODO need to look up the amount of EXP awarded based on species, difficulty, type, etc
			int rewardAmount = (10 * (int)m_Type) + 5;
			VSEventManager.Instance.TriggerEvent(new GameEvents.UpdatePlayerEXPEvent(killingPlayerID, rewardAmount));
		}
    }

    // TODO
    // figure out how to handle attacking/damaging. Perhaps an IDamagable interface

    // FOR TESTING
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // TODO maybe roll for a miss? or roll for a block depending on a stat
			int playerID = -1;
			Player killingPlayer = collision.gameObject.GetComponent<Player>();
			if (killingPlayer != null)
			{
				playerID = killingPlayer.PlayerID;
			}

			Kill(playerID);
        }
    }
}
