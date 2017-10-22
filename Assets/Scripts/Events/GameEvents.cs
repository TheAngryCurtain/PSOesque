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
        public int RoomID;
        public Transform Player;

        public PlayerEnteredRoomEvent(int id, Transform p)
        {
            RoomID = id;
            Player = p;
        }
    }

    public class PlayerExitedRoomEvent : VSGameEvent
    {
        public int RoomID;

        public PlayerExitedRoomEvent(int id)
        {
            RoomID = id;
        }
    }

    public class EnemyDefeatedEvent : VSGameEvent { }
}
