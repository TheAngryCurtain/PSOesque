﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public class RequestDungeonEvent : VSGameEvent
    {
        public int Seed;
        public eLevelTheme Theme;

        public RequestDungeonEvent(int seed, eLevelTheme theme)
        {
            Seed = seed;
            Theme = theme;
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

    public class SetupRoomDoorEvent : VSGameEvent
    {
        public int EnemyCount;
        public int RoomID;

        public SetupRoomDoorEvent(int roomId, int enemyCount)
        {
            RoomID = roomId;
            EnemyCount = enemyCount;
        }
    }

    public class EnemyDefeatedEvent : VSGameEvent
    {
        public int RoomID;

        public EnemyDefeatedEvent(int roomID)
        {
            RoomID = roomID;
        }
    }

    public class DoorSwitchPressedEvent : VSGameEvent
    {
        public int RoomID;

        public DoorSwitchPressedEvent(int roomID)
        {
            RoomID = roomID;
        }
    }

    public class RequestItemSpawnEvent : VSGameEvent
    {
        public Vector3 SpawnPosition;
        public eItemSource ItemSource;
        public eEnemyType EnemyType;
        public eCrateType CrateType;

        public RequestItemSpawnEvent(Vector3 position, eItemSource source, eEnemyType eType, eCrateType cType)
        {
            SpawnPosition = position;
            ItemSource = source;
            EnemyType = eType;
            CrateType = cType;
        }
    }
}
