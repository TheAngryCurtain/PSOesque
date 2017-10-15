using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector
{
    public int Slot;
    public bool Available;
    public Vector3 WorldPosition;
    public Space ConnectedSpace;
    public Space NextSpace;
    public bool IsOnMainPath = false;

    public Vector3 Forward { get { return m_Obj.transform.forward; } }

    private GameObject m_Obj;

    public Connector(int slot, GameObject obj, Space connected, bool mainPath, bool available = true)
    {
        Slot = slot;
        WorldPosition = obj.transform.position;
        ConnectedSpace = connected;
        IsOnMainPath = mainPath;
        Available = available;

        // may not need this
        m_Obj = obj;
    }

    public void SetNextSpace(Space s)
    {
        NextSpace = s;
        Available = false;
    }
}

public class Space : MonoBehaviour
{
    [SerializeField] private Transform m_FloorCenter;
    public Transform FloorCenter { get { return m_FloorCenter; } }

    [SerializeField] private Transform[] m_ConnectorTransforms = new Transform[4];

    protected List<Connector> m_Connectors;
    public List<Connector> Connectors { get { return m_Connectors; } }

    protected Connector m_LastUsedConnector;

    public int GetTotalAvailableConnectors()
    {
        int count = 0;
        for (int i = 0; i < m_Connectors.Count; i++)
        {
            if (m_Connectors[i].Available)
            {
                count += 1;
            }
        }

        return count;
    }

    private void Awake()
    {
        m_Connectors = new List<Connector>();
        for (int i = 0; i < m_ConnectorTransforms.Length; i++)
        {
            if (m_ConnectorTransforms[i] != null)
            {
                m_Connectors.Add(new Connector(i, m_ConnectorTransforms[i].gameObject, this, false));
            }
        }
    }

    public Vector3 GetConnectorToCenter(Connector c)
    {
        return m_FloorCenter.position - c.WorldPosition;
    }

    public Vector3 GetCenterToConnector(Connector c)
    {
        return c.WorldPosition - m_FloorCenter.position;
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

        m_LastUsedConnector = first;
        return first;
    }

    public Connector GetRandomConnector(int previousSlot, System.Random rng)
    {
        List<Connector> unused = new List<Connector>(m_Connectors.Count);
        for (int i = 0; i < m_Connectors.Count; i++)
        {
            if (m_Connectors[i].Available)
            {
                if (i != previousSlot)
                {
                    unused.Add(m_Connectors[i]);
                }
            }
        }

        if (unused.Count > 0)
        {
            int random = rng.Next(unused.Count);
            m_LastUsedConnector = unused[random];
        }
        else
        {
            m_LastUsedConnector = null;
        }

        return m_LastUsedConnector;
    }

    public Connector GetLastUsedOrFirstConnector()
    {
        if (m_LastUsedConnector == null)
        {
            return GetFirstAvailableConnector();
        }

        return m_LastUsedConnector;
    }

    public Connector GetLastAvailableConnector()
    {
        int totalConnectors = m_Connectors.Count;
        Connector last = m_Connectors[totalConnectors - 1];

        for (int i = totalConnectors - 1; i > 0; i--)
        {
            if (m_Connectors[i].Available)
            {
                last = m_Connectors[i];
                break;
            }
        }

        m_LastUsedConnector = last;
        return last;
    }

    // for debug
    public Connector GetSecondLastConnector()
    {
        int totalConnectors = m_Connectors.Count;
        Connector last;
        if (totalConnectors > 1)
        {
            last = m_Connectors[totalConnectors - 2];
        }
        else
        {
            last = m_Connectors[0];
        }

        m_LastUsedConnector = last;
        return last;
    }
}
