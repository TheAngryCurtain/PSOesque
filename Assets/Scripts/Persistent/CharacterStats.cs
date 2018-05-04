using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public System.Action<int> OnLevelChanged;
    public System.Action<int, int> OnEXPAdded;
    public System.Action<int, int> OnHPModified;
    public System.Action<int, int> OnMPModified;

    [SerializeField] private string m_PlayerName;
    [SerializeField] private Enums.eGender m_Gender;
    [SerializeField] private Enums.eClassType m_Class;
    [SerializeField] private Enums.eRaceType m_Race;

    // TODO Race
    // TODO customization features? Height, weight, skin color

    [SerializeField] private int[] m_MaxStats;
    [SerializeField] private int[] m_CurrentStats;

    private int m_TotalEXP = 0;
    private int m_CurrentEXP = 0;
    private int m_Level = 1;

#if UNITY_EDITOR
    // debug UI access
    public string PlayerName { get { return m_PlayerName; } }
    public int Level { get { return m_Level; } }
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
        // TODO EXP gameplay event. fire when killing things to test.
        //VSEventManager.Instance.AddListener<>();

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

        // ew
        if (stat == Enums.eStatType.HP && OnHPModified != null)
        {
            OnHPModified(m_CurrentStats[statIndex], m_MaxStats[statIndex]);
        }
        else if (stat == Enums.eStatType.MP && OnMPModified != null)
        {
            OnMPModified(m_CurrentStats[statIndex], m_MaxStats[statIndex]);
        }
    }

    public int GetEXPToNextLevel()
    {
        int nextLevel = m_Level + 1;
        return ((m_Level + nextLevel) * 50) - ((int)(nextLevel / 5) * 50) + ((int)(nextLevel / 15) * 98);
    }

    // just for testing!
    public void AddEXP(int amount)
    {
        OnEXPEarned(amount);
    }

    // TODO this should be a listener for some xp added event
    private void OnEXPEarned(int amount)
    {
        int EXPToNextLevel = GetEXPToNextLevel();

        m_TotalEXP += amount;
        m_CurrentEXP += amount;
        if (m_CurrentEXP > EXPToNextLevel)
        {
            int delta = m_CurrentEXP - EXPToNextLevel;
            m_Level += 1;

            if (OnLevelChanged != null)
            {
                OnLevelChanged(m_Level);
            }

            m_CurrentEXP = delta;
            EXPToNextLevel = GetEXPToNextLevel();
        }

        if (OnEXPAdded != null)
        {
            OnEXPAdded(m_CurrentEXP, EXPToNextLevel);
        }
    }
}
