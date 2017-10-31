using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO more this somewhere else that makes more sense
[CreateAssetMenu(fileName = "NewItemData", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    // TODO
    // maybe also support species of enemies as well? I guess that would be a combination of theme + type though.

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

public class EquippableData : ItemData
{
    public int m_MinLevelToEquip;

    protected bool m_Equipped = false;
    public bool Equipped { get { return m_Equipped; } }
}

public class ConsumableData : ItemData, IUsable
{
    public virtual void Use() { }
}

public class StatBoostData : EquippableData, IEquippable
{
    public virtual void Equip() { }
    public virtual void Unequip() { }
}

public class ArmourData : EquippableData { }
public class WeaponData : EquippableData
{
    public int m_BaseDamage;
    public Enums.eStatusEffect m_DamageType;
    public float m_AttackRange;
}

#region Armour
public class BodyData : ArmourData
{

}

public class ArmData : ArmourData
{

}

#endregion

#region Weapons
public class MeleeData : WeaponData
{

}

public class RangedData : WeaponData
{

}

public class MagicData : WeaponData
{

}

#endregion

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