using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : NonPlayableCharacter
{
    [SerializeField] private Vector3 m_Offset;

    private InventoryItem m_CompanionData;
    public InventoryItem CompanionData { get { return m_CompanionData; } }

    private Transform m_PlayerTransform;

    public void SetData(InventoryItem data)
    {
        m_CompanionData = data;
    }

    /*
     * TODO
     * 
     * will eventually need to make companions equippable.
     * When they are equipped, they'll need to spawn this prefab (similarly, destroy it when unequipped)
     */

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
