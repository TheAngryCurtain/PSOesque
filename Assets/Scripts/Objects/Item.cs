using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] protected InventoryItem m_Item;
    public InventoryItem ItemData { get { return m_Item; } }

    [SerializeField] private Transform m_Transform;
    [SerializeField] private Sprite m_CalloutSprite;
    [SerializeField] private string m_CalloutText;

    private Transform m_MarkerObj;

    public void SetData(InventoryItem item)
    {
        m_Item = item;
    }

    public void AssignMarkerObject(Transform marker)
    {
        m_MarkerObj = marker;
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
        VSEventManager.Instance.TriggerEvent(new GameEvents.UpdateInventoryEvent(m_Item, 1, AddedItemResult));

        Unhighlight(callout);
    }

    private void AddedItemResult(bool success)
    {
        if (success)
        {
            // clean up
            if (m_MarkerObj != null)
            {
                Destroy(m_MarkerObj.gameObject);
            }

            Destroy(this.gameObject);
        }
        else
        {
            Debug.LogWarningFormat("Failed to pick up item");
        }
    }

    private void LateUpdate()
    {
        if (m_MarkerObj != null)
        {
            Vector3 pos = m_Transform.position;
            pos.y = 0.025f;

            // this should probably be done in it's own script, and not here
            m_MarkerObj.position = pos;
            m_MarkerObj.transform.Rotate(Vector3.up, Time.deltaTime * 25f);
        }
    }
}
