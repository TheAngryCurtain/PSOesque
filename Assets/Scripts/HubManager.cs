using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HubRoomData
{
    public GameObject HallObj;
    public UserActivatedDoor RoomDoor;
}

public class HubManager : MonoBehaviour
{
    // ensure the prefabs are placed in the hubRooms array in this order
    private enum eHubRoom { Shop, Teleporter, QuestKiosk, Home, Bank, Hospital, Trophy };

    [SerializeField] private GameObject[] m_RoomPrefabs;
    [SerializeField] private HubRoomData[] m_HubRooms;

    // TODO
    // Once the scene is loaded, fire an event
    // if you're the host/player 1, get that player's save data (TODO game progress not done yet)
    // Load the save data progress, place the rooms based on where the player placed them
    // spawn the player(s)
}
