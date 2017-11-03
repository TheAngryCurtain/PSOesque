using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : RoomObject, IInteractable
{
    [SerializeField] private Sprite m_CalloutSprite;
    [SerializeField] private string m_CalloutText;

    private bool m_Used = false;
    private int m_DoorRoomID;

    public void SetDoorRoomID(int id)
    {
        m_DoorRoomID = id;
    }

    public void Highlight(WorldSpaceCallout callout)
    {
        if (!m_Used)
        {
            callout.Setup(m_CalloutSprite, m_CalloutText);
            callout.Show(true);
        }
    }

    public void Unhighlight(WorldSpaceCallout callout)
    {
        callout.Show(false);
    }

    public void Interact(WorldSpaceCallout callout)
    {
        if (!m_Used)
        {
            VSEventManager.Instance.TriggerEvent(new GameEvents.DoorSwitchPressedEvent(m_DoorRoomID));
            m_Used = true;

            Unhighlight(callout);
        }
    }
}
