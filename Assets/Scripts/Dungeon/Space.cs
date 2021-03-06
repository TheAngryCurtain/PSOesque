﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField] private Transform m_FloorCenter;
    public Transform FloorCenter { get { return m_FloorCenter; } }

    [SerializeField] protected Transform[] m_ConnectorTransforms = new Transform[4];

    protected List<Connector> m_Connectors;
    public List<Connector> Connectors { get { return m_Connectors; } }

    private Renderer[] m_ObjectRenderers;

    protected virtual void Awake()
    {
        m_Connectors = new List<Connector>();
        for (int i = 0; i < m_ConnectorTransforms.Length; i++)
        {
            if (m_ConnectorTransforms[i] != null)
            {
                m_Connectors.Add(new Connector(i, m_ConnectorTransforms[i].gameObject, false));
            }
        }

        m_ObjectRenderers = GetComponentsInChildren<Renderer>();
        Show(false);
    }

    public virtual void Show(bool show)
    {
        for (int i = 0; i < m_ObjectRenderers.Length; i++)
        {
            for (int j = 0; j < m_ObjectRenderers[i].materials.Length; j++)
            {
                m_ObjectRenderers[i].materials[j].color = (show ? Color.white : Color.black);
            }

            m_ObjectRenderers[i].receiveShadows = show;
            m_ObjectRenderers[i].shadowCastingMode = (show ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off);
        }
    }

    public Vector3 GetConnectorToCenter(Connector c)
    {
        return m_FloorCenter.position - c.WorldPosition;
    }

    public Connector GetFirstAvailableConnector()
    {
        int totalConnectors = m_Connectors.Count;
        Connector first = m_Connectors[0];

        for (int i = 0; i < totalConnectors; i++)
        {
            if (m_Connectors[i].Available)
            {
                first = m_Connectors[i];
                break;
            }
        }

        return first;
    }

    public Connector GetRandomConnector(int previousSlot, System.Random rng)
    {
        Connector c = null;
        List<Connector> unused = new List<Connector>(m_Connectors.Count);
        for (int i = 0; i < m_Connectors.Count; i++)
        {
            if (m_Connectors[i].Available)
            {
                if (i != previousSlot || m_Connectors.Count == 1) // starting room has only one connector
                {
                    unused.Add(m_Connectors[i]);
                }
            }
        }

        if (unused.Count > 0)
        {
            int random = rng.Next(unused.Count);
            c = unused[random];
        }

        return c;
    }

    public void OnFloorCollsionEnter(Collision collision)
    {
        Show(true);

        Room r = this.gameObject.GetComponent<Room>();
        if (r != null && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            VSEventManager.Instance.TriggerEvent(new GameEvents.PlayerEnteredRoomEvent(r.RoomID, collision.transform));
        }
    }

    public void OnFloorCollisionExit(Collision collision)
    {
        Room r = this.gameObject.GetComponent<Room>();
        if (r != null && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            VSEventManager.Instance.TriggerEvent(new GameEvents.PlayerExitedRoomEvent(r.RoomID));
        }
    }
}
