using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterManager : Singleton<CharacterManager>
{
    private CharacterProgress m_CharacterProgress;

    private Dictionary<int, CharacterClassStatsPreset> m_ClassPresets;
    private Dictionary<int, CharacterRaceStatsPreset> m_RacePresets;

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
        LoadClassPresets();
        LoadRacePresets();

        SaveLoad.Init();
        LoadCharacterProgress();
    }

    private void LoadClassPresets()
    {
        m_ClassPresets = new Dictionary<int, CharacterClassStatsPreset>();
        CharacterClassStatsPreset[] presets = Resources.LoadAll<CharacterClassStatsPreset>("Character Stats/Class");

        for (int i = 0; i < presets.Length; i++)
        {
            m_ClassPresets.Add((int)presets[i].ClassType, presets[i]);
        }
    }

    private void LoadRacePresets()
    {
        m_RacePresets = new Dictionary<int, CharacterRaceStatsPreset>();
        CharacterRaceStatsPreset[] races = Resources.LoadAll<CharacterRaceStatsPreset>("Character Stats/Race");

        for (int i = 0; i < races.Length; i++)
        {
            m_RacePresets.Add((int)races[i].RaceType, races[i]);
        }
    }

    public CharacterClassStatsPreset GetPresetFromClass(Enums.eClassType classType)
    {
        return m_ClassPresets[(int)classType];
    }

    public CharacterRaceStatsPreset GetPresetFromRace(Enums.eRaceType raceType)
    {
        return m_RacePresets[(int)raceType];
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
            Debug.LogFormat("> Successfully Loaded Character Progress");
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
        bool successful = m_CharacterProgress.m_Inventory.Add(e.Item);
        e.AddedCallback(successful);

        Debug.LogFormat("Adding {0} successful? {1}", e.Item.Name, successful);
    }

    #region Test Names
    [HideInInspector]
    public string[] m_MaleTestNames = new string[]
    {
        "Liam", "Jackson", "Logan", "Lucas", "Noah", "Ethan", "William", "Jacob", "James",
        "Santiago", "Juan", "Matias", "Benjamin", "Pedro", "Tomas",
        "Miguel", "Arthur", "Davi", "Gabriel",
        "Joshua", "Daniel", "Javier", "Antonio", "Carlos",
        "Sebastian", "Dylan", "Ian", "Diego",
        "John", "Michael", "David", "Richard",

        "Bo", "Cheng", "Dong", "Gang", "Jian", "Kang", "Liang", "Ning", "Peng", "Tao", "Wei",
        "Jie", "Hao", "Yi", "Feng",
        "Aarav", "Reyansh", "Mohammad", "Vivaan",
        "Mahdi", "Ali", "Hassan", "Reza",
        "Ori", "David", "Ariel", "Noam", "Yosef",
        "Itsuki", "Tatsuki", "Ren", "Haruta", "Asahi", "Haruki", "Tomoharu", "Yuuma", "Minato", "Ichika",
        "Min-jun", "Seo-jun", "Joo-won", "Shi-woo", "Do-yoon"
    };

    [HideInInspector]
    public string[] m_FemaleTestNames = new string[]
    {
        "Sofia", "Maria", "Lucia", "Martina", "Catalina", "Emilia",
        "Alice", "Valentina", "Helena", "Laura",
        "Emma", "Olivia", "Zoe", "Emily", "Isabella", "Charolette", "Ava",
        "Florence", "Chole", "Charlotte",
        "Amanda", "Agustina",
        "Gabrielle", "Tianna", "Brianna", "Jada",
        "Elizabeth", "Ramona", "Carolina", "Mabel",
        "Abigail", "Mary",
        "Victoria", "Nicole", "Samantha",

        "Cai", "Dan", "Fang", "Hong", "Lan", "Li", "Na", "Qian", "Shan", "Ting", "Xia", "Zhen",
        "Jing", "Ying", "Yan", "Xiaoyan", "Lili",
        "Diya", "Saanvi", "Angel", "Myra", "Riya",
        "Zahra", "Setayesh", "Hasti", "Mobina",
        "Tamar", "Avigail", "Adele", "Shira", "Talia", "Sarah",
        "Maria", "Celine", "Maya", "Noor",
        "Sakura", "Riko", "Wakana", "Azuna", "Hinata", "Yuna", "Himari", "Kaede",
        "Seo-yeon", "Seo-yun", "Ji-woo", "Min-seo", "Chae-won", "Ha-yoon"
    };
    #endregion

    #region Debug UI
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
            GUI.Label(new Rect(10f, 40f, 300f, 30f), string.Format("-- Inventory -- Count: {0}, Capacity: {1}", count, m_CharacterProgress.m_Inventory.Capacity));

            for (int i = 0; i < count; i++)
            {
                GUI.Label(new Rect(10f, 60f + (i * 20f), 300f, 30f), m_CharacterProgress.m_Inventory.ItemNameAt(i) + ", Quantity: " + m_CharacterProgress.m_Inventory.QuantityAt(i).ToString());
            }

            GUI.Label(new Rect(10f, 300f, 300f, 30f), string.Format("Money: {0}", m_CharacterProgress.m_Inventory.Money));


            // Save Character ---------------------------------------------------
            if (GUI.Button(new Rect(10f, 330f, 200f, 30f), "Save Character"))
            {
                SaveCharacterProgress();
            }


            // Give Item --------------------------------------------------------
            GUILayout.BeginHorizontal();

            GUI.Label(new Rect(10f, 380f, 100f, 30f), "Item ID:");
            IdString = GUI.TextField(new Rect(10f, 400f, 100f, 30f), IdString);
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

            GUI.Label(new Rect(120f, 380f, 100f, 30f), "Quantity:");
            quantityString = GUI.TextField(new Rect(120f, 400f, 100f, 30f), quantityString);
            if (quantityString != string.Empty)
            {
                try
                {
                    itemQuantity = System.Convert.ToInt32(quantityString);
                }
                catch (System.Exception e) { }
            }

            GUI.Label(new Rect(10f, 430f, 200f, 30f), "Name: " + itemName);

            if (GUI.Button(new Rect(10f, 460f, 200f, 30f), "Get Item"))
            {
                Tuple<int, int> itemIdQuantity = new Tuple<int, int>(itemID, itemQuantity);
                InventoryItem item = ItemDatabase.Instance.GetItemFromIDWithQuantity(itemIdQuantity);
                if (item != null)
                {
                    m_CharacterProgress.m_Inventory.Add(item);
                    IdString = string.Empty;
                    quantityString = string.Empty;
                }
            }

            GUILayout.EndHorizontal();

            // Character Stats ------------------------------------------------
            float leftStart = Screen.width - 10f - 200f;
            GUI.Label(new Rect(leftStart, 10f, 200f, 30f), "- CHARACTER -");
            GUI.Label(new Rect(leftStart, 30f, 200f, 30f), "Gender: " + m_CharacterProgress.m_Stats.Gender + "   " + "Name: " + m_CharacterProgress.m_Stats.PlayerName);
            GUI.Label(new Rect(leftStart, 50f, 200f, 30f), "Race: " + m_CharacterProgress.m_Stats.Race + "   " + "Class: " + m_CharacterProgress.m_Stats.Class);

            GUI.Label(new Rect(leftStart, 90f, 200f, 30f), "- STATS -");
            GUI.Label(new Rect(leftStart, 110f, 200f, 30f), "HP: " + m_CharacterProgress.m_Stats.HP);
            GUI.Label(new Rect(leftStart, 130f, 200f, 30f), "MP: " + m_CharacterProgress.m_Stats.MP);
            GUI.Label(new Rect(leftStart, 150f, 200f, 30f), "ATT: " + m_CharacterProgress.m_Stats.ATT);
            GUI.Label(new Rect(leftStart, 170f, 200f, 30f), "DEF: " + m_CharacterProgress.m_Stats.DEF);
            GUI.Label(new Rect(leftStart, 190f, 200f, 30f), "ACC: " + m_CharacterProgress.m_Stats.ACC);
            GUI.Label(new Rect(leftStart, 210f, 200f, 30f), "MGC: " + m_CharacterProgress.m_Stats.MGC);
            GUI.Label(new Rect(leftStart, 230f, 200f, 30f), "EVN: " + m_CharacterProgress.m_Stats.EVN);
            GUI.Label(new Rect(leftStart, 250f, 200f, 30f), "SPD: " + m_CharacterProgress.m_Stats.SPD);
            GUI.Label(new Rect(leftStart, 270f, 200f, 30f), "LCK: " + m_CharacterProgress.m_Stats.LCK);

            if (GUI.Button(new Rect(leftStart, 300f, 200f, 30f), "Randomize"))
            {
                m_CharacterProgress.m_Stats.Init();
                SaveCharacterProgress();
            }
        }
    }
#endif
    #endregion
}
