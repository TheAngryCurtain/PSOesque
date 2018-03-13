using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemEditor : EditorWindow
{
    private static int itemID = 0;

    private string newItemName = "New Item";
    private Texture2D newItemIcon;
    private string newItemDesc = "New Description";
    private Enums.eItemType newItemType;
    private int newItemValue = 0;
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
    private List<Enums.eClassType> UsableClasses = new List<Enums.eClassType>((int)Enums.eClassType.All);
    private List<Enums.eRaceType> UsableRaces = new List<Enums.eRaceType>((int)Enums.eRaceType.All);

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

    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Tools/Item Editor")]
    static void Init()
    {
        ItemEditor window = (ItemEditor)EditorWindow.GetWindow(typeof(ItemEditor));
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 400f, 600f);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Create Item", EditorStyles.boldLabel);
        newItemIcon = (Texture2D)EditorGUILayout.ObjectField("Icon", newItemIcon, typeof(Texture2D), false);
        newItemName = EditorGUILayout.TextField("Name: ", newItemName);

        EditorGUILayout.PrefixLabel("Description");
        newItemDesc = EditorGUILayout.TextArea(newItemDesc, GUILayout.MinHeight(60f));
        newItemValue = EditorGUILayout.IntField("Worth: ", newItemValue);
        newItemRarity = (Enums.eRarity)EditorGUILayout.EnumPopup("Rarity: ", newItemRarity);

        GUILayout.Label("Item Type", EditorStyles.boldLabel);
        newItemType = (Enums.eItemType)EditorGUILayout.EnumPopup("Type: ", newItemType);

        switch (newItemType)
        {
            case Enums.eItemType.Consumable:
                {
                    ShowConsumables();
                }
                break;

            #region Stat Boost
            case Enums.eItemType.StatBoost:
                {
                    ShowStatBoosts();
                }
                break;
            #endregion

            #region Armour
            case Enums.eItemType.Armour:
                {
                    ShowArmours();
                }
                break;
            #endregion

            #region Weapon
            case Enums.eItemType.Weapon:
                {
                    ShowWeapons();
                }
                break;
            #endregion

            case Enums.eItemType.Money:
            default:
                break;
        }

        if (GUILayout.Button("Create Item"))
        {
            Debug.LogFormat("TODO");
            // TODO
            // take the lowest sub-type to create an instance of that class
            
            // reset all variables!
        }

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();
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
                break;

            case Enums.eConsumableType.StatUpgrade:
                    newStatType = (Enums.eStatType)EditorGUILayout.EnumPopup("Boost Stat: ", newStatType);
                    statBoostAmount = EditorGUILayout.IntField("Amount: ", statBoostAmount);
                break;

            case Enums.eConsumableType.StatusEffect:
                    newEffectType = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", newEffectType);
                break;

            case Enums.eConsumableType.WeaponUpgrade:
                    weaponUpgradeAmount = EditorGUILayout.IntField("Amount: ", weaponUpgradeAmount);
                break;
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
                break;

            case Enums.eStatBoostType.LongTerm:
                LongTermEffect = (Enums.eLongTermEffectType)EditorGUILayout.EnumPopup("Effect: ", LongTermEffect);
                LongTermDelay = EditorGUILayout.FloatField("Interval: ", LongTermDelay);
                LongTermAmount = EditorGUILayout.IntField("Amount: ", LongTermAmount);
                break;

            case Enums.eStatBoostType.Resist:
                ResistEffect = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", ResistEffect);
                ResistPercent = EditorGUILayout.FloatField("Percent: ", ResistPercent);
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
                break;

            case Enums.eArmourLocation.Body:
                BodySlots = EditorGUILayout.IntField("# Slots: ", BodySlots);
                break;

            case Enums.eArmourLocation.Arm:
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
                break;

            case Enums.eWeaponRangeType.Ranged:
                RangedRange = EditorGUILayout.FloatField("Range: ", RangedRange);
                break;
        }
    }
}
