using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO this all needs to be put into it's own class

public interface IInteractable
{
    void Highlight(WorldSpaceCallout callout);
    void Unhighlight(WorldSpaceCallout callout);
    void Interact(WorldSpaceCallout callout);
}

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite m_CalloutSprite;
    [SerializeField] private string m_CalloutText;

    private int m_RoomID;
    private bool m_Used = false;

    public void SetRoomID(int id)
    {
        m_RoomID = id;
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
            VSEventManager.Instance.TriggerEvent(new GameEvents.DoorSwitchPressedEvent(m_RoomID));
            m_Used = true;

            Unhighlight(callout);
        }
    }
}
