using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] protected ItemData m_ItemData;
    public ItemData Data { get { return m_ItemData; } }

    [SerializeField] private Transform m_Transform;
    [SerializeField] private Sprite m_CalloutSprite;
    [SerializeField] private string m_CalloutText;

    private Transform m_MarkerObj;

    public void SetData(ItemData data)
    {
        m_ItemData = data;
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
        // TODO
        // transfer item to inventory
        // destroy item object
        string output;
        if (m_ItemData.m_ItemType == Enums.eItemType.Money)
        {
            output = string.Format("${0}", m_ItemData.m_ItemValue);
        }
        else
        {
            output = m_ItemData.m_ItemName;
        }

        Debug.LogFormat("Picked up {0}", output);
        Unhighlight(callout);

        // clean up
        if (m_MarkerObj != null)
        {
            Destroy(m_MarkerObj.gameObject);
        }

        Destroy(this.gameObject);
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
