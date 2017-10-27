using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] protected GameObject m_DoorObj;
    [SerializeField] private Transform m_EndLocation;

    public bool IsOnMainPath = false;

    protected int m_RoomID;
    private bool m_MoveDoor = false;

    public virtual void SetRoomID(int id)
    {
        m_RoomID = id;
    }

    protected virtual void UnlockDoor()
    {
        VSEventManager.Instance.TriggerEvent(new GameEvents.DoorOpenedEvent(this.transform));
        
        // TODO make this less lame
        m_MoveDoor = true;
    }

    protected virtual void Update()
    {
        if (m_MoveDoor)
        {
            Vector3 pos = Vector3.Lerp(m_DoorObj.transform.localPosition, m_EndLocation.localPosition, Time.deltaTime);
            m_DoorObj.transform.localPosition = pos;

            if (pos == m_EndLocation.localPosition)
            {
                m_MoveDoor = false;
            }
        }
    }
}
