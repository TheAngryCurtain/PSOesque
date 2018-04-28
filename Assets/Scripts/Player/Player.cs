using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayableCharacter
{
    public int SaveSlot { get { return m_SaveSlot; } }

    protected override void Awake()
    {
        base.Awake();

        SetPlayerActive(true);
        VSEventManager.Instance.TriggerEvent(new GameEvents.PlayerSpawnedEvent(this.gameObject));
    }

    public void Init(int playerId, int saveSlot)
    {
        m_PlayerId = playerId;
        m_SaveSlot = saveSlot;
    }
}
