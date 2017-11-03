using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    protected int m_RoomID;

    public void SetRoomID(int id)
    {
        m_RoomID = id;
    }

    protected void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }

    protected virtual void Start()
    {
        VSEventManager.Instance.AddListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
        SetActive(false);
    }

    protected virtual void OnPlayerEnteredRoom(GameEvents.PlayerEnteredRoomEvent e)
    {
        if (m_RoomID == e.RoomID)
        {
            SetActive(true);
        }
    }
}
