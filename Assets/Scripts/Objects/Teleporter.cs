using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : RoomObject, IInteractable
{
    [SerializeField] private Sprite m_CalloutSprite;
    [SerializeField] private string m_CalloutText;
    [SerializeField] private bool m_End = false;

    public void Highlight(WorldSpaceCallout callout)
    {
        callout.Setup(m_CalloutSprite, m_CalloutText);
        callout.Show(true);
    }

    public void Interact(WorldSpaceCallout callout)
    {
        string test = (m_End ? "to the next level!" : "back to hub world!");
        Debug.LogFormat("Teleport {0}", test);

        Unhighlight(callout);
    }

    public void Unhighlight(WorldSpaceCallout callout)
    {
        callout.Show(false);
    }
}
