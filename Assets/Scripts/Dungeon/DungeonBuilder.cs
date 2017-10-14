using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    public enum eHall { Sm, Md, Lg }
    public enum eRoom { Start, End, Sqr_Sm, Sqr_Md, Sqr_Lg, Rct_Sm, Rct_Md, Rct_Lg }

    [SerializeField] private GameObject[] m_HallPrefabs;
    [SerializeField] private GameObject[] m_RoomPrefabs;

    private System.Random m_Rng;
    private int m_LastConnectorSlot = -1;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.RequestDungeonEvent>(BuildDungeon);
    }

    private void BuildDungeon(GameEvents.RequestDungeonEvent e)
    {
        m_Rng = new System.Random(e.Seed);

        Queue<eRoom> roomQueue = new Queue<eRoom>();
        Queue<eHall> hallQueue = new Queue<eHall>();
        bool dungeonComplete = false;
        int totalRoomCount = 0;
        int totalHallCount = 0;
        int maxRoomCount = 10;

        Room previousRoom = null;
        Hall previousHall = null;

        bool lastWasDeadEnd = false;
        Vector3 startRoomPos = Vector3.zero;
        Quaternion startRoomRot = Quaternion.identity;

        // queue up the first room
        roomQueue.Enqueue(eRoom.Start);
        while (!dungeonComplete)
        {
            // ROOM CREATION
            Debug.LogFormat("***** Queued Rooms: {0}", roomQueue.Count);
            //while (roomQueue.Count > 0)
            if (roomQueue.Count > 0)
            {
                // get next room
                eRoom nextRoomType = roomQueue.Dequeue();
                GameObject nextRoomObj = (GameObject)Instantiate(m_RoomPrefabs[(int)nextRoomType], null);
                Room next = nextRoomObj.GetComponent<Room>();

                if (nextRoomType == eRoom.Sqr_Sm)
                {
                    float deadendRoll = (float)m_Rng.NextDouble();//UnityEngine.Random.Range(0f, 1f);
                    if (deadendRoll > 0.5f && !lastWasDeadEnd && previousRoom != null)
                    {
                        Debug.Log(" <!> Dead End <!>");
                        next.MakeDeadEnd();
                        nextRoomObj.name = string.Format("{0} [{1}]", nextRoomObj.name, "Dead End?");
                        lastWasDeadEnd = true;
                    }
                    else
                    {
                        lastWasDeadEnd = false;

                        // only set this here to that dead end rooms aren't marked as previous
                        previousRoom = next;
                    }
                }

                totalRoomCount += 1;

                Debug.LogFormat("Placing ROOM: {0}", nextRoomObj.name);

                // spawn room
                Connector NextRoomConnector = next.GetLastUsedOrFirstConnector();

                Vector3 connectorPosition = Vector3.zero;
                Connector previousHallConnector = null;
                if (previousHall != null)
                {
                    previousHallConnector = previousHall.GetLastAvailableConnector(); // last available is used because halls only have 2 connectors on opposite ends
                    connectorPosition = previousHallConnector.Object.transform.position;
                    next.OpenWall(NextRoomConnector.Slot);

                    // set used
                    previousHallConnector.SetAvailable(false);
                    NextRoomConnector.SetAvailable(false);
                }

                Vector3 position = connectorPosition + next.GetConnectorToCenter(NextRoomConnector);
                nextRoomObj.transform.position = position;

                if (nextRoomType == eRoom.Start)
                {
                    // mark start room position for player spawning
                    // this is just temporary for now
                    startRoomPos = next.FloorCenter.position;
                    startRoomRot = Quaternion.identity;
                }

                // rotate appropriately
                if (previousHallConnector != null)
                {
                    Vector3 previousConnectorForward = previousHallConnector.Object.transform.forward;
                    float angleFromForward = 180f + SignedAngle(Vector3.forward, previousConnectorForward);
                    nextRoomObj.transform.RotateAround(connectorPosition, Vector3.up, angleFromForward);
                }

                // end check
                if (nextRoomType == eRoom.End)
                {
                    dungeonComplete = true;
                    break;
                }

                // add hallways
                int availableConnectors = next.GetTotalAvailableConnectors();

                // TODO
                // if the number of available connectors is zero, go back to the previous room to look for some
                if (availableConnectors == 0)
                {
                    // dead end room has no connectors, check the room before it
                    availableConnectors = previousRoom.GetTotalAvailableConnectors();
                }

                Debug.LogFormat("== available connectors: {0}", availableConnectors);

                for (int i = 0; i < availableConnectors; i++)
                {
                    //float hallChance = UnityEngine.Random.Range(0f, 1f);
                    //if (hallChance < 0.3f)
                    //{
                        Debug.LogFormat("> Queueing Small Hall");
                        hallQueue.Enqueue(eHall.Sm);
                    //}
                    //else if (hallChance < 0.6f)
                    //{
                    //    // TODO md hall
                    //    Debug.LogFormat("> Queueing Small Hall");
                    //    hallQueue.Enqueue(eHall.Sm);
                    //}
                    //else
                    //{
                    //    // TODO lg hall
                    //    Debug.LogFormat("> Queueing Small Hall");
                    //    hallQueue.Enqueue(eHall.Sm);
                    //}
                }
            }

            if (dungeonComplete) continue;

            // HALL CREATION

            Debug.LogFormat("******* Queued Halls: {0}", hallQueue.Count);
            //while (hallQueue.Count > 0)
            if (hallQueue.Count > 0)
            {
                eHall nextHallType = hallQueue.Dequeue();
                GameObject nextHallObj = (GameObject)Instantiate(m_HallPrefabs[(int)nextHallType], null);
                Hall next = nextHallObj.GetComponent<Hall>();
                totalHallCount += 1;

                Debug.LogFormat("Placing HALL: {0}", nextHallObj.name);

                // spawn hall
                Connector nextHallConnector = next.GetLastUsedOrFirstConnector();

                Vector3 connectorPosition = Vector3.zero;
                Connector previousRoomConnector = null;
                if (previousRoom != null)
                {
                    previousRoomConnector = previousRoom.GetRandomConnector(m_LastConnectorSlot, m_Rng); //previousRoom.GetFirstAvailableConnector();
                    previousRoom.OpenWall(previousRoomConnector.Slot);
                    m_LastConnectorSlot = previousRoomConnector.Slot;

                    // TODO check for too many left/right turns
                    if (previousRoomConnector != null)
                    {
                        connectorPosition = previousRoomConnector.Object.transform.position;

                        // set used
                        previousRoomConnector.SetAvailable(false);
                        nextHallConnector.SetAvailable(false);
                    }
                    else
                    {
                        Debug.LogFormat("... {0} had no available connectors", previousRoom);
                    }
                }

                Vector3 position = connectorPosition + next.GetConnectorToCenter(nextHallConnector);
                nextHallObj.transform.position = position;
                previousHall = next;

                // rotate appropriately
                if (previousRoomConnector != null)
                {
                    // hall connectors point into their center, where room connectors point out away from their center
                    // so we need to reverse the angle for halls
                    float angleBetweenConnectors = SignedAngle(previousRoomConnector.Object.transform.forward, nextHallConnector.Object.transform.forward);
                    nextHallObj.transform.RotateAround(connectorPosition, Vector3.up, -angleBetweenConnectors);
                }

                // add rooms
                int availableConnectors = next.GetTotalAvailableConnectors();
                Debug.LogFormat("== available connectors: {0}", availableConnectors);

                for (int i = 0; i < availableConnectors; i++)
                {
                    //float endChance = UnityEngine.Random.Range(0f, 1f);
                    //if (endChance > 0f) // test, just put the end room in
                    //{
                    //    roomQueue.Enqueue(eRoom.End);
                    //}
                    if (totalRoomCount == maxRoomCount - 1)
                    {
                        Debug.LogFormat("> Queueing End Room");
                        roomQueue.Enqueue(eRoom.End);

                        // clear the hall queue so that no more halls are processed after the end room is queued
                        //hallQueue.Clear();
                    }
                    else
                    {
                        Debug.LogFormat("> Queuing Small Square room");
                        roomQueue.Enqueue(eRoom.Sqr_Sm);
                    }
                }
            }
        }

        // notify
        VSEventManager.Instance.TriggerEvent(new GameEvents.DungeonBuiltEvent(startRoomPos, startRoomRot));
    }

    public float SignedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.y < 0) angle = -angle;

        return angle;
    }
}
