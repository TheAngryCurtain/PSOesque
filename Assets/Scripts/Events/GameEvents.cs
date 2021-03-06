﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public class RequestDungeonEvent : VSGameEvent
    {
        public int Seed;
        public Enums.eLevelTheme Theme;

        public RequestDungeonEvent(int seed, Enums.eLevelTheme theme)
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
        public Enums.eItemSource ItemSource;
        public Enums.eEnemyType EnemyType;
        public Enums.eCrateType CrateType;

        public RequestItemSpawnEvent(Vector3 position, Enums.eItemSource source, Enums.eEnemyType eType, Enums.eCrateType cType)
        {
            SpawnPosition = position;
            ItemSource = source;
            EnemyType = eType;
            CrateType = cType;
        }
    }

    public class DoorOpenedEvent : VSGameEvent
    {
        public Transform DoorTransform;

        public DoorOpenedEvent(Transform door)
        {
            DoorTransform = door;
        }
    }

    public class TimeOfDayChangeEvent : VSGameEvent
    {
        public Enums.eTimeOfDay TimeOfDay;

        public TimeOfDayChangeEvent(Enums.eTimeOfDay tod)
        {
            TimeOfDay = tod;
        }
    }

    public class AttackLandedEvent : VSGameEvent
    {
        public Vector3 HitLocation;

        public AttackLandedEvent(Vector3 position)
        {
            HitLocation = position;
        }
    }

    public class UpdateCharacterStatEvent : VSGameEvent
    {
        public Enums.eStatType Stat;
        public bool UpdateMax;
        public int Amount;

        public UpdateCharacterStatEvent(Enums.eStatType stat, int amount, bool updateMax = false)
        {
            Stat = stat;
            Amount = amount;
            UpdateMax = updateMax;
        }
    }

    public class UpdateInventoryEvent : VSGameEvent
    {
        public InventoryItem Item;
        public int Quantity;
        public int SaveSlot;
        public System.Action<bool> AddedCallback;

        public UpdateInventoryEvent(InventoryItem item, int quantity, int saveSlot, System.Action<bool> callback)
        {
            Item = item;
            Quantity = quantity;
            SaveSlot = saveSlot;
            AddedCallback = callback;
        }
    }

	public class UpdatePlayerEXPEvent: VSGameEvent
	{
		public int PlayerID;
		public int Amount;

		public UpdatePlayerEXPEvent(int id, int amount)
		{
			PlayerID = id;
			Amount = amount;
		}
	}
}
