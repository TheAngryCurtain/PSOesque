using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterManager : Singleton<CharacterManager>
{
    public bool DebugGUI = false;

    private List<Character> m_RegisteredCharacters;

    private CharacterProgressData m_CharacterData;
    public int SavedCharacterCount { get { return m_CharacterData.SavedCharacterCount; } }

    //private List<CharacterProgress> m_ActivePlayerProgress;

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
        base.Awake();

        //m_ActivePlayerProgress = new List<CharacterProgress>(LobbyManager.MAX_PLAYERS);
        m_RegisteredCharacters = new List<Character>();

        LoadClassPresets();
        LoadRacePresets();

        SaveLoad.Init();
        LoadCharacterProgress();

		VSEventManager.Instance.AddListener<GameEvents.UpdatePlayerEXPEvent>(OnPlayerEXPUpdated);
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
        VSEventManager.Instance.AddListener<GameEvents.UpdateCharacterStatEvent>(OnCharacterStatUpdated);
        VSEventManager.Instance.AddListener<GameEvents.UpdateInventoryEvent>(OnInventoryUpdated);
    }

    private void LoadCharacterProgress()
    {
        m_CharacterData = SaveLoad.LoadCharacterProgress();
        if (m_CharacterData == null)
        {
            m_CharacterData = new CharacterProgressData();
            m_CharacterData.Init();

            SaveCharacterProgress();
        }
        else
        {
            Debug.LogFormat("> Successfully Loaded Character Progress");
        }
    }

    public void RegisterCharacter(Character c)
    {
        m_RegisteredCharacters.Add(c);
    }

    private void SaveCharacterProgress()
    {
        SaveLoad.SaveCharacterProgress(m_CharacterData);
    }

    private void OnCharacterStatUpdated(GameEvents.UpdateCharacterStatEvent e)
    {
        Debug.LogFormat("Updating {0} by {1} (update max? {2})", e.Stat, e.Amount, e.UpdateMax);
    }

    private void OnInventoryUpdated(GameEvents.UpdateInventoryEvent e)
    {
        Inventory inventory = m_CharacterData.GetCharacterProgressInSlot(e.SaveSlot).m_Inventory;
        bool successful = inventory.Add(e.Item);
        e.AddedCallback(successful);

        Debug.LogFormat("Adding {0} successful? {1}", e.Item.Name, successful);
    }

    // also for testing local multiplayer
    public CharacterProgress GetProgressForCharacterInSlot(int slot)
    {
        return m_CharacterData.GetCharacterProgressInSlot(slot);
    }

    // also for testing local multiplayer
    public void AddCharacterProgress(CharacterProgress progress)
    {
        m_CharacterData.AddCharacterProgress(progress);
        SaveCharacterProgress();
    }

    // JUST FOR TESTING LOBBY STUFF
    public void ClearProgress()
    {
        m_CharacterData.Clear();
    }

	private void OnPlayerEXPUpdated(GameEvents.UpdatePlayerEXPEvent e)
	{
		// UGH. shouldn't be using lobby manager in here...
		int playerSaveSlot = LobbyManager.Instance.GetLobbyDataForPlayer(e.PlayerID).m_SaveSlot;
		CharacterStats stats = m_CharacterData.GetCharacterProgressInSlot(playerSaveSlot).m_Stats;
		stats.AddEXP(e.Amount);
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
    private int currentPlayerID = 0;

    private void OnGUI()
    {
        if (DebugGUI && m_CharacterData != null)
        {
            GUI.Label(new Rect(10f, 10f, 60f, 30f), "Player ID: ");
            if (int.TryParse(GUI.TextField(new Rect(40f, 10f, 30f, 30f), currentPlayerID.ToString(), 1), out currentPlayerID))
            {
                currentPlayerID = Mathf.Clamp(currentPlayerID, 0, m_RegisteredCharacters.Count - 1);
                Player currentPlayer = (Player)m_RegisteredCharacters[currentPlayerID];
                Inventory currentInv = m_CharacterData.GetCharacterProgressInSlot(currentPlayer.SaveSlot).m_Inventory;

                // Inventory --------------------------------------------------------
                int count = currentInv.Count;
                GUI.Label(new Rect(10f, 40f, 300f, 30f), string.Format("-- Inventory -- Count: {0}, Capacity: {1}", count, currentInv.Capacity));

                for (int i = 0; i < count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUI.Label(new Rect(10f, 60f + (i * 20f), 300f, 30f), currentInv.ItemNameAt(i) + ", Quantity: " + currentInv.QuantityAt(i).ToString());
                    if (currentInv.IsItemUsable(i) && GUI.Button(new Rect(10f, 60f + (i * 20f), 300f, 30f), "USE"))
                    {
                        InventoryItem item = currentInv.GetItemAt(i);
                        currentInv.UseItem(item, currentPlayer);
                    }
                    GUILayout.EndHorizontal();
                }

                GUI.Label(new Rect(10f, 300f, 300f, 30f), string.Format("Money: {0}", currentInv.Money));


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
                        currentInv.Add(item);
                        IdString = string.Empty;
                        quantityString = string.Empty;
                    }
                }

                GUILayout.EndHorizontal();

                // Character Stats ------------------------------------------------
                CharacterStats currentStats = m_CharacterData.GetCharacterProgressInSlot(currentPlayerID).m_Stats;

                float leftStart = Screen.width - 10f - 200f;
                GUI.Label(new Rect(leftStart, 10f, 200f, 30f), "- CHARACTER -");
                GUI.Label(new Rect(leftStart, 30f, 200f, 30f), "Gender: " + currentStats.Gender + "   " + "Name: " + currentStats.PlayerName);
                GUI.Label(new Rect(leftStart, 50f, 200f, 30f), "Race: " + currentStats.Race + "   " + "Class: " + currentStats.Class);

                GUI.Label(new Rect(leftStart, 90f, 200f, 30f), "- STATS -");
                GUI.Label(new Rect(leftStart, 110f, 200f, 30f), "HP: " + currentStats.HP);
                GUI.Label(new Rect(leftStart, 130f, 200f, 30f), "MP: " + currentStats.MP);
                GUI.Label(new Rect(leftStart, 150f, 200f, 30f), "ATT: " + currentStats.ATT);
                GUI.Label(new Rect(leftStart, 170f, 200f, 30f), "DEF: " + currentStats.DEF);
                GUI.Label(new Rect(leftStart, 190f, 200f, 30f), "ACC: " + currentStats.ACC);
                GUI.Label(new Rect(leftStart, 210f, 200f, 30f), "MGC: " + currentStats.MGC);
                GUI.Label(new Rect(leftStart, 230f, 200f, 30f), "EVN: " + currentStats.EVN);
                GUI.Label(new Rect(leftStart, 250f, 200f, 30f), "SPD: " + currentStats.SPD);
                GUI.Label(new Rect(leftStart, 270f, 200f, 30f), "LCK: " + currentStats.LCK);

                if (GUI.Button(new Rect(leftStart, 300f, 200f, 30f), "Randomize"))
                {
                    currentStats.Init();
                    SaveCharacterProgress();
                }
            }
        }
    }
#endif
    #endregion
}
