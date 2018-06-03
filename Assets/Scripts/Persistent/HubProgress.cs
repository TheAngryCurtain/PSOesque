﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HubProgress
{
    public Enums.eHubRoom[] m_HubRooms;

    public HubProgress()
    {
        m_HubRooms = new Enums.eHubRoom[]
        {
            Enums.eHubRoom.Home,
            Enums.eHubRoom.None,
            Enums.eHubRoom.None,
            Enums.eHubRoom.None,
            Enums.eHubRoom.None,
            Enums.eHubRoom.None
        };
    }
}