using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] private GameObject m_LightSourceObj;

    private int m_RoomID;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);

        m_LightSourceObj.SetActive(false);
    }

    public void SetRoomID(int id)
    {
        m_RoomID = id;
    }

    private void OnPlayerEnteredRoom(GameEvents.PlayerEnteredRoomEvent e)
    {
        if (e.RoomID == m_RoomID && !m_LightSourceObj.activeInHierarchy)
        {
            m_LightSourceObj.SetActive(true);
        }
    }
}
