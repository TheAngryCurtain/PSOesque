using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConfirmPopup : EditorWindow
{
    public System.Action<bool> OnResultClicked;

    private string mMessage;

    public static ConfirmPopup Create()
    {
        ConfirmPopup window = ScriptableObject.CreateInstance<ConfirmPopup>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 200, 75);
        window.Show();

        return window;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField(mMessage, EditorStyles.wordWrappedLabel);
        GUILayout.Space(70);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Yes"))
        {
            OnResultClicked(true);
        }

        if (GUILayout.Button("No"))
        {
            OnResultClicked(false);
        }

        EditorGUILayout.EndHorizontal();
    }

    public void SetMessage(string message)
    {
        mMessage = message;
    }
}

public class ItemEditor : EditorWindow
{
    private static int itemID = 0;

    private List<InventoryItem> ItemList;
    private ItemContainer ItemContainer;
    private string ItemXMLPath;
    private InventoryItem mCurrentItem = null;

    private bool mIsCreating = false;
    private bool mIsEditing = false;

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
    private Enums.eWeaponType WeaponRangeType;
    private bool MeleeTwoHanded;
    private float Range;
    private Enums.eMagicFocusType MagicFocusType;
    private Enums.eMagicType MagicType;
    private float MagicRadius;
    private Enums.eStatusEffect MagicEffect;
    private int MagicCost;
    #endregion

    #region Edit Item Fields
    private int mSelectedItemIndex = 0;
    private string[] mItemOptions;
    private InventoryItem mSelectedItem;
    private Sprite mSelectedItemSprite = null;
    #endregion

