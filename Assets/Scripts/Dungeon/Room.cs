using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : Space
{
    public void MakeDeadEnd()
    {
        // remove all except 0, so you have a way in
        for (int i = m_Connectors.Count - 1; i > 0; i--)
        {
            m_Connectors.RemoveAt(i);
        }
    }
}
