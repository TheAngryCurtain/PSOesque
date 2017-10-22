using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDoor : Door
{
    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.DoorSwitchPressedEvent>(OnSwitchPressed);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameEvents.DoorSwitchPressedEvent>(OnSwitchPressed);
    }

    private void OnSwitchPressed(GameEvents.DoorSwitchPressedEvent e)
    {
        if (e.RoomID == m_RoomID)
        {
            UnlockDoor();
        }
    }
}
