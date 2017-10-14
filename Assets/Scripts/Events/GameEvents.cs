using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public class RequestDungeonEvent : VSGameEvent { }
    public class DungeonBuiltEvent : VSGameEvent
    {
        public Transform StartLocation;

        public DungeonBuiltEvent(Transform startLoc)
        {
            StartLocation = startLoc;
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
}
