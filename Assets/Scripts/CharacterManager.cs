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
            //m_CharacterProgress.Load();
        }
    }

    private void SaveCharacterProgress()
    {
        //m_CharacterProgress.Save();
        SaveLoad.SaveCharacterProgress(m_CharacterProgress);
    }

    private void OnCharacterUpdated(GameEvents.UpdateCharacterStatEvent e)
    {
        Debug.LogFormat("Updating {0} by {1}", e.Stat, e.Amount);
    }

    private void OnInventoryUpdated(GameEvents.UpdateInventoryEvent e)
    {
        bool successful = m_CharacterProgress.m_Inventory.Add(e.Item);
        e.AddedCallback(successful);

        Debug.LogFormat("Adding {0} successful? {1}", e.Item.Name, successful);
    }

#if UNITY_EDITOR
    private string IdString = string.Empty;
    private string quantityString = string.Empty;

    private int itemID = -1;
    private int itemQuantity = 1;
    private string itemName = string.Empty;

    private void OnGUI()
    {
        if (m_CharacterProgress != null)
        {
            // Inventory --------------------------------------------------------
            int count = m_CharacterProgress.m_Inventory.Count;
            GUI.Label(new Rect(10, 40, 300, 30), string.Format("-- Inventory -- Count: {0}, Capacity: {1}", count, m_CharacterProgress.m_Inventory.Capacity));

            for (int i = 0; i < count; i++)
            {
                GUI.Label(new Rect(10, 60 + (i * 20), 300, 30), m_CharacterProgress.m_Inventory.ItemNameAt(i) + ", Quantity: " + m_CharacterProgress.m_Inventory.QuantityAt(i).ToString());
            }

            GUI.Label(new Rect(10, 300, 300, 30), string.Format("Money: {0}", m_CharacterProgress.m_Inventory.Money));


            // Save Character ---------------------------------------------------
            if (GUI.Button(new Rect(10, 330, 200, 30), "Save Character"))
            {
                SaveCharacterProgress();
            }


            // Give Item --------------------------------------------------------
            GUILayout.BeginHorizontal();

            GUI.Label(new Rect(10f, 480f, 100f, 30f), "Item ID:");
            IdString = GUI.TextField(new Rect(10f, 500f, 100f, 30f), IdString);
            if (IdString != string.Empty)
            {
                try
                {
                    itemID = System.Convert.ToInt32(IdString);
                }
                catch (System.Exception e) { }

                try
                {
                    itemName = ItemDatabase.Instance.GetItemFromID(itemID).Name;
                }
                catch (System.Exception e) { }
            }

            GUI.Label(new Rect(120f, 480f, 100f, 30f), "Quantity:");
            quantityString = GUI.TextField(new Rect(120f, 500f, 100f, 30f), quantityString);
            if (quantityString != string.Empty)
            {
                try
                {
                    itemQuantity = System.Convert.ToInt32(quantityString);
                }
                catch (System.Exception e) { }
            }

            GUI.Label(new Rect(10f, 530f, 200f, 30f), "Name: " + itemName);

            if (GUI.Button(new Rect(10f, 560f, 200f, 30f), "Get Item"))
            {
                Tuple<int, int> itemIdQuantity = new Tuple<int, int>(itemID, itemQuantity);
                InventoryItem item = ItemDatabase.Instance.GetItemFromIDWithQuantity(itemIdQuantity);
                if (item != null)
                {
                    m_CharacterProgress.m_Inventory.Add(item);
                }
            }

            GUILayout.EndHorizontal();
        }
    }
#endif
}
