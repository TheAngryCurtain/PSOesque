using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterProgress
{
    public Inventory m_Inventory;
    public CharacterStats m_Stats;
    public int m_SaveSlot = -1;

    public static int MAX_INVENTORY_SIZE = 20;

    public CharacterProgress() { }

    public void Init()
    {
        m_Inventory = new Inventory(MAX_INVENTORY_SIZE);

        m_Stats = new CharacterStats();
        m_Stats.Init();
    }
}

[System.Serializable]
public class CharacterProgressData
{
    [SerializeField] private List<CharacterProgress> m_ProgressData;

    public static int MAX_CHARACTER_SLOTS = 9;

    public CharacterProgressData() { }

    public void Init()
    {
        m_ProgressData = new List<CharacterProgress>(MAX_CHARACTER_SLOTS);
    }

    public CharacterProgress GetCharacterProgressInSlot(int slot)
    {
        if (slot >= MAX_CHARACTER_SLOTS)
        {
            Debug.LogWarningFormat("Trying to request Character Progress from invalid slot {0}", slot);
            return null;
        }
        else if (slot >= m_ProgressData.Count)
        {
            Debug.LogWarningFormat("Trying to request Character Progress from empty slot {0}", slot);
            return null;
        }

        return m_ProgressData[slot];
    }

    public void AddCharacterProgress(CharacterProgress progress)
    {
        int saveSlot = m_ProgressData.Count;
        progress.m_SaveSlot = saveSlot;

        m_ProgressData.Add(progress);
    }
}
