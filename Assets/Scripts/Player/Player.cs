using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayableCharacter
{
    protected override void Awake()
    {
        base.Awake();

        SetActive(true);
        VSEventManager.Instance.TriggerEvent(new GameEvents.PlayerSpawnedEvent(this.gameObject));
    }

    public void Init(int playerId)
    {
        m_PlayerId = playerId;
    }
}
