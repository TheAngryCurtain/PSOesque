using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO
// this will need to also copy over all possible values from different item types, like armour, weapons, companions, etc
[System.Serializable]
public class InventoryItem
{
    public int ID;
    public string Name;
    [TextArea(3, 5)] public string Description;

    public float Value;
    public int Quantity;

    public Enums.eItemType Type;

    public InventoryItem(int id)
    {
        ID = id;
    }
}

[CreateAssetMenu(fileName = "NewItemData", menuName = "Item Data")]
[System.Serializable]
public class ItemData : ScriptableObject
{
    // TODO
    // breakable? repairable? ?? maybe the rarest items are unbreakable but the smaller ones aren't?

    // for resource loading
    // icon name string?
    // model name string?
    // data name string?

    [SerializeField] private int m_ItemID = -1;
    public int ItemID { get { return m_ItemID; } set { m_ItemID = value; } }

    // Find related
    public List<Enums.eLevelTheme> m_Themes;
    public List<Enums.eDifficulty> m_Difficulties;

    public Enums.eItemType m_ItemType;
    public Enums.eRarity m_ItemRarity;

    // Details
    public string m_ItemName;

    [TextArea(3, 5)]
    public string m_ItemDescription;
    public float m_ItemValue;
}

// --- CONSUMABLES
// Recovery         > fields for amount of health, magic to recover, bool for if it can revive?
// Status Effect    > enum for status effect, will clear players effect if they match
// Stat Upgrade     > enum for stat type, field for amount to boost by
// Weapon Upgrade   > field for amount to boost attack by

// --- ARMOUR (min level/Defense stat to equip?)
// Body             > Mail and Armour, affects defense, possibly evasion?
// Arm              > Guards and Shields, additional defense? (no resist? otherwise it makes stat boost > resist seem silly?)

// --- STAT BOOSTS
// Boost            > field for stat type enum that it boosts, and amount to boost by (only while equipped)
// Resist           > field for status effect enum that it resists, can't be affected by it (only while equipped)
// Longterm Effect  > fields for amount of health, magic, (special??) to regenerate, and time step for how often it is regained (only while equipped)

// --- WEAPONS (min level/stat to equip?)
// Melee            > no range; collider only. damage, optional status effect
// Ranged           > range, damage, optional status effect
// Magic            > range, damage, status effect