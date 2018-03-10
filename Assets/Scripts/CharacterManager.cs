using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterManager : Singleton<CharacterManager>
{
    private CharacterProgress m_CharacterProgress;

#if UNITY_EDITOR
    [MenuItem("Character Manager/Clear Character Save")]
    public static void ClearCharacterSave()
    {
        SaveLoad.ClearSaveData(true, false);
    }

    [MenuItem("Character Manager/Clear Game Save")]
    public static void ClearGameSave()
    {
        SaveLoad.ClearSaveData(false, true);
    }
#endif

    public override void Awake()
    {
        SaveLoad.Init();
        LoadCharacterProgress();
    }

    public void Start()
    {
        VSEventManager.Instance.AddListener<GameEvents.UpdateCharacterStatEvent>(OnCharacterUpdated);
        VSEventManager.Instance.AddListener<GameEvents.UpdateInventoryEvent>(OnInventoryUpdated);
    }

    private void LoadCharacterProgress()
    {
        m_CharacterProgress = SaveLoad.LoadCharacterProgress();
        if (m_CharacterProgress == null)
        {
            m_CharacterProgress = new CharacterProgress();
            m_CharacterProgress.Init();

            SaveCharacterProgress();
        }
        else
        {
            Debug.LogFormat("> Successfully loaded... something!");
            m_CharacterProgress.Load();
        }
    }

    private void SaveCharacterProgress()
    {
        SaveLoad.SaveCharacterProgress(m_CharacterProgress);
    }

    private void OnCharacterUpdated(GameEvents.UpdateCharacterStatEvent e)
    {
        Debug.LogFormat("Updating {0} by {1}", e.Stat, e.Amount);
    }

    private void OnInventoryUpdated(GameEvents.UpdateInventoryEvent e)
    {
        bool successful = m_CharacterProgress.m_Inventory.Add(e.Data);
        e.AddedCallback(successful);

        Debug.LogFormat("Adding {0} successful? {1}", e.Data.m_ItemName, successful);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (m_CharacterProgress != null)
        {
            int count = m_CharacterProgress.m_Inventory.Count;
            GUI.Label(new Rect(10, 40, 300, 30), string.Format("-- Inventory -- Count: {0}, Capacity: {1}", count, m_CharacterProgress.m_Inventory.Capacity));

            for (int i = 0; i < count; i++)
            {
                GUI.Label(new Rect(10, 60 + (i * 20), 300, 30), m_CharacterProgress.m_Inventory.ItemNameAt(i));
            }

            GUI.Label(new Rect(10, 300, 300, 30), string.Format("Money: {0}", m_CharacterProgress.m_Inventory.Money));

            if (GUI.Button(new Rect(10, 330, 200, 30), "Save Character"))
            {
                SaveCharacterProgress();
            }
        }
    }
#endif
}
