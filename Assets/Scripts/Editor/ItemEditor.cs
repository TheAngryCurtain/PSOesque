using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemEditor : EditorWindow
{
    private static int itemID = 0;

    private List<InventoryItem> ItemList;
    private ItemContainer ItemContainer;
    private string ItemXMLPath;
    private InventoryItem CurrentItem = null;

    private bool IsCreating = false;
    private bool IsEditing = false;

    #region Create Item Fields
    private string newItemName ;
    private Sprite newItemIcon;
    private string newItemDesc;
    private Enums.eItemType newItemType;
    private int newItemValue;
    private Enums.eRarity newItemRarity;

    // consumable
    private Enums.eConsumableType newConsumableType;
    private Enums.eConsumableStatType newRecoveryStatType;
    private int recoveryAmount;
    private Enums.eStatType newStatType;
    private int statBoostAmount;
    private Enums.eStatusEffect newEffectType;
    private int weaponUpgradeAmount;

    // stat boosts
    private Enums.eStatBoostType newBoostType;
    private Enums.eStatType StatToBoost;
    private int BoostAmount;
    private Enums.eLongTermEffectType LongTermEffect;
    private float LongTermDelay;
    private int LongTermAmount;
    private Enums.eStatusEffect ResistEffect;
    private float ResistPercent;

    // equippables
    private Enums.eMinEquipRequirementType MinEquipType;
    private int MinLevelToEquip;
    private Enums.eStatType MinStatType;
    private int MinStatLevel;
    private List<Enums.eClassType> UsableClasses;
    private List<Enums.eRaceType> UsableRaces;

    // armours
    private Enums.eArmourLocation newArmourType;
    private int BodySlots;

    // weapons
    private int BaseWeaponDamage;
    private Enums.eStatusEffect WeaponEffect;
    private bool WeaponMultiTarget;
    private Enums.eWeaponRangeType WeaponRangeType;
    private bool MeleeTwoHanded;
    private float RangedRange;
    #endregion

    #region Edit Item Fields
    private int SelectedItemIndex = 0;
    private string[] ItemOptions;
    private InventoryItem SelectedItem;
    private Sprite SelectedItemSprite = null;
    #endregion

    [MenuItem("Tools/Item Editor")]
    private static void Init()
    {
        ItemEditor window = (ItemEditor)EditorWindow.GetWindow(typeof(ItemEditor));
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 400f, 700f);
        window.Show();
    }

    private void Awake()
    {
        ItemXMLPath = Application.streamingAssetsPath + "/XML/ItemDatabase.xml";

        Load();
        Reset();
    }

    private void Load()
    {
        try
        {
            ItemContainer = XMLSerializer.Deserialize<ItemContainer>(ItemXMLPath);
        }
        catch (System.IO.FileNotFoundException e)
        {
            Debug.LogWarningFormat("Unable to Find Items XML. Creating new File.");
            ItemContainer = new ItemContainer();
        }

        ItemList = ItemContainer.m_Items;

        // set up edit stuff
        ItemOptions = new string[ItemList.Count];
        for (int i = 0; i < ItemList.Count; i++)
        {
            ItemOptions[i] = string.Format("[{0}] {1}", ItemList[i].ID, ItemList[i].Name);
        }
    }

    private void Save()
    {
        XMLSerializer.Serialize((ItemContainer)ItemContainer, ItemXMLPath);
        Reset();
    }

    private void Reset()
    {
        CurrentItem = new InventoryItem();

        newItemName = "New Item";
        newItemIcon = null;
        newItemDesc = "New Description";
        newItemType = Enums.eItemType.Consumable;
        newItemValue = 0;
        newItemRarity = Enums.eRarity.Common;

        // consumable
        newConsumableType = Enums.eConsumableType.Recovery;
        newRecoveryStatType = Enums.eConsumableStatType.HP;
        recoveryAmount = 0;
        newStatType = Enums.eStatType.HP;
        statBoostAmount = 0;
        newEffectType = Enums.eStatusEffect.All;
        weaponUpgradeAmount = 0;

        // stat boosts
        newBoostType = Enums.eStatBoostType.Basic;
        StatToBoost = Enums.eStatType.HP;
        BoostAmount = 0;
        LongTermEffect = Enums.eLongTermEffectType.HP;
        LongTermDelay = 0f;
        LongTermAmount = 0;
        ResistEffect = Enums.eStatusEffect.All;
        ResistPercent = 0f;

        // equippables
        MinEquipType = Enums.eMinEquipRequirementType.None;
        MinLevelToEquip = 0;
        MinStatType = Enums.eStatType.HP;
        MinStatLevel = 0;
        UsableClasses = new List<Enums.eClassType>((int)Enums.eClassType.All);
        UsableRaces = new List<Enums.eRaceType>((int)Enums.eRaceType.All);

        // armours
        newArmourType = Enums.eArmourLocation.Head;
        BodySlots = 0;

        // weapons
        BaseWeaponDamage = 0;
        WeaponEffect = Enums.eStatusEffect.None;
        WeaponMultiTarget = false;
        WeaponRangeType = Enums.eWeaponRangeType.Melee;
        MeleeTwoHanded = false;
        RangedRange = 0f;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button((IsCreating ? "Creating" : "Create")))
        {
            IsEditing = false;
            IsCreating = !IsCreating;
        }

        if (GUILayout.Button((IsEditing ? "Editing" : "Edit")))
        {
            IsCreating = false;
            IsEditing = !IsEditing;
        }
        GUILayout.EndHorizontal();

        if (IsCreating)
        {
            ShowCreateItem();
        }

        if (IsEditing)
        {
            ShowEditItem();
        }

        if (GUILayout.Button("Save"))
        {
            Save();
        }

        GUILayout.Label("Total Items: " + ItemList.Count, EditorStyles.boldLabel);
    }

    private void ShowCreateItem()
    {
        GUILayout.Label("Create Item", EditorStyles.boldLabel);
        newItemIcon = (Sprite)EditorGUILayout.ObjectField("Icon", newItemIcon, typeof(Sprite), false);
        newItemName = EditorGUILayout.TextField("Name: ", newItemName);

        EditorGUILayout.PrefixLabel("Description");
        EditorStyles.textField.wordWrap = true;
        newItemDesc = EditorGUILayout.TextArea(newItemDesc, GUILayout.MinHeight(60f));
        newItemValue = EditorGUILayout.IntField("Worth: ", newItemValue);
        newItemRarity = (Enums.eRarity)EditorGUILayout.EnumPopup("Rarity: ", newItemRarity);

        GUILayout.Label("Item Type", EditorStyles.boldLabel);
        newItemType = (Enums.eItemType)EditorGUILayout.EnumPopup("Type: ", newItemType);

        switch (newItemType)
        {
            case Enums.eItemType.Consumable:
                ShowConsumables();
                break;

            case Enums.eItemType.Companion:
                ShowCompanions();
                break;

            case Enums.eItemType.StatBoost:
                ShowStatBoosts();
                break;

            case Enums.eItemType.Armour:
                ShowArmours();
                break;

            case Enums.eItemType.Weapon:
                ShowWeapons();
                break;

            case Enums.eItemType.Money:
            default:
                break;
        }

        if (GUILayout.Button("Create Item"))
        {
            CreateItem();
        }
    }

    private void CreateItem()
    {
        System.Type currentType = CurrentItem.GetType();

        // fill in required fields
        #region Consumables
        if (currentType == typeof(RecoveryItem))
        {
            RecoveryItem item = new RecoveryItem();
            item.ID = itemID;
            item.ConsumableType = newRecoveryStatType;
            item.Amount = recoveryAmount;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            ItemList.Add(item);
        }
        else if (currentType == typeof(StatUpgradeItem))
        {
            StatUpgradeItem item = new StatUpgradeItem();
            item.ID = itemID;
            item.StatType = newStatType;
            item.Amount = statBoostAmount;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            ItemList.Add(item);
        }
        else if (currentType == typeof(StatusEffectItem))
        {
            StatusEffectItem item = new StatusEffectItem();
            item.ID = itemID;
            item.Effect = newEffectType;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            ItemList.Add(item);
        }
        else if (currentType == typeof(WeaponUpgradeItem))
        {
            WeaponUpgradeItem item = new WeaponUpgradeItem();
            item.ID = itemID;
            item.Amount = weaponUpgradeAmount;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            ItemList.Add(item);
        }
        #endregion

        #region Companions
        else if (currentType == typeof(CompanionItem))
        {
            CompanionItem item = new CompanionItem();
            item.ID = itemID;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        #endregion

        #region Stat Boosts
        else if (currentType == typeof(StatBoostItem))
        {
            StatBoostItem item = new StatBoostItem();
            item.ID = itemID;

            item.Stat = StatToBoost;
            item.Amount = BoostAmount;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        else if (currentType == typeof(LongTermEffectItem))
        {
            LongTermEffectItem item = new LongTermEffectItem();
            item.ID = itemID;

            item.EffectType = LongTermEffect;
            item.Delay = LongTermDelay;
            item.Amount = LongTermAmount;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        else if (currentType == typeof(ResistItem))
        {
            ResistItem item = new ResistItem();
            item.ID = itemID;

            item.Effect = ResistEffect;
            item.Percent = ResistPercent;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        #endregion

        #region Armours
        else if (currentType == typeof(HeadArmourItem))
        {
            HeadArmourItem item = new HeadArmourItem();
            item.ID = itemID;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        else if (currentType == typeof(BodyArmourItem))
        {
            BodyArmourItem item = new BodyArmourItem();
            item.ID = itemID;

            item.Slots = BodySlots;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        else if (currentType == typeof(ArmArmourItem))
        {
            ArmArmourItem item = new ArmArmourItem();
            item.ID = itemID;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        #endregion

        #region Weapons
        else if (currentType == typeof(MeleeWeaponItem))
        {
            MeleeWeaponItem item = new MeleeWeaponItem();
            item.ID = itemID;

            item.TwoHanded = MeleeTwoHanded;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            item.UsableClasses = new List<Enums.eClassType>();
            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            item.UsableRaces = new List<Enums.eRaceType>();
            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        else if (currentType == typeof(RangedWeaponItem))
        {
            RangedWeaponItem item = new RangedWeaponItem();
            item.ID = itemID;

            item.Range = RangedRange;

            // base item
            item.Name = newItemName;
            item.Description = newItemDesc;
            item.Type = newItemType;
            item.IconName = newItemIcon.name;
            item.Value = newItemValue;
            item.Rarity = newItemRarity;

            // equippable
            item.MinEquipType = MinEquipType;
            switch (MinEquipType)
            {
                case Enums.eMinEquipRequirementType.None:
                default:
                    break;

                case Enums.eMinEquipRequirementType.Stat:
                    item.StatType = MinStatType;
                    item.MinStatValueToEquip = MinStatLevel;
                    break;

                case Enums.eMinEquipRequirementType.Level:
                    item.MinLevelToEquip = MinLevelToEquip;
                    break;
            }

            for (int i = 0; i < UsableClasses.Count; i++)
            {
                item.UsableClasses.Add(UsableClasses[i]);
            }

            for (int i = 0; i < UsableRaces.Count; i++)
            {
                item.UsableRaces.Add(UsableRaces[i]);
            }

            ItemList.Add(item);
        }
        #endregion

        itemID += 1;
        Reset();
    }

    private void ShowEditItem()
    {
        SelectedItemIndex = EditorGUILayout.Popup("Item: ", SelectedItemIndex, ItemOptions);
        SelectedItem = ItemList[SelectedItemIndex];

        System.Type currentType = SelectedItem.GetType();
        if (currentType == typeof(RecoveryItem))
        {
            RecoveryItem item = (RecoveryItem)SelectedItem;

            // base item
            item.Name = EditorGUILayout.TextField("Name: ", item.Name);
            if (SelectedItemSprite == null)
            {
                SelectedItemSprite = Resources.Load<Sprite>( string.Format("Icons/{0}", item.IconName));
            }
            else
            {
                SelectedItemSprite = (Sprite)EditorGUILayout.ObjectField("Icon", SelectedItemSprite, typeof(Sprite), false);
                item.IconName = SelectedItemSprite.name;
            }

            EditorGUILayout.PrefixLabel("Description");
            EditorStyles.textField.wordWrap = true;
            item.Description = EditorGUILayout.TextArea(item.Description, GUILayout.MinHeight(60f));
            item.Value = EditorGUILayout.FloatField("Worth: ", item.Value);
            item.Rarity = (Enums.eRarity)EditorGUILayout.EnumPopup("Rarity: ", item.Rarity);

            item.ConsumableType = (Enums.eConsumableStatType)EditorGUILayout.EnumPopup("Stat Type: ", item.ConsumableType);
            item.Amount = EditorGUILayout.IntField("Amount: ", item.Amount);
        }

        // TODO all of the other item types
    }

    private void ShowConsumables()
    {
        GUILayout.Label("Consumables", EditorStyles.boldLabel);
        newConsumableType = (Enums.eConsumableType)EditorGUILayout.EnumPopup("Consumable: ", newConsumableType);
        switch (newConsumableType)
        {
            case Enums.eConsumableType.Recovery:
                    newRecoveryStatType = (Enums.eConsumableStatType)EditorGUILayout.EnumPopup("Stat Type: ", newRecoveryStatType);
                    recoveryAmount = EditorGUILayout.IntField("Amount: ", recoveryAmount);

                if (CurrentItem.GetType() != typeof(RecoveryItem))
                {
                    CurrentItem = new RecoveryItem();
                }
                break;

            case Enums.eConsumableType.StatUpgrade:
                    newStatType = (Enums.eStatType)EditorGUILayout.EnumPopup("Boost Stat: ", newStatType);
                    statBoostAmount = EditorGUILayout.IntField("Amount: ", statBoostAmount);

                if (CurrentItem.GetType() != typeof(StatUpgradeItem))
                {
                    CurrentItem = new StatUpgradeItem();
                }
                break;

            case Enums.eConsumableType.StatusEffect:
                    newEffectType = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", newEffectType);

                if (CurrentItem.GetType() != typeof(StatusEffectItem))
                {
                    CurrentItem = new StatusEffectItem();
                }
                break;

            case Enums.eConsumableType.WeaponUpgrade:
                    weaponUpgradeAmount = EditorGUILayout.IntField("Amount: ", weaponUpgradeAmount);

                if (CurrentItem.GetType() != typeof(WeaponUpgradeItem))
                {
                    CurrentItem = new WeaponUpgradeItem();
                }
                break;
        }
    }

    private void ShowCompanions()
    {
        ShowEquippables();

        GUILayout.Label("Companions", EditorStyles.boldLabel);

        if (CurrentItem.GetType() != typeof(CompanionItem))
        {
            CurrentItem = new CompanionItem();
        }
    }

    private void ShowStatBoosts()
    {
        ShowEquippables();

        GUILayout.Label("Stat Boosts", EditorStyles.boldLabel);
        newBoostType = (Enums.eStatBoostType)EditorGUILayout.EnumPopup("Boost Type: ", newBoostType);
        switch (newBoostType)
        {
            case Enums.eStatBoostType.Basic:
                StatToBoost = (Enums.eStatType)EditorGUILayout.EnumPopup("Stat: ", StatToBoost);
                BoostAmount = EditorGUILayout.IntField("Amount: ", BoostAmount);

                if (CurrentItem.GetType() != typeof(StatBoostItem))
                {
                    CurrentItem = new StatBoostItem();
                }
                break;

            case Enums.eStatBoostType.LongTerm:
                LongTermEffect = (Enums.eLongTermEffectType)EditorGUILayout.EnumPopup("Effect: ", LongTermEffect);
                LongTermDelay = EditorGUILayout.FloatField("Interval: ", LongTermDelay);
                LongTermAmount = EditorGUILayout.IntField("Amount: ", LongTermAmount);

                if (CurrentItem.GetType() != typeof(LongTermEffectItem))
                {
                    CurrentItem = new LongTermEffectItem();
                }
                break;

            case Enums.eStatBoostType.Resist:
                ResistEffect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", ResistEffect);
                ResistPercent = EditorGUILayout.FloatField("Percent: ", ResistPercent);

                if (CurrentItem.GetType() != typeof(ResistItem))
                {
                    CurrentItem = new ResistItem();
                }
                break;
        }
    }

    private void ShowEquippables()
    {
        GUILayout.Label("Equippable", EditorStyles.boldLabel);
        MinEquipType = (Enums.eMinEquipRequirementType)EditorGUILayout.EnumPopup("Minimum Requirement: ", MinEquipType);
        MinLevelToEquip = EditorGUILayout.IntField("Minimum Level: ", MinLevelToEquip);
        MinStatType = (Enums.eStatType)EditorGUILayout.EnumPopup("Limiting Stat: ", MinStatType);
        MinStatLevel = EditorGUILayout.IntField("Min Stat Level: ", MinStatLevel);

        for (int i = 0; i < UsableClasses.Count; i++)
        {
            UsableClasses[i] = (Enums.eClassType)EditorGUILayout.EnumPopup("Usable By (Class): ", UsableClasses[i]);
        }

        if (GUILayout.Button("Add Usable Class"))
        {
            UsableClasses.Add(Enums.eClassType.Melee);
        }

        for (int i = 0; i < UsableRaces.Count; i++)
        {
            UsableRaces[i] = (Enums.eRaceType)EditorGUILayout.EnumPopup("Usable By (Race): ", UsableRaces[i]);
        }

        if (GUILayout.Button("Add Usable Race"))
        {
            UsableRaces.Add(Enums.eRaceType.Human);
        }
    }

    private void ShowArmours()
    {
        ShowEquippables();

        GUILayout.Label("Armour", EditorStyles.boldLabel);
        newArmourType = (Enums.eArmourLocation)EditorGUILayout.EnumPopup("Armour Type: ", newArmourType);
        switch (newArmourType)
        {
            case Enums.eArmourLocation.Head:
                if (CurrentItem.GetType() != typeof(HeadArmourItem))
                {
                    CurrentItem = new HeadArmourItem();
                }
                break;

            case Enums.eArmourLocation.Body:
                BodySlots = EditorGUILayout.IntField("# Slots: ", BodySlots);
                if (CurrentItem.GetType() != typeof(BodyArmourItem))
                {
                    CurrentItem = new BodyArmourItem();
                }
                break;

            case Enums.eArmourLocation.Arm:
                if (CurrentItem.GetType() != typeof(ArmArmourItem))
                {
                    CurrentItem = new ArmArmourItem();
                }
                break;
        }
    }

    private void ShowWeapons()
    {
        ShowEquippables();

        GUILayout.Label("Weapon", EditorStyles.boldLabel);
        BaseWeaponDamage = EditorGUILayout.IntField("Base Damage: ", BaseWeaponDamage);
        WeaponEffect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", WeaponEffect);
        WeaponMultiTarget = EditorGUILayout.Toggle("Multiple Targets?: ", WeaponMultiTarget);

        WeaponRangeType = (Enums.eWeaponRangeType)EditorGUILayout.EnumPopup("Weapon Type: ", WeaponRangeType);
        switch (WeaponRangeType)
        {
            case Enums.eWeaponRangeType.Melee:
                MeleeTwoHanded = EditorGUILayout.Toggle("Two Handed?: ", MeleeTwoHanded);
                if (CurrentItem.GetType() != typeof(MeleeWeaponItem))
                {
                    CurrentItem = new MeleeWeaponItem();
                }
                break;

            case Enums.eWeaponRangeType.Ranged:
                RangedRange = EditorGUILayout.FloatField("Range: ", RangedRange);
                if (CurrentItem.GetType() != typeof(RangedWeaponItem))
                {
                    CurrentItem = new RangedWeaponItem();
                }
                break;
        }
    }
}
