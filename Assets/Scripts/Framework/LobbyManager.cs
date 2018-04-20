using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLobbyData
{
    public int m_PlayerIndex = -1;
    public string m_PlayerName;
    public Transform m_PadTransform;
    public Color m_PlayerColor;

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
    public bool AllPlayersReady { get { return  m_ReadyPlayerCount == m_ConnectedPlayerCount; } }

    public System.Action<PlayerLobbyData> OnPlayerAdded;

    [SerializeField] private Transform[] m_TeleportTransforms;
    [SerializeField] private Material[] m_PlayerRings;
    [SerializeField] private Color[] m_PlayerColors;
    [SerializeField] private Color m_DefaultColor;

    private PlayerLobbyData[] m_PlayerData;
    private int m_ConnectedPlayerCount = 0;
    private int m_ReadyPlayerCount = 0;

    public override void Awake()
    {
        m_PlayerData = new PlayerLobbyData[MAX_PLAYERS];
    }

    public void Init()
    {
        // TODO
        // Get player data from save file as P1 and set player data
        // TODO when this is networked, once players connect, they will need to send an RPC with their stats to the server
        CharacterStats p1Stats = CharacterManager.Instance.PlayerCharacterProgress.m_Stats;
        PlayerLobbyData data = new PlayerLobbyData(0, p1Stats.PlayerName, m_TeleportTransforms[0], m_PlayerColors[0]);
        SetPlayerData(data);
    }

    public void SetConfirmed(int playerID, bool confirmed)
    {
        m_PlayerData[playerID].m_Confirmed = confirmed;
        m_ReadyPlayerCount += (confirmed ? 1 : -1);
    }

    public void SetPlayerData(PlayerLobbyData data)
    {
        m_PlayerData[data.m_PlayerIndex] = data;
        m_ConnectedPlayerCount += 1;

        // TODO
        // spawn character on pad
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        temp.transform.position = data.m_PadTransform.position + Vector3.up;
        // add some sort of cool teleport effect or something

        // change pad color
        m_PlayerRings[data.m_PlayerIndex].color = data.m_PlayerColor;

        // notify UI 
        if (OnPlayerAdded != null)
        {
            OnPlayerAdded(data);
        }
    }

    public PlayerLobbyData GetLobbyData(int playerIndex)
    {
        return m_PlayerData[playerIndex];
    }
}
