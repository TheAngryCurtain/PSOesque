﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDatum
{
    public eRoom RoomType;
    public Room Room;
    public Connector PreviousHallConnector;

    public bool IsDeadEnd { get { return Room.DeadEnd; } }

    public eHall PreviousHallType;

    public RoomDatum(eRoom type)
    {
        RoomType = type;
    }

    public void SetRoom(Room r, Connector previous)
    {
        Room = r;
        PreviousHallConnector = previous;
    }

    public void SetDeadEnd()
    {
        Room.MakeDeadEnd();
    }
}
