using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [SerializeField] private string m_PlayerName;
    [SerializeField] private Enums.eGender m_Gender;
    [SerializeField] private Enums.eClassType m_Class;
    [SerializeField] private Enums.eRaceType m_Race;

    // TODO Race
    // TODO customization features? Height, weight, skin color

    [SerializeField] private int[] m_MaxStats;
    [SerializeField] private int[] m_CurrentStats;

    // Other
    [SerializeField] private int m_TotalEXP = 0;

#if UNITY_EDITOR
    // debug UI access
    public string PlayerName { get { return m_PlayerName; } }
    public string Gender { get { return m_Gender == Enums.eGender.Male ? "♂" : "♀"; } }
    public string Race { get { return m_Race.ToString(); } }
    public string Class { get { return m_Class.ToString(); } }

    public int HP { get { return m_MaxStats[(int)Enums.eStatType.HP]; } }
    public int MP { get { return m_MaxStats[(int)Enums.eStatType.MP]; } }
    public int ATT { get { return m_MaxStats[(int)Enums.eStatType.Att]; } }
    public int DEF { get { return m_MaxStats[(int)Enums.eStatType.Def]; } }
    public int ACC { get { return m_MaxStats[(int)Enums.eStatType.Acc]; } }
    public int MGC { get { return m_MaxStats[(int)Enums.eStatType.Mgc]; } }
    public int EVN { get { return m_MaxStats[(int)Enums.eStatType.Evn]; } }
    public int SPD { get { return m_MaxStats[(int)Enums.eStatType.Spd]; } }
    public int LCK { get { return m_MaxStats[(int)Enums.eStatType.Lck]; } }

#endif

    public CharacterStats()
    {
        m_MaxStats = new int[(int)Enums.eStatType.Count];
    }

    public void Init()
    {
        GenerateRandom();
        SetMaxStats();
        InitCurrentStats();
    }

    private void GenerateRandom()
    {
        float randGender = UnityEngine.Random.Range(0f, 1f);
        m_Gender = (randGender < 0.5f ? Enums.eGender.Male : Enums.eGender.Female);

        string[] names = (m_Gender == Enums.eGender.Male ? CharacterManager.Instance.m_MaleTestNames : CharacterManager.Instance.m_FemaleTestNames);
        int randName = UnityEngine.Random.Range(0, names.Length);
        m_PlayerName = names[randName];

        int randClass = UnityEngine.Random.Range(0, (int)Enums.eClassType.All);
        m_Class = (Enums.eClassType)randClass;

        m_Race = Enums.eRaceType.Human;
    }

    private void SetMaxStats()
    {
        // base
        CharacterRaceStatsPreset racePreset = CharacterManager.Instance.GetPresetFromRace(m_Race);
        m_MaxStats[(int)Enums.eStatType.HP] = racePreset.Base_HP;
        m_MaxStats[(int)Enums.eStatType.MP] = racePreset.Base_MP;
        m_MaxStats[(int)Enums.eStatType.Att] = racePreset.Base_Att;
        m_MaxStats[(int)Enums.eStatType.Def] = racePreset.Base_Def;
        m_MaxStats[(int)Enums.eStatType.Acc] = racePreset.Base_Acc;
        m_MaxStats[(int)Enums.eStatType.Mgc] = racePreset.Base_Mgc;
        m_MaxStats[(int)Enums.eStatType.Evn] = racePreset.Base_Evn;
        m_MaxStats[(int)Enums.eStatType.Spd] = racePreset.Base_Spd;
        m_MaxStats[(int)Enums.eStatType.Lck] = racePreset.Base_Lck;

        // modify
        CharacterClassStatsPreset classPreset = CharacterManager.Instance.GetPresetFromClass(m_Class);
        m_MaxStats[(int)Enums.eStatType.HP] += classPreset.Base_HP;
        m_MaxStats[(int)Enums.eStatType.MP] += classPreset.Base_MP;
        m_MaxStats[(int)Enums.eStatType.Att] += classPreset.Base_Att;
        m_MaxStats[(int)Enums.eStatType.Def] += classPreset.Base_Def;
        m_MaxStats[(int)Enums.eStatType.Acc] += classPreset.Base_Acc;
        m_MaxStats[(int)Enums.eStatType.Mgc] += classPreset.Base_Mgc;
        m_MaxStats[(int)Enums.eStatType.Evn] += classPreset.Base_Evn;
        m_MaxStats[(int)Enums.eStatType.Spd] += classPreset.Base_Spd;
        m_MaxStats[(int)Enums.eStatType.Lck] += classPreset.Base_Lck;

        // TODO
        // modify certain stats based on customization features
    }

    private void InitCurrentStats()
    {
        int total = (int)Enums.eStatType.Count;
        m_CurrentStats = new int[total];

        for (int i = 0; i < total; i++)
        {
            Restore((Enums.eStatType)i, -1);
        }
    }

    public void Restore(Enums.eStatType stat, int amount = -1)
    {
        int statIndex = (int)stat;
        bool fullRestore = amount < 0;

        if (fullRestore)
        {
            m_CurrentStats[statIndex] = m_MaxStats[statIndex];
        }
        else
        {
            m_CurrentStats[statIndex] += amount;
        }
    }

    // TODO this should be a listener for some xp added event
    private void OnXPEarned()
    {

    }
}
