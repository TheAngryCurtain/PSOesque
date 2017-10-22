using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : Space
{
    public static int RoomCount = 0;

    [SerializeField] private GameObject[] m_MiddleWalls;

    public int RoomID;
    public bool DeadEnd = false;
    public bool ContainsImportantObject = false;

    protected override void Awake()
    {
        base.Awake();

        RoomID = RoomCount++;
    }

    public void MakeDeadEnd()
    {
        // remove all except 0, so you have a way in
        for (int i = m_Connectors.Count - 1; i > 0; i--)
        {
            m_Connectors.RemoveAt(i);
        }

        DeadEnd = true;
    }

    public void OpenWall(int index)
    {
        if (m_MiddleWalls.Length > 0 && m_MiddleWalls[index] != null)
        {
            m_MiddleWalls[index].SetActive(false);
        }
    }

    public Vector2 GetBoundaries()
    {
        float width = (m_ConnectorTransforms[1].position - m_ConnectorTransforms[2].position).magnitude;
        float length = (m_ConnectorTransforms[0].position - m_ConnectorTransforms[3].position).magnitude;

        return new Vector2(width, length);
    }
}
