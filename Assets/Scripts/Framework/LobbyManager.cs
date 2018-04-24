using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLobbyData
{
    public int m_PlayerIndex = -1;
    public string m_PlayerName;
    public Transform m_PadTransform;
    public Color m_PlayerColor;
    public GameObject m_PlayerModelObj;

    public bool m_Confirmed;

    public PlayerLobbyData(int index, string name, Transform pad, Color color)
    {
        m_PlayerIndex = index;
        m_PlayerName = name;
        m_PadTransform = pad;
        m_PlayerColor = color;

        m_Confirmed = false;
    }
}

public class LobbyManager : Singleton<LobbyManager>
{
    public int MAX_PLAYERS = 4;
    public bool AllPlayersReady { get { return  m_ConnectedPlayerCount > 0 &&  m_ReadyPlayerCount == m_ConnectedPlayerCount; } }

    public System.Action<PlayerLobbyData> OnPlayerAdded;
    public System.Action<PlayerLobbyData> OnPlayerRemoved;

    [SerializeField] private Transform[] m_TeleportTransforms;
    [SerializeField] private Material[] m_PlayerRings;
    [SerializeField] private Color[] m_PlayerColors;
    [SerializeField] private Color m_DefaultColor;

    private PlayerLobbyData[] m_PlayerData;
    private int m_ConnectedPlayerCount = 0;
    public int ConnectedPlayerCount { get { return m_ConnectedPlayerCount; } }
    private int m_ReadyPlayerCount = 0;

    public override void Awake()
    {
        base.Awake();

        m_PlayerData = new PlayerLobbyData[MAX_PLAYERS];
    }

    public void Init()
    {
        // TODO if you're the server, request yourself
        RequestAddPlayer(0);
    }

    public void SetConfirmed(int playerID, bool confirmed)
    {
        m_PlayerData[playerID].m_Confirmed = confirmed;
        m_PlayerData[playerID].m_PadTransform.GetChild(0).gameObject.SetActive(confirmed); //gross. fix this properly later

        m_ReadyPlayerCount += (confirmed ? 1 : -1);
    }

    public void RequestAddPlayer(int playerId)
    {
        // TODO
        // when the networking stuff goes in...
        // when the player (client) requests to join, acknowledge from server, then client sends player display data.
        // server gets it, sends it out to all clients

        // for local multiplayer, will need to load all existing player saves and allow them to pick one. That will get populated below.

        CharacterProgress progress = CharacterManager.Instance.GetProgressForCharacterWithID(playerId);
        string playerName = progress.m_Stats.PlayerName;
        PlayerLobbyData data = new PlayerLobbyData(playerId, playerName, m_TeleportTransforms[playerId], m_PlayerColors[playerId]);
        AddPlayer(data); // send me to everyone!
    }

    public void RequestRemovePlayer(int playerId)
    {
        PlayerLobbyData data = m_PlayerData[playerId];
        RemovePlayer(data);
    }

    private void AddPlayer(PlayerLobbyData data)
    {
        SetPlayerData(data);

        // notify UI 
        if (OnPlayerAdded != null)
        {
            OnPlayerAdded(data);
        }
    }

    private void RemovePlayer(PlayerLobbyData data)
    {
        // notify UI
        if (OnPlayerRemoved != null)
        {
            OnPlayerRemoved(data);
        }

        ClearPlayerData(data);
    }

    private void SetPlayerData(PlayerLobbyData data)
    {
        int playerIndex = data.m_PlayerIndex;
        m_ConnectedPlayerCount += 1;

        // TODO
        // spawn character on pad
        // some sort of cool particle or shader effect here
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        temp.transform.position = data.m_PadTransform.position + Vector3.up;
        data.m_PlayerModelObj = temp;

        // change pad color
        m_PlayerRings[playerIndex].color = data.m_PlayerColor;
        m_PlayerData[playerIndex] = data;
    }

    private void ClearPlayerData(PlayerLobbyData data)
    {
        int playerIndex = data.m_PlayerIndex;
        m_ConnectedPlayerCount -= 1;

        // TODO
        // remove character
        // some sort of cool particle or shader effect here
        // for networking, will need to disconnect a player
        Destroy(data.m_PlayerModelObj);

        m_PlayerRings[playerIndex].color = m_DefaultColor;
        m_PlayerData[playerIndex] = null;
    }

    public PlayerLobbyData GetLobbyDataForPlayer(int playerIndex)
    {
        return m_PlayerData[playerIndex];
    }
}
