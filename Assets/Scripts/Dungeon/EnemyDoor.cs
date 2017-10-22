using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoor : Door
{
    private int m_TotalEnemies;
    private int m_DefeatedEnemies = 0;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.SetupRoomDoorEvent>(OnRoomSetup);
        VSEventManager.Instance.AddListener<GameEvents.EnemyDefeatedEvent>(OnEnemyDefeated);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameEvents.SetupRoomDoorEvent>(OnRoomSetup);
        VSEventManager.Instance.RemoveListener<GameEvents.EnemyDefeatedEvent>(OnEnemyDefeated);
    }

    private void OnEnemyDefeated(GameEvents.EnemyDefeatedEvent e)
    {
        if (e.RoomID == m_RoomID)
        {
            m_DefeatedEnemies += 1;

            if (m_DefeatedEnemies == m_TotalEnemies)
            {
                UnlockDoor();
            }
        }
    }

    private void OnRoomSetup(GameEvents.SetupRoomDoorEvent e)
    {
        if (e.RoomID == m_RoomID)
        {
            m_TotalEnemies = e.EnemyCount;
        }
    }
}
