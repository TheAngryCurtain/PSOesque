using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public class RequestDungeonEvent : VSGameEvent
    {
        public int Seed;

        public RequestDungeonEvent(int seed)
        {
            Seed = seed;
        }
    }

    public class DungeonBuiltEvent : VSGameEvent
    {
        public Vector3 StartPosition;
        public Quaternion StartRotation;

        public DungeonBuiltEvent(Vector3 startPos, Quaternion startRot)
        {
            StartPosition = startPos;
            StartRotation = startRot;
        }
    }

    public class PlayerSpawnedEvent : VSGameEvent
    {
        public GameObject PlayerObj;

        public PlayerSpawnedEvent(GameObject obj)
        {
            PlayerObj = obj;
        }
    }

    public class PlayerEnteredRoomEvent : VSGameEvent
    {
        public int roomID;

        public PlayerEnteredRoomEvent(int id)
        {
            roomID = id;
        }
    }

    public class PlayerExitedRoomEvent : VSGameEvent
    {
        public int roomID;

        public PlayerExitedRoomEvent(int id)
        {
            roomID = id;
        }
    }
}
