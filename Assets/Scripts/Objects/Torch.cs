using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : RoomObject
{
    [SerializeField] private GameObject m_LightSourceObj;

    private Enums.eTimeOfDay m_TimeOfDay;

    protected override void Start()
    {
        base.Start();

        VSEventManager.Instance.AddListener<GameEvents.PlayerEnteredRoomEvent>(OnPlayerEnteredRoom);
        VSEventManager.Instance.AddListener<GameEvents.TimeOfDayChangeEvent>(OnTimeOfDayChanged);

        m_LightSourceObj.SetActive(false);
    }

    protected override void OnPlayerEnteredRoom(GameEvents.PlayerEnteredRoomEvent e)
    {
        base.OnPlayerEnteredRoom(e);

        if (m_RoomID == e.RoomID && !m_LightSourceObj.activeInHierarchy)
        {
            // if you enter a room and night, active the torches
            if (m_TimeOfDay == Enums.eTimeOfDay.Sunset)
            //if (TimeKeeper.CurrentTime <= TimeKeeper.PreSunrise || TimeKeeper.CurrentTime >= TimeKeeper.Sunset)
            {
                m_LightSourceObj.SetActive(true);
            }
        }
    }

    protected void OnTimeOfDayChanged(GameEvents.TimeOfDayChangeEvent e)
    {
        // if you're playing and it changes to night, turn the torches on
        m_TimeOfDay = e.TimeOfDay;
        m_LightSourceObj.SetActive(m_TimeOfDay == Enums.eTimeOfDay.Sunset);
    }
}
