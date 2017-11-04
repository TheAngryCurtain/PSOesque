using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : NonPlayableCharacter
{
    [SerializeField] private Vector3 m_Offset;

    private Transform m_PlayerTransform;

    protected override void Awake()
    {
        base.Awake();

        VSEventManager.Instance.AddListener<GameEvents.PlayerSpawnedEvent>(OnPlayerSpawned);
    }

    protected virtual void FixedUpdate()
    {
        if (m_PlayerTransform != null)
        {
            SetDestination(m_PlayerTransform.position - m_PlayerTransform.rotation * m_Offset);
            MoveToPosition();
        }
    }

    private void OnPlayerSpawned(GameEvents.PlayerSpawnedEvent e)
    {
        m_PlayerTransform = e.PlayerObj.transform;
    }
}
