﻿#define DEBUG_BUILD_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    public class RoomDatum
    {
        public eRoom RoomType;
        public Room Room;

        public RoomDatum(eRoom type)
        {
            RoomType = type;
        }

        public void SetRoom(Room r)
        {
            Room = r;
        }
    }

    public enum eHall { Sm, Md, Lg }
    public enum eRoom { Start, End, Sqr_Sm, Sqr_Md, Sqr_Lg, Rct_Sm, Rct_Md, Rct_Lg }

    [SerializeField] private GameObject[] m_HallPrefabs;
    [SerializeField] private GameObject[] m_RoomPrefabs;

    private System.Random m_Rng;
    private int m_MaxRoomCount = 5;
    private int m_CurrentRoomCount = 0;
    //private int m_previousRoomDataIndex;
    //private int m_LastConnectorSlot = -1;

    //private bool TESTDOOR = false;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.RequestDungeonEvent>(BuildDungeon);
    }

    private void BuildDungeon(GameEvents.RequestDungeonEvent e)
    {
        m_Rng = new System.Random(e.Seed);

        ProcessRoom(new RoomDatum(eRoom.Start), null);
    }

	private void ProcessRoom(RoomDatum roomData, Connector prevHallConnector)
	{
		// NEW IDEA
		// the idea here is that you'd pass in a room, AND the previous connector (end of a hall basically)
		// this way, you'd have all the info you'd need (IsOnMainPath, position, rotation, etc) to position the next room
		// this could possibly eliminate a bunch of extra data currently being stored and the list of roomData may not
		// even be needed. 

		// spawn the new room
		 GameObject roomObj = (GameObject)Instantiate(m_RoomPrefabs[(int)roomData.RoomType], null);
        Room room = roomObj.GetComponent<Room>();
        roomData.SetRoom(room);

        Connector entryConnector = room.Connectors[0];
        if (roomData.RoomType == eRoom.Start)
        {
            entryConnector.IsOnMainPath = true;
        }

		if (prevHallConnector != null)
		{
			// ------- since we pass in the previous, we won't need the previousRoomDataIndex to look for a room
			// ------- we can just use the connector to handle the rotations

			Vector3 position = prevHallConnector.WorldPosition + room.GetConnectorToCenter(entryConnector);
            roomObj.transform.position = position;

			// update connections
            prevHallConnector.SetNextSpace(room);
            entryConnector.IsOnMainPath = prevHallConnector.IsOnMainPath;

			// rotate
            float angleFromForward = 180f + SignedAngle(Vector3.forward, prevHallConnector.Forward);
            roomObj.transform.RotateAround(prevHallConnector.WorldPosition, Vector3.up, angleFromForward);
		}
		else
        {
            // no previous room, likely the start room
            roomObj.transform.position = Vector3.zero + room.GetConnectorToCenter(entryConnector);
        }

        // check for dead ends
        if (roomData.RoomType == eRoom.Sqr_Sm && !entryConnector.IsOnMainPath)
        {
            //float deadEndChance = (float)m_Rng.NextDouble();
            //if (deadEndChance < 0.75f)
            //{
                LogMessage("<!> DEAD END <!>");
                
                room.MakeDeadEnd();
                roomObj.name = string.Format("{0} [{1}]", roomObj.name, "Dead End");
                return;
            //}
        }

        // don't count the dead ends
        m_CurrentRoomCount += 1;

        // check end room
        if (roomData.RoomType == eRoom.End)
        {
            LogMessage("> End Room Placed");
            return;
        }

		bool mainPathChosen = false;
        int mainPathIndex = m_Rng.Next(room.Connectors.Count);
        int hallsPlaced = 0;
        // lay down the hallways
        for (int i = 0; i < room.Connectors.Count; i++)
        {
            Connector currentRoomConnector = room.Connectors[i];

            eHall hallType = eHall.Lg;
            float chanceOfHall = (float)m_Rng.NextDouble();
            bool placeHall = (hallsPlaced == 0 && room.Connectors.Count == 1) || chanceOfHall < 0.75f;

            if (placeHall)
            {
                float hallTypeChance = (float)m_Rng.NextDouble();
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
                Vector3 position = currentRoomConnector.WorldPosition + hall.GetConnectorToCenter(hallEntryConnector);
                position.y = -0.01f; // hack to prevent clipping
                hallObj.transform.position = position;

                // open the way
                room.OpenWall(currentRoomConnector.Slot);
                
                // update connections
                currentRoomConnector.SetNextSpace(hall);
                if (entryConnector.IsOnMainPath && !mainPathChosen && i == mainPathIndex)
                {
                    // must be a better way to do this
                    currentRoomConnector.IsOnMainPath = true;
                    hallEntryConnector.IsOnMainPath = true;
                    hall.Connectors[1].IsOnMainPath = true;
                    mainPathChosen = true;
                }

                // rotate
                float angleBetweenConnectors = SignedAngle(currentRoomConnector.Forward, hallEntryConnector.Forward);
                hallObj.transform.RotateAround(currentRoomConnector.WorldPosition, Vector3.up, -angleBetweenConnectors);

                hallsPlaced += 1;

                // ------- THEN right here, you have access to the (now) previous connector, so just create the new room

                // TODO
                // determine the next room type
                // create a new RoomDatum
                // grab the END connector from the hall (hall.Connectors[1] I believe)
                // ProcessRoom(new RoomDatum( next room type, END connector)

                // --- NOTES ---
                // make sure to add a case up top where a small room is likely to be a dead end
                // if you reach a dead end, just return. That way the function can exit and resume the loop from the previous room
                // should just work?

                RoomDatum nextRoom = null;
                if (m_CurrentRoomCount == m_MaxRoomCount - 1)
                {
                    nextRoom = new RoomDatum(eRoom.End);
                }
                else
                {
                    float nextRoomChance = (float)m_Rng.NextDouble();
                    if (nextRoomChance < 0.75f)
                    {
                        nextRoom = new RoomDatum(eRoom.Sqr_Sm);
                    }
                    else if (nextRoomChance < 0.9f)
                    {
                        nextRoom = new RoomDatum(eRoom.Sqr_Md);
                    }
                    else
                    {
                        nextRoom = new RoomDatum(eRoom.Rct_Sm);
                    }

                    LogMessage(string.Format("Rolled {0}, chose {1}", nextRoomChance, nextRoom.RoomType));
                }

                ProcessRoom(nextRoom, hall.Connectors[1]);
            }
        }
	}

    private void LogMessage(string message)
    {
#if DEBUG_BUILD_LOG
        Debug.Log(message);
#endif
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

    public float SignedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.y < 0) angle = -angle;

        return angle;
    }
}
