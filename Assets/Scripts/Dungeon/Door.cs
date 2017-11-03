using UnityEngine;

public class Door : RoomObject
{
    [SerializeField] protected GameObject m_DoorObj;
    [SerializeField] private Transform m_EndLocation;
    [SerializeField] private GameObject m_DustParticleObj;

    public bool IsOnMainPath = false;

    private bool m_MoveDoor = false;

    protected virtual void UnlockDoor()
    {
        VSEventManager.Instance.TriggerEvent(new GameEvents.DoorOpenedEvent(this.transform));
        
        // TODO make this less lame
        m_MoveDoor = true;

        Instantiate(m_DustParticleObj, transform.position, transform.rotation);
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
