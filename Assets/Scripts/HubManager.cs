using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HubRoomData
{
    public GameObject HallObj;
    public UserActivatedDoor RoomDoor;

    // TODO need a place to store player decorative data, like textures, colors, etc
}

public class HubManager : MonoBehaviour
{
    [SerializeField] private GameObject[] m_RoomPrefabs;
    [SerializeField] private HubRoomData[] m_HubRooms;

    private void Start()
    {
        OnLoadRequested();
    }

    private void OnLoadRequested()
    {
        GameProgress gameProg = SaveLoad.LoadGameProgress();

        // HACK HACK HACK
        // this should be loaded before here normally, but for now...
        if (gameProg == null)
        {
            gameProg = new GameProgress();
            gameProg.Init();
        }

        HubProgress hubProg = gameProg.m_HubProgress;

        int maxRoomCount = hubProg.m_HubRooms.Length;
        for (int i = 0; i < maxRoomCount; i++)
        {
            if (hubProg.m_HubRooms[i] != Enums.eHubRoom.None)
            {
                PlaceRoom(i, hubProg.m_HubRooms[i]);
            }
        }

        Vector3 startPosition = new Vector3(0f, 1.1f, -4f);
        VSEventManager.Instance.TriggerEvent(new GameEvents.DungeonBuiltEvent(startPosition, Quaternion.identity)); // should probably rename this event to something better
    }

    private void PlaceRoom(int roomIndex, Enums.eHubRoom roomType)
    {
        // find and spawn correct room
        GameObject roomPrefab = m_RoomPrefabs[(int)roomType];
        GameObject roomObj = (GameObject)Instantiate(roomPrefab, null);
        HubRoomData roomData = m_HubRooms[roomIndex];

        // position & rotate it based on hall transform
        GameObject hallObj = roomData.HallObj;
        Transform hallTransform = hallObj.transform;
        roomObj.transform.position = hallTransform.position;// - new Vector3(0f, 0f, 5f); // ? this is probably close
        roomObj.transform.rotation = hallTransform.rotation;

        // enable hall
        hallObj.SetActive(true);

        // unlock door
        roomData.RoomDoor.SetLocked(false);
    }
}
