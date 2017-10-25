using System.Collections.Generic;
using UnityEngine;

public enum eHall { Sm, Md, Lg }
public enum eRoom { Start, End, Sqr_Sm, Sqr_Md, Sqr_Lg, Rct_Sm, Rct_Md, Rct_Lg }

public class DungeonBuilder : MonoBehaviour
{
    [SerializeField] private GameObject[] m_HallPrefabs;
    [SerializeField] private GameObject[] m_RoomPrefabs;

    private System.Random m_Rng;
    private Queue<RoomDatum> m_PotentialPathRooms;
    private int m_CurrentRoomCount = 0;
    private bool m_MainPath = true;

    // used to place objects
    private List<RoomDatum> m_MainPathRooms;
    private List<RoomDatum> m_DeadEndRooms;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.RequestDungeonEvent>(BuildDungeon);
    }

    private void BuildDungeon(GameEvents.RequestDungeonEvent e)
    {
        Debug.LogFormat("Seed: {0}, Theme: {1}", e.Seed, e.Theme);

        // TODO
        // will need to load prefabs based on level theme!

        m_Rng = new System.Random(e.Seed);

        m_PotentialPathRooms = new Queue<RoomDatum>();
        m_MainPathRooms = new List<RoomDatum>();
        m_DeadEndRooms = new List<RoomDatum>();

        // construct the main path
        int maxPathLength = 5;
        m_CurrentRoomCount = 0;
        m_MainPath = true;
        RoomDatum startRoom = new RoomDatum(eRoom.Start);
        ProcessNextRoom(startRoom, null, maxPathLength);

        // fill out side paths
        BuildSidePaths();

        // place interesting things
        PlaceObjects();

        // notify
        Vector3 startPosition = startRoom.Room.FloorCenter.position;
        Quaternion startRotation = Quaternion.identity;
        VSEventManager.Instance.TriggerEvent(new GameEvents.DungeonBuiltEvent(startPosition, startRotation));
    }

    private void BuildSidePaths()
    {
        int maxLength = 4;
        int minLength = 2;

        while (m_PotentialPathRooms.Count > 0)
        {
            RoomDatum data = m_PotentialPathRooms.Dequeue();
            int pathLength = m_Rng.Next(maxLength) + minLength;

            m_CurrentRoomCount = 0;
            m_MainPath = false;
            ProcessNextRoom(data, null, pathLength);
        }
    }

	private void ProcessNextRoom(RoomDatum roomData, Connector prevHallConnector, int maxRoomCount)
	{
        Room room = BuildRoom(roomData, prevHallConnector);
        Connector entryConnector = room.Connectors[0];
        m_CurrentRoomCount += 1;

        // check for dead ends
        bool end = DetermineEndOfRoute(roomData, entryConnector);
        if (end)
        {
            return;
        }

        int previousSlot = 0; // should probably track the actual last slot to help avoid overlapping
        Connector randomConnector = room.GetRandomConnector(previousSlot, m_Rng);
        
        Hall hall = BuildHall(randomConnector, entryConnector, room, maxRoomCount);

        RoomDatum nextRoom = DetermineNextRoom(maxRoomCount);
        ProcessNextRoom(nextRoom, hall.Connectors[1], maxRoomCount);
    }

    private bool DetermineEndOfRoute(RoomDatum roomData, Connector roomEntry)
    {
        if (roomData.RoomType == eRoom.Sqr_Sm && !roomEntry.IsOnMainPath)
        {
            roomData.SetDeadEnd();
            m_DeadEndRooms.Add(roomData);

            GameObject roomObj = roomData.Room.gameObject;
            roomObj.name = string.Format("{0} [{1}]", roomObj.name, "Dead End");
            return true;
        }

        // check end room
        if (roomData.RoomType == eRoom.End)
        {
            return true;
        }

        return false;
    }

    private RoomDatum DetermineNextRoom(int maxRoomCount)
    {
        RoomDatum nextRoom = null;
        if (m_CurrentRoomCount == maxRoomCount - 1)
        {
            if (m_MainPath)
            {
                nextRoom = new RoomDatum(eRoom.End);
            }
            else
            {
                // TODO better way of marking dead end? like an eRoom type?
                nextRoom = new RoomDatum(eRoom.Sqr_Sm);
            }
        }
        else
        {
            float nextRoomChance = (float)m_Rng.NextDouble();
            if (nextRoomChance < 0.45f)
            {
                nextRoom = new RoomDatum(eRoom.Sqr_Md);
            }
            else if (nextRoomChance < 0.75f)
            {
                nextRoom = new RoomDatum(eRoom.Rct_Sm);
            }
            else
            {
                nextRoom = new RoomDatum(eRoom.Sqr_Lg);
            }
        }

        return nextRoom;
    }

    private Room BuildRoom(RoomDatum roomData, Connector prevHallConnector)
    {
        // spawn the new room
        bool existingRoom = (roomData.Room != null);

        Room room = roomData.Room;
        GameObject roomObj = null;
        Connector previousConnector = prevHallConnector;

        if (existingRoom)
        {
            room = roomData.Room;
            roomObj = room.gameObject;
            previousConnector = roomData.PreviousHallConnector;
        }
        else
        {
            roomObj = (GameObject)Instantiate(m_RoomPrefabs[(int)roomData.RoomType], null);
            room = roomObj.GetComponent<Room>();
            roomData.SetRoom(room, prevHallConnector);
        }

        Connector entryConnector = room.Connectors[0];
        if (roomData.RoomType == eRoom.Start)
        {
            entryConnector.IsOnMainPath = true;
        }

        if (previousConnector != null)
        {
            Vector3 position = previousConnector.WorldPosition + room.GetConnectorToCenter(entryConnector);
            position.y = 0f; // all rooms at 0 height
            roomObj.transform.position = position;

            // update connections
            previousConnector.SetNextSpace(room);
            entryConnector.IsOnMainPath = previousConnector.IsOnMainPath;

            // rotate
            if (!existingRoom)
            {
                float angleFromForward = 180f + Utils.SignedAngle(Vector3.forward, previousConnector.Forward);
                roomObj.transform.RotateAround(previousConnector.WorldPosition, Vector3.up, angleFromForward);
            }

            // open the door
            room.OpenWall(0);
        }
        else
        {
            // no previous room, likely the start room
            roomObj.transform.position = Vector3.zero + room.GetConnectorToCenter(entryConnector);
        }

        // queue up potential side path rooms
        if (m_MainPath && roomData.RoomType != eRoom.Start && roomData.RoomType != eRoom.End && !roomData.IsDeadEnd)
        {
            m_PotentialPathRooms.Enqueue(roomData);
            m_MainPathRooms.Add(roomData);
        }

        return room;
    }

    private void PlaceObjects()
    {
        // TODO
        // pick whether a chest, switch & door pair, etc
        // this should probably be run at the end? once we know whether rooms are dead ends or not? we could store dead ends in a list and pass over them?

        // place any locked doors and/or switches, enemy spawners
        for (int i = 0; i < m_MainPathRooms.Count; i++)
        {
            Room r = m_MainPathRooms[i].Room;
            if (m_MainPathRooms[i].RoomType == eRoom.Sqr_Lg ||
                m_MainPathRooms[i].RoomType == eRoom.Sqr_Md)
            {
                float chance = (float)m_Rng.NextDouble();
                if (chance < 0.75f)
                {
                    Room switchRoom = CheckForSwitchConfiguration(r);
                    SetupSwitchRoom(r, switchRoom);

                    // put enemies in, but with no doors
                    SetupEnemyRoom(r, false);
                }
                else
                {
                    SetupEnemyRoom(r);
                }
            }
        }

        // place any item boxes
        for (int i = 0; i < m_DeadEndRooms.Count; i++)
        {
            if (!m_DeadEndRooms[i].Room.ContainsImportantObject)
            {
                SetupItemRoom(m_DeadEndRooms[i].Room);
            }
        }
    }

    private Room CheckForSwitchConfiguration(Room r)
    {
        List<Room> closestRooms = new List<Room>();
        List<float> minSqrDists = new List<float>();

        // check non-main path rooms for nearby deadends
        for (int i = 0; i < r.Connectors.Count; i++)
        {
            if (!r.Connectors[i].IsOnMainPath)
            {
                float minSqrDist = float.MaxValue;
                Room closest = null;

                // for now, just do a distance check... not robust, but a better method is needed eventually anyway
                for (int j = 0; j < m_DeadEndRooms.Count; j++)
                {
                    float sqrDist = (r.Connectors[i].ObjTransform.position - m_DeadEndRooms[j].Room.Connectors[0].ObjTransform.position).sqrMagnitude;
                    if (sqrDist < minSqrDist)
                    {
                        minSqrDist = sqrDist;
                        closest = m_DeadEndRooms[j].Room;
                    }
                }

                closestRooms.Add(closest);
                minSqrDists.Add(minSqrDist);
            }
        }

        // get the closest possible room from each connector above
        Room absClosest = null;
        if (closestRooms.Count > 1)
        {
            float absMinSqrDist = float.MaxValue;
            for (int i = 0; i < closestRooms.Count; i++)
            {
                if (minSqrDists[i] < absMinSqrDist)
                {
                    absMinSqrDist = minSqrDists[i];
                    absClosest = closestRooms[i];
                }
            }
        }
        else
        {
            absClosest = closestRooms[0];
        }

        return absClosest;
    }

    private void SetupSwitchRoom(Room mainRoom, Room switchRoom)
    {
        // door
        for (int i = 1; i < mainRoom.Connectors.Count; i++)
        {
            // need to block main path door
            Connector c = mainRoom.Connectors[i];
            if (c.IsOnMainPath)
            {
                GameObject doorObj = (GameObject)Instantiate(ObjectFactory.Instance.GetObjectPrefab(ObjectFactory.eObject.SwitchDoor), c.ObjTransform.position, c.ObjTransform.rotation);
                SwitchDoor door = doorObj.GetComponent<SwitchDoor>();
                door.SetRoomID(mainRoom.RoomID);
            }
        }

        // switch
        GameObject switchObj = (GameObject)Instantiate(ObjectFactory.Instance.GetObjectPrefab(ObjectFactory.eObject.Switch), switchRoom.FloorCenter.position, Quaternion.identity);
        Switch s = switchObj.GetComponent<Switch>();
        s.SetRoomID(mainRoom.RoomID);

        // mark that there is something important here
        switchRoom.ContainsImportantObject = true;
    }

    private void SetupEnemyRoom(Room r, bool withDoors = true)
    {
        // door(s)
        if (withDoors)
        {
            for (int i = 1; i < r.Connectors.Count; i++)
            {
                // need to block off each exit with a hallway (except the entry door)
                Connector c = r.Connectors[i];
                if (!c.Available)
                {
                    GameObject doorObj = (GameObject)Instantiate(ObjectFactory.Instance.GetObjectPrefab(ObjectFactory.eObject.EnemyDoor), c.ObjTransform.position, c.ObjTransform.rotation);
                    EnemyDoor door = doorObj.GetComponent<EnemyDoor>();
                    door.SetRoomID(r.RoomID);
                }
            }
        }

        // enemy spawner
        GameObject spawnObj = (GameObject)Instantiate(ObjectFactory.Instance.GetObjectPrefab(ObjectFactory.eObject.EnemySpawner), null);
        spawnObj.transform.position = r.FloorCenter.position;

        EnemySpawner spawner = spawnObj.GetComponent<EnemySpawner>();
        if (spawner != null)
        {
            spawner.SetRoomID(r.RoomID);

            Vector2 boundaries = r.GetBoundaries();
            spawner.SetRoomBoundaries(boundaries.x, boundaries.y);
        }
    }

    private void SetupItemRoom(Room r)
    {
        GameObject spawnerObj = (GameObject)Instantiate(ObjectFactory.Instance.GetObjectPrefab(ObjectFactory.eObject.CrateSpawner), null);
        spawnerObj.transform.position = r.FloorCenter.position;

        CrateSpawner spawner = spawnerObj.GetComponent<CrateSpawner>();
        if (spawner != null)
        {
            spawner.SetRoomID(r.RoomID);

            Vector2 boundaries = r.GetBoundaries();
            spawner.SetRoomBoundaries(boundaries.x, boundaries.y);
        }
    }

    private Hall BuildHall(Connector roomConnector, Connector roomEntry, Room room, int maxRoomCount)
    {
        float hallTypeChance = (float)m_Rng.NextDouble();
        eHall hallType = eHall.Lg;
        if (hallTypeChance < 0.65f)
        {
            hallType = eHall.Sm;
        }
        else if (hallTypeChance < 0.85f)
        {
            hallType = eHall.Md;
        }

        GameObject hallObj = (GameObject)Instantiate(m_HallPrefabs[(int)hallType], null);
        Hall hall = hallObj.GetComponent<Hall>();

        Connector hallEntryConnector = hall.Connectors[0];

        // position hall
        Vector3 position = roomConnector.WorldPosition + hall.GetConnectorToCenter(hallEntryConnector);
        position.y = -0.01f; // hack to prevent clipping
        hallObj.transform.position = position;

        // open the way
        room.OpenWall(roomConnector.Slot);

        // update connections
        roomConnector.SetNextSpace(hall);
        if (m_MainPath)
        {
            // must be a better way to do this
            roomConnector.IsOnMainPath = true;
            hallEntryConnector.IsOnMainPath = true;
            hall.Connectors[1].IsOnMainPath = true;
        }

        // rotate
        float angleBetweenConnectors = Utils.SignedAngle(roomConnector.Forward, hallEntryConnector.Forward);
        hallObj.transform.RotateAround(roomConnector.WorldPosition, Vector3.up, -angleBetweenConnectors);

        return hall;
    }

    // >>>>>>>>>>>>> previous implementation

    //private void BuildDungeon(GameEvents.RequestDungeonEvent e)
    //{
    //    m_Rng = new System.Random(e.Seed);

    //    Queue<eRoom> roomQueue = new Queue<eRoom>();
    //    Queue<eHall> hallQueue = new Queue<eHall>();
    //    bool dungeonComplete = false;
    //    int totalRoomCount = 0;
    //    int totalHallCount = 0;
    //    int maxRoomCount = 10;

    //    Room previousRoom = null;
    //    Hall previousHall = null;

    //    bool lastWasDeadEnd = false;
    //    Vector3 startRoomPos = Vector3.zero;
    //    Quaternion startRoomRot = Quaternion.identity;

    //    // queue up the first room
    //    roomQueue.Enqueue(eRoom.Start);
    //    while (!dungeonComplete)
    //    {
    //        // ROOM CREATION
    //        Debug.LogFormat("***** Queued Rooms: {0}", roomQueue.Count);
    //        //while (roomQueue.Count > 0)
    //        if (roomQueue.Count > 0)
    //        {
    //            // get next room
    //            eRoom nextRoomType = roomQueue.Dequeue();
    //            GameObject nextRoomObj = (GameObject)Instantiate(m_RoomPrefabs[(int)nextRoomType], null);
    //            Room next = nextRoomObj.GetComponent<Room>();

    //            bool deadEnd = false;
    //            if (nextRoomType == eRoom.Sqr_Sm)
    //            {
    //                float deadendRoll = (float)m_Rng.NextDouble();
    //                if (deadendRoll > 0.25f && !lastWasDeadEnd && previousRoom != null)
    //                {
    //                    deadEnd = true;
    //                    Debug.Log(" <!> Dead End <!>");
    //                    next.MakeDeadEnd();
    //                    nextRoomObj.name = string.Format("{0} [{1}]", nextRoomObj.name, "Dead End?");
    //                    lastWasDeadEnd = true;
    //                }
    //                else
    //                {
    //                    lastWasDeadEnd = false;

    //                    // only set this here to that dead end rooms aren't marked as previous
    //                    previousRoom = next;
    //                }
    //            }
    //            else if (nextRoomType != eRoom.End)
    //            {
    //                lastWasDeadEnd = false;
    //                previousRoom = next;
    //            }

    //            totalRoomCount += 1;

    //            Debug.LogFormat("Placing ROOM: {0}", nextRoomObj.name);

    //            // spawn room
    //            Connector NextRoomConnector = next.GetLastUsedOrFirstConnector();

    //            Vector3 connectorPosition = Vector3.zero;
    //            Connector previousHallConnector = null;
    //            if (previousHall != null)
    //            {
    //                previousHallConnector = previousHall.GetLastAvailableConnector(); // last available is used because halls only have 2 connectors on opposite ends
    //                connectorPosition = previousHallConnector.Object.transform.position;
    //                next.OpenWall(NextRoomConnector.Slot);

    //                // set used
    //                previousHallConnector.SetAvailable(false);
    //                NextRoomConnector.SetAvailable(false);
    //            }

    //            Vector3 position = connectorPosition + next.GetConnectorToCenter(NextRoomConnector);
    //            nextRoomObj.transform.position = position;

    //            if (nextRoomType == eRoom.Start)
    //            {
    //                // mark start room position for player spawning
    //                // this is just temporary for now
    //                startRoomPos = next.FloorCenter.position;
    //                startRoomRot = Quaternion.identity;
    //            }

    //            // rotate appropriately
    //            if (previousHallConnector != null)
    //            {
    //                Vector3 previousConnectorForward = previousHallConnector.Object.transform.forward;
    //                float angleFromForward = 180f + SignedAngle(Vector3.forward, previousConnectorForward);
    //                nextRoomObj.transform.RotateAround(connectorPosition, Vector3.up, angleFromForward);
    //            }

    //            // room was dead end, is now in proper position/rotation. Spawn interesting thing.
    //            if (deadEnd)
    //            {
    //                // spawn an object
    //                Instantiate(ObjectFactory.Instance.GetObjectPrefab(ObjectFactory.eObject.Chest), next.FloorCenter.position, Quaternion.identity);
    //            }

    //            // end check
    //            if (nextRoomType == eRoom.End)
    //            {
    //                dungeonComplete = true;
    //                break;
    //            }

    //            if (nextRoomType == eRoom.Sqr_Md && !TESTDOOR)
    //            {
    //                TESTDOOR = true;

    //                // set up the door/switch system... this is all going to need to change
    //                GameObject switchObj = (GameObject)Instantiate(ObjectFactory.Instance.GetObjectPrefab(ObjectFactory.eObject.Switch), next.FloorCenter.position, Quaternion.identity);
    //                Switch s = switchObj.GetComponent<Switch>();

    //                // this is the previous door, not the next. we don't know about the next until the next hall is placed... so this wont' work in the current state of the dungeon builder :(
    //                // idea: when a room is place, pick the next connector at that point and store it. retrieve it in the later step when the hall needs it
    //                s.Setup(NextRoomConnector.Object.transform);
    //            }

    //            // add hallways
    //            int availableConnectors = next.GetTotalAvailableConnectors();
    //            if (availableConnectors == 0)
    //            {
    //                // dead end room has no connectors, check the room before it
    //                availableConnectors = previousRoom.GetTotalAvailableConnectors();
    //            }

    //            Debug.LogFormat("== available connectors: {0}", availableConnectors);

    //            for (int i = 0; i < availableConnectors; i++)
    //            {
    //                float hallChance = (float)m_Rng.NextDouble();
    //                if (hallChance < 0.15f)
    //                {
    //                    Debug.LogFormat("> Queueing Large Hall");
    //                    hallQueue.Enqueue(eHall.Lg);
    //                }
    //                else if (hallChance < 0.4f)
    //                {
    //                    Debug.LogFormat("> Queueing Medium Hall");
    //                    hallQueue.Enqueue(eHall.Md);
    //                }
    //                else
    //                {
    //                    Debug.LogFormat("> Queueing Small Hall");
    //                    hallQueue.Enqueue(eHall.Sm);
    //                }
    //            }
    //        }

    //        if (dungeonComplete) continue;

    //        // HALL CREATION

    //        Debug.LogFormat("******* Queued Halls: {0}", hallQueue.Count);
    //        //while (hallQueue.Count > 0)
    //        if (hallQueue.Count > 0)
    //        {
    //            eHall nextHallType = hallQueue.Dequeue();
    //            GameObject nextHallObj = (GameObject)Instantiate(m_HallPrefabs[(int)nextHallType], null);
    //            Hall next = nextHallObj.GetComponent<Hall>();
    //            totalHallCount += 1;

    //            Debug.LogFormat("Placing HALL: {0}", nextHallObj.name);

    //            // spawn hall
    //            Connector nextHallConnector = next.GetLastUsedOrFirstConnector();

    //            Vector3 connectorPosition = Vector3.zero;
    //            Connector previousRoomConnector = null;
    //            if (previousRoom != null)
    //            {
    //                previousRoomConnector = previousRoom.GetRandomConnector(m_LastConnectorSlot, m_Rng);
    //                //previousRoomConnector = previousRoom.GetFirstAvailableConnector();
    //                previousRoom.OpenWall(previousRoomConnector.Slot);
    //                m_LastConnectorSlot = previousRoomConnector.Slot;

    //                // TODO check for too many left/right turns
    //                if (previousRoomConnector != null)
    //                {
    //                    connectorPosition = previousRoomConnector.Object.transform.position;

    //                    // set used
    //                    previousRoomConnector.SetAvailable(false);
    //                    nextHallConnector.SetAvailable(false);
    //                }
    //                else
    //                {
    //                    Debug.LogFormat("... {0} had no available connectors", previousRoom);
    //                }
    //            }

    //            Vector3 position = connectorPosition + next.GetConnectorToCenter(nextHallConnector);
    //            nextHallObj.transform.position = position;
    //            previousHall = next;

    //            // rotate appropriately
    //            if (previousRoomConnector != null)
    //            {
    //                // hall connectors point into their center, where room connectors point out away from their center
    //                // so we need to reverse the angle for halls
    //                float angleBetweenConnectors = SignedAngle(previousRoomConnector.Object.transform.forward, nextHallConnector.Object.transform.forward);
    //                nextHallObj.transform.RotateAround(connectorPosition, Vector3.up, -angleBetweenConnectors);
    //            }

    //            // add rooms
    //            int availableConnectors = next.GetTotalAvailableConnectors();
    //            Debug.LogFormat("== available connectors: {0}", availableConnectors);

    //            for (int i = 0; i < availableConnectors; i++)
    //            {
    //                if (totalRoomCount == maxRoomCount - 1)
    //                {
    //                    Debug.LogFormat("> Queueing End Room");
    //                    roomQueue.Enqueue(eRoom.End);
    //                }
    //                else
    //                {
    //                    float roomChance = (float)m_Rng.NextDouble();
    //                    if (roomChance < 0.25f)
    //                    {
    //                        Debug.LogFormat("> Queuing Medium Square room");
    //                        roomQueue.Enqueue(eRoom.Sqr_Md);
    //                    }
    //                    else if (roomChance < 0.5f)
    //                    {
    //                        Debug.LogFormat("> Queuing Small Rect room");
    //                        roomQueue.Enqueue(eRoom.Rct_Sm);
    //                    }
    //                    else
    //                    {
    //                        Debug.LogFormat("> Queuing Small Square room");
    //                        roomQueue.Enqueue(eRoom.Sqr_Sm);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    // notify
    //    VSEventManager.Instance.TriggerEvent(new GameEvents.DungeonBuiltEvent(startRoomPos, startRoomRot));
    //}
}
