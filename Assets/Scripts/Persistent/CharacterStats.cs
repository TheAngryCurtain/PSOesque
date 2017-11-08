using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public string m_PlayerName;
    public Enums.eClassType m_Class;
    // TODO Race

    public Enums.eStatType[] m_Stats;

    public CharacterStats()
    {
        m_PlayerName = string.Empty;
        m_Stats = new Enums.eStatType[(int)Enums.eStatType.Count];
    }
}
