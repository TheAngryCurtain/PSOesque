using UnityEngine;

public class Connector
{
    public int Slot;
    public bool Available;
    public Space NextSpace;
    public bool IsOnMainPath = false;

    public Vector3 WorldPosition { get { return m_Obj.transform.position; } }
    public Vector3 Forward { get { return m_Obj.transform.forward; } }

    private GameObject m_Obj;
    public Transform ObjTransform { get { return m_Obj.transform; } }

    public Connector(int slot, GameObject obj, bool mainPath, bool available = true)
    {
        Slot = slot;
        IsOnMainPath = mainPath;
        Available = available;
        m_Obj = obj;
    }

    public void SetNextSpace(Space s)
    {
        NextSpace = s;
        Available = false;
    }
}