    private ConfirmPopup mConfirmPopup;

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
            itemID = ItemContainer.m_Items.Count;
        }
        catch (System.IO.FileNotFoundException e)
        {
            Debug.LogWarningFormat("Unable to Find Items XML. Creating new File.");
            ItemContainer = new ItemContainer();
        }

        ItemList = ItemContainer.m_Items;
        PopulateOptionItems();
    }

    private void PopulateOptionItems()
    {
        mItemOptions = new string[ItemList.Count];
        for (int i = 0; i < ItemList.Count; i++)
        {
            mItemOptions[i] = string.Format("[{0}] {1}", ItemList[i].ID, ItemList[i].Name);
        }
    }

    private void Save()
    {
        XMLSerializer.Serialize((ItemContainer)ItemContainer, ItemXMLPath);
        Reset();
    }

    private void Reset()
    {
        mCurrentItem = new InventoryItem();

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
        WeaponRangeType = Enums.eWeaponType.Melee;
        MeleeTwoHanded = false;
        Range = 0f;
        MagicFocusType = Enums.eMagicFocusType.Self;
        MagicType = Enums.eMagicType.Support;
        MagicRadius = 1f;
        MagicEffect = Enums.eStatusEffect.None;
        MagicCost = 0;
}

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button((mIsCreating ? "Creating" : "Create")))
        {
            mIsEditing = false;
            mIsCreating = !mIsCreating;
        }

        if (GUILayout.Button((mIsEditing ? "Editing" : "Edit")))
        {
            mIsCreating = false;

            if (!mIsEditing)
            {
                mSelectedItemIndex = 0;
                PopulateOptionItems();
            }

            mIsEditing = !mIsEditing;
        }
        GUILayout.EndHorizontal();

        if (mIsCreating)
        {
            ShowCreateItem();
        }

        if (mIsEditing)
        {
            ShowEditItem();
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
        newItemValue = EditorGUILayout.IntField("Worth: ", Mathf.Clamp(newItemValue, 0, 999));
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
            Save();
        }
    }

    private void CreateItem()
    {
        System.Type currentType = mCurrentItem.GetType();

        // fill in required fields
        #region Consumables
        if (currentType == typeof(RecoveryItem))
        {
            RecoveryItem item = new RecoveryItem(itemID);
            item.ConsumableType = newRecoveryStatType;
            item.Amount = recoveryAmount;

            SetItemBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(StatUpgradeItem))
        {
            StatUpgradeItem item = new StatUpgradeItem(itemID);
            item.StatType = newStatType;
            item.Amount = statBoostAmount;

            SetItemBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(StatusEffectItem))
        {
            StatusEffectItem item = new StatusEffectItem(itemID);
            item.Effect = newEffectType;

            SetItemBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(WeaponUpgradeItem))
        {
            WeaponUpgradeItem item = new WeaponUpgradeItem(itemID);
            item.Amount = weaponUpgradeAmount;

            SetItemBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(CharacterSupportItem))
        {
            // TODO
        }
        #endregion

        #region Companions
        else if (currentType == typeof(CompanionItem))
        {
            CompanionItem item = new CompanionItem(itemID);

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        #endregion

        #region Stat Boosts
        else if (currentType == typeof(StatBoostItem))
        {
            StatBoostItem item = new StatBoostItem(itemID);

            item.Stat = StatToBoost;
            item.Amount = BoostAmount;

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(LongTermEffectItem))
        {
            LongTermEffectItem item = new LongTermEffectItem(itemID);

            item.EffectType = LongTermEffect;
            item.Delay = LongTermDelay;
            item.Amount = LongTermAmount;

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(ResistItem))
        {
            ResistItem item = new ResistItem(itemID);

            item.Effect = ResistEffect;
            item.Percent = ResistPercent;

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        #endregion

        #region Armours
        else if (currentType == typeof(HeadArmourItem))
        {
            HeadArmourItem item = new HeadArmourItem(itemID);

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(BodyArmourItem))
        {
            BodyArmourItem item = new BodyArmourItem(itemID);

            item.Slots = BodySlots;

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(ArmArmourItem))
        {
            ArmArmourItem item = new ArmArmourItem(itemID);

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        #endregion

        #region Weapons
        else if (currentType == typeof(MeleeWeaponItem))
        {
            MeleeWeaponItem item = new MeleeWeaponItem(itemID);

            item.TwoHanded = MeleeTwoHanded;

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(RangedWeaponItem))
        {
            RangedWeaponItem item = new RangedWeaponItem(itemID);

            item.Range = Range;

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        else if (currentType == typeof(MagicWeaponItem))
        {
            MagicWeaponItem item = new MagicWeaponItem(itemID);

            item.FocusType = MagicFocusType;
            item.MagicType = MagicType;
            item.Radius = MagicRadius;
            item.MagicEffect = MagicEffect;
            item.MPCost = MagicCost;

            SetItemBase(item);
            SetEquippableBase(item);

            ItemList.Add(item);
        }
        #endregion

        itemID += 1;
        Reset();
    }

    private void SetItemBase(InventoryItem item)
    {
        // base item
        item.Name = newItemName;
        item.Description = newItemDesc;
        item.Type = newItemType;
        item.IconName = newItemIcon.name;
        item.Value = newItemValue;
        item.Rarity = newItemRarity;
    }

    private void SetEquippableBase(EquippableItem item)
    {
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
    }

    private void ShowEditItem()
    {
        mSelectedItemIndex = EditorGUILayout.Popup("Item: ", mSelectedItemIndex, mItemOptions);
        InventoryItem nextItem = ItemList[mSelectedItemIndex];
        if (nextItem != mSelectedItem)
        {
            // clear the sprite so it will be reloaded
            mSelectedItemSprite = null;
            mSelectedItem = ItemList[mSelectedItemIndex];
        }

        System.Type currentType = mSelectedItem.GetType();
        #region Consumables
        if (currentType == typeof(RecoveryItem))
        {
            RecoveryItem item = (RecoveryItem)mSelectedItem;

            ShowEditItemBase(item);

            // recovery
            item.ConsumableType = (Enums.eConsumableStatType)EditorGUILayout.EnumPopup("Stat Type: ", item.ConsumableType);
            item.Amount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(item.Amount, 0, 999));
        }
        else if (currentType == typeof(StatUpgradeItem))
        {
            StatUpgradeItem item = (StatUpgradeItem)mSelectedItem;

            ShowEditItemBase(item);

            // stat upgrade
            item.StatType = (Enums.eStatType)EditorGUILayout.EnumPopup("Boost Stat: ", item.StatType);
            item.Amount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(item.Amount, 0, 999));
        }
        else if (currentType == typeof(StatusEffectItem))
        {
            StatusEffectItem item = (StatusEffectItem)mSelectedItem;

            ShowEditItemBase(item);

            // status effect
            item.Effect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", item.Effect);
        }
        else if (currentType == typeof(WeaponUpgradeItem))
        {
            WeaponUpgradeItem item = (WeaponUpgradeItem)mSelectedItem;

            ShowEditItemBase(item);

            // weapon upgrade
            item.Amount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(item.Amount, 0, 999));
        }
        else if (currentType == typeof(CharacterSupportItem))
        {
            // TODO
        }
        #endregion

        #region Companions
        else if (currentType == typeof(CompanionItem))
        {
            CompanionItem item = (CompanionItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // companion
        }
        #endregion

        #region Stat Boosts
        else if (currentType == typeof(StatBoostItem))
        {
            StatBoostItem item = (StatBoostItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // stat boost
            item.StatType = (Enums.eStatType)EditorGUILayout.EnumPopup("Stat: ", item.StatType);
            item.Amount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(item.Amount, 0, 999));
        }
        else if (currentType == typeof(LongTermEffectItem))
        {
            LongTermEffectItem item = (LongTermEffectItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // long term effect
            item.EffectType = (Enums.eLongTermEffectType)EditorGUILayout.EnumPopup("Effect: ", item.EffectType);
            item.Delay = EditorGUILayout.FloatField("Interval: ", Mathf.Clamp(item.Delay, 0, 10));
            item.Amount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(item.Amount, 0, 999));
        }
        else if (currentType == typeof(ResistItem))
        {
            ResistItem item = (ResistItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // resist
            item.Effect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", item.Effect);
            item.Percent = EditorGUILayout.FloatField("Percent: ", Mathf.Clamp(item.Percent, 0, 100));
        }
        #endregion

        #region Armours
        else if (currentType == typeof(HeadArmourItem))
        {
            HeadArmourItem item = (HeadArmourItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // head armour
        }
        else if (currentType == typeof(BodyArmourItem))
        {
            BodyArmourItem item = (BodyArmourItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // body armour
            item.Slots = EditorGUILayout.IntField("# Slots: ", Mathf.Clamp(item.Slots, 0, 5));
        }
        else if (currentType == typeof(ArmArmourItem))
        {
            ArmArmourItem item = (ArmArmourItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // arm armour
        }
        #endregion

        #region Weapons
        else if (currentType == typeof(MeleeWeaponItem))
        {
            MeleeWeaponItem item = (MeleeWeaponItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // melee weapon
            item.TwoHanded = EditorGUILayout.Toggle("Two Handed?: ", item.TwoHanded);
        }
        else if (currentType == typeof(RangedWeaponItem))
        {
            RangedWeaponItem item = (RangedWeaponItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // ranged weapon
            item.Range = EditorGUILayout.FloatField("Range: ", Mathf.Clamp(item.Range, 0, 99));
        }
        else if (currentType == typeof(MagicWeaponItem))
        {
            MagicWeaponItem item = (MagicWeaponItem)mSelectedItem;

            ShowEditItemBase(item);
            ShowEditEquippableBase(item);

            // magic weapon
            item.FocusType = (Enums.eMagicFocusType)EditorGUILayout.EnumPopup("Focus: ", item.FocusType);
            item.MagicType = (Enums.eMagicType)EditorGUILayout.EnumPopup("Type: ", item.MagicType);
            item.Radius = EditorGUILayout.FloatField("Radius: ", Mathf.Clamp(item.Radius, 0, 99));
            item.MagicEffect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", item.MagicEffect);
            item.MPCost = EditorGUILayout.IntField("Cost: ", Mathf.Clamp(item.MPCost, 0, 999));
        }
        #endregion

        if (GUILayout.Button("Delete Item"))
        {
            mConfirmPopup = ConfirmPopup.Create();
            mConfirmPopup.OnResultClicked += OnPopupClosed;
            mConfirmPopup.SetMessage("Are you sure you'd like to delete this item?");
        }

        if (GUILayout.Button("Update"))
        {
            Save();
        }
    }

    private void OnPopupClosed(bool result)
    {
        mConfirmPopup.OnResultClicked -= OnPopupClosed;
        if (result)
        {
            ItemList.Remove(mSelectedItem);
            mSelectedItemIndex = 0;
            PopulateOptionItems();

            Save();
        }

        mConfirmPopup.Close();
        mConfirmPopup = null;
    }

    private void ShowEditItemBase(InventoryItem item)
    {
        // base item
        item.Name = EditorGUILayout.TextField("Name: ", item.Name);
        if (mSelectedItemSprite == null)
        {
            mSelectedItemSprite = Resources.Load<Sprite>(string.Format("Icons/{0}", item.IconName));
        }
        else
        {
            mSelectedItemSprite = (Sprite)EditorGUILayout.ObjectField("Icon", mSelectedItemSprite, typeof(Sprite), false);
            item.IconName = mSelectedItemSprite.name;
        }

        EditorGUILayout.PrefixLabel("Description");
        EditorStyles.textField.wordWrap = true;
        item.Description = EditorGUILayout.TextArea(item.Description, GUILayout.MinHeight(60f));
        GUILayout.Label("Type: " + item.Type, EditorStyles.label);
        item.Value = EditorGUILayout.FloatField("Worth: ", Mathf.Clamp(item.Value, 0, 999));
        item.Rarity = (Enums.eRarity)EditorGUILayout.EnumPopup("Rarity: ", item.Rarity);
    }

    private void ShowEditEquippableBase(EquippableItem item)
    {
        // equippable
        GUILayout.Label("Equippable", EditorStyles.boldLabel);
        item.MinEquipType = (Enums.eMinEquipRequirementType)EditorGUILayout.EnumPopup("Minimum Requirement: ", item.MinEquipType);
        item.MinLevelToEquip = EditorGUILayout.IntField("Minimum Level: ", Mathf.Clamp(item.MinLevelToEquip, 0, 99));
        item.StatType = (Enums.eStatType)EditorGUILayout.EnumPopup("Limiting Stat: ", item.StatType);
        item.MinStatValueToEquip = EditorGUILayout.IntField("Min Stat Level: ", Mathf.Clamp(item.MinStatValueToEquip, 0, 999));

        for (int i = 0; i < item.UsableClasses.Count; i++)
        {
            item.UsableClasses[i] = (Enums.eClassType)EditorGUILayout.EnumPopup("Usable By (Class): ", item.UsableClasses[i]);
        }

        for (int i = 0; i < item.UsableRaces.Count; i++)
        {
            item.UsableRaces[i] = (Enums.eRaceType)EditorGUILayout.EnumPopup("Usable By (Race): ", item.UsableRaces[i]);
        }
    }

    private void ShowConsumables()
    {
        GUILayout.Label("Consumables", EditorStyles.boldLabel);
        newConsumableType = (Enums.eConsumableType)EditorGUILayout.EnumPopup("Consumable: ", newConsumableType);
        switch (newConsumableType)
        {
            case Enums.eConsumableType.Recovery:
                newRecoveryStatType = (Enums.eConsumableStatType)EditorGUILayout.EnumPopup("Stat Type: ", newRecoveryStatType);
                recoveryAmount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(recoveryAmount, 0, 99));

                if (mCurrentItem.GetType() != typeof(RecoveryItem))
                {
                    mCurrentItem = new RecoveryItem();
                }
                break;

            case Enums.eConsumableType.StatUpgrade:
                newStatType = (Enums.eStatType)EditorGUILayout.EnumPopup("Boost Stat: ", newStatType);
                statBoostAmount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(statBoostAmount, 0, 99));

                if (mCurrentItem.GetType() != typeof(StatUpgradeItem))
                {
                    mCurrentItem = new StatUpgradeItem();
                }
                break;

            case Enums.eConsumableType.StatusEffect:
                newEffectType = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", newEffectType);

                if (mCurrentItem.GetType() != typeof(StatusEffectItem))
                {
                    mCurrentItem = new StatusEffectItem();
                }
                break;

            case Enums.eConsumableType.WeaponUpgrade:
                weaponUpgradeAmount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(weaponUpgradeAmount, 0, 99));

                if (mCurrentItem.GetType() != typeof(WeaponUpgradeItem))
                {
                    mCurrentItem = new WeaponUpgradeItem();
                }
                break;

            case Enums.eConsumableType.CharacterSupport:
                // TODO
                break;
        }
    }

    private void ShowCompanions()
    {
        ShowEquippables();

        GUILayout.Label("Companions", EditorStyles.boldLabel);

        if (mCurrentItem.GetType() != typeof(CompanionItem))
        {
            mCurrentItem = new CompanionItem();
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
                BoostAmount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(BoostAmount, 0, 99));

                if (mCurrentItem.GetType() != typeof(StatBoostItem))
                {
                    mCurrentItem = new StatBoostItem();
                }
                break;

            case Enums.eStatBoostType.LongTerm:
                LongTermEffect = (Enums.eLongTermEffectType)EditorGUILayout.EnumPopup("Effect: ", LongTermEffect);
                LongTermDelay = EditorGUILayout.FloatField("Interval: ", Mathf.Clamp(LongTermDelay, 0, 10));
                LongTermAmount = EditorGUILayout.IntField("Amount: ", Mathf.Clamp(LongTermAmount, 0, 99));

                if (mCurrentItem.GetType() != typeof(LongTermEffectItem))
                {
                    mCurrentItem = new LongTermEffectItem();
                }
                break;

            case Enums.eStatBoostType.Resist:
                ResistEffect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", ResistEffect);
                ResistPercent = EditorGUILayout.FloatField("Percent: ", Mathf.Clamp(ResistPercent, 0, 100));

                if (mCurrentItem.GetType() != typeof(ResistItem))
                {
                    mCurrentItem = new ResistItem();
                }
                break;
        }
    }

    private void ShowEquippables()
    {
        GUILayout.Label("Equippable", EditorStyles.boldLabel);
        MinEquipType = (Enums.eMinEquipRequirementType)EditorGUILayout.EnumPopup("Minimum Requirement: ", MinEquipType);
        MinLevelToEquip = EditorGUILayout.IntField("Minimum Level: ", Mathf.Clamp(MinLevelToEquip, 0, 99));
        MinStatType = (Enums.eStatType)EditorGUILayout.EnumPopup("Limiting Stat: ", MinStatType);
        MinStatLevel = EditorGUILayout.IntField("Min Stat Level: ", Mathf.Clamp(MinStatLevel, 0, 999));

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
                if (mCurrentItem.GetType() != typeof(HeadArmourItem))
                {
                    mCurrentItem = new HeadArmourItem();
                }
                break;

            case Enums.eArmourLocation.Body:
                BodySlots = EditorGUILayout.IntField("# Slots: ", Mathf.Clamp(BodySlots, 0, 5));
                if (mCurrentItem.GetType() != typeof(BodyArmourItem))
                {
                    mCurrentItem = new BodyArmourItem();
                }
                break;

            case Enums.eArmourLocation.Arm:
                if (mCurrentItem.GetType() != typeof(ArmArmourItem))
                {
                    mCurrentItem = new ArmArmourItem();
                }
                break;
        }
    }

    private void ShowWeapons()
    {
        ShowEquippables();

        GUILayout.Label("Weapon", EditorStyles.boldLabel);
        BaseWeaponDamage = EditorGUILayout.IntField("Base Damage: ", Mathf.Clamp(BaseWeaponDamage, 0, 999));
        WeaponEffect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", WeaponEffect);
        WeaponMultiTarget = EditorGUILayout.Toggle("Multiple Targets?: ", WeaponMultiTarget);
        Range = EditorGUILayout.FloatField("Range: ", Mathf.Clamp(Range, 0, 99));

        GUILayout.Label("Type", EditorStyles.boldLabel);
        WeaponRangeType = (Enums.eWeaponType)EditorGUILayout.EnumPopup("Weapon Type: ", WeaponRangeType);
        switch (WeaponRangeType)
        {
            case Enums.eWeaponType.Melee:
                MeleeTwoHanded = EditorGUILayout.Toggle("Two Handed?: ", MeleeTwoHanded);
                if (mCurrentItem.GetType() != typeof(MeleeWeaponItem))
                {
                    mCurrentItem = new MeleeWeaponItem();
                }
                break;

            case Enums.eWeaponType.Ranged:
                if (mCurrentItem.GetType() != typeof(RangedWeaponItem))
                {
                    mCurrentItem = new RangedWeaponItem();
                }
                break;

            case Enums.eWeaponType.Magic:
                MagicFocusType = (Enums.eMagicFocusType)EditorGUILayout.EnumPopup("Focus: ", MagicFocusType);
                MagicType = (Enums.eMagicType)EditorGUILayout.EnumPopup("Type: ", MagicType);
                MagicRadius = EditorGUILayout.FloatField("Radius: ", Mathf.Clamp(MagicRadius, 0, 99));
                MagicEffect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", MagicEffect);
                MagicCost = EditorGUILayout.IntField("Cost: ", Mathf.Clamp(MagicCost, 0, 999));
                if (mCurrentItem.GetType() != typeof(MagicWeaponItem))
                {
                    mCurrentItem = new MagicWeaponItem();
                }
                break;
        }
    }
}
