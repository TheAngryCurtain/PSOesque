using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData m_ItemData;
    public ItemData Data { get { return m_ItemData; } }

    [SerializeField] private Sprite m_CalloutSprite;
    [SerializeField] private string m_CalloutText;

    public void SetData(ItemData data)
    {
        m_ItemData = data;
    }

    public void Highlight(WorldSpaceCallout callout)
    {
        callout.Setup(m_CalloutSprite, m_CalloutText);
        callout.Show(true);
    }

    public void Unhighlight(WorldSpaceCallout callout)
    {
        callout.Show(false);
    }

    public void Interact(WorldSpaceCallout callout)
    {
        // TODO
        // transfer item to inventory
        // destroy item object
        Debug.LogFormat("Picked up {0}", m_ItemData.m_ItemName);

        Unhighlight(callout);
    }
}
