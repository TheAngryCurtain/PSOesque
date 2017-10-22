using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO this all needs to be put into it's own class

public interface IInteractable
{
    void Highlight();
    void Unhighlight();
    void Interact();
}

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private WorldSpaceCallout m_InteractCallout;
    [SerializeField] private Sprite m_CalloutSprite;
    [SerializeField] private string m_CalloutText;

    private int m_RoomID;
    private bool m_Used = false;

    private void Awake()
    {
        // TODO this should be done a better way
        m_InteractCallout.Setup(m_CalloutSprite, m_CalloutText);
    }

    public void SetRoomID(int id)
    {
        m_RoomID = id;
    }

    public void Highlight()
    {
        if (!m_Used)
        {
            m_InteractCallout.Show(true);
        }
    }

    public void Unhighlight()
    {
        m_InteractCallout.Show(false);
    }

    public void Interact()
    {
        if (!m_Used)
        {
            VSEventManager.Instance.TriggerEvent(new GameEvents.DoorSwitchPressedEvent(m_RoomID));
            m_Used = true;

            Unhighlight();
        }
    }
}
