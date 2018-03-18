using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("ItemContainer")]
public class ItemContainer
{
    [XmlArray("Items")] public List<InventoryItem> m_Items = new List<InventoryItem>();
}

[XmlInclude(typeof(ConsumableItem))]
[XmlInclude(typeof(RecoveryItem))]
[XmlInclude(typeof(StatUpgradeItem))]
[XmlInclude(typeof(StatusEffectItem))]
[XmlInclude(typeof(WeaponUpgradeItem))]
[XmlInclude(typeof(EquippableItem))]
[XmlInclude(typeof(ArmourItem))]
[XmlInclude(typeof(CompanionItem))]
[XmlInclude(typeof(EquippableStatBoost))]
[XmlInclude(typeof(WeaponItem))]
[XmlInclude(typeof(ArmArmourItem))]
[XmlInclude(typeof(BodyArmourItem))]
[XmlInclude(typeof(HeadArmourItem))]
[XmlInclude(typeof(StatBoostItem))]
[XmlInclude(typeof(LongTermEffectItem))]
[XmlInclude(typeof(ResistItem))]
[XmlInclude(typeof(MeleeWeaponItem))]
[XmlInclude(typeof(RangedWeaponItem))]

[System.Serializable]
public class InventoryItem
{
    [XmlAttribute("ID")]            public int ID;
    [XmlAttribute("IconName")]      public string IconName;
    [XmlAttribute("Name")]          public string Name;
    [XmlAttribute("Description")]   public string Description;
    [XmlAttribute("Worth")]         public float Value;
    [XmlIgnore]                     public int Quantity;
    [XmlAttribute("Type")]          public Enums.eItemType Type;
    [XmlAttribute("Rarity")]        public Enums.eRarity Rarity;

    // need this for xml serialization
    public InventoryItem() { }

    public InventoryItem(int id)
    {
        ID = id;
    }
}

[System.Serializable]
public class ConsumableItem : InventoryItem, IUsable
{
    // need this for xml serialization
    public ConsumableItem() : base() { }

    public ConsumableItem(int id) : base(id) { }

    public virtual void Use() { }
}

#region Consumable Sub-Types
[System.Serializable]
public class RecoveryItem : ConsumableItem
{
    [XmlAttribute("ConsumableType")]    public Enums.eConsumableStatType ConsumableType;
    [XmlAttribute("Amount")]            public int Amount;

    // need this for xml serialization
    public RecoveryItem() : base() { }

    public RecoveryItem(int id) : base(id) { }

    public override void Use() { }
}

[System.Serializable]
public class StatUpgradeItem : ConsumableItem
{
    [XmlAttribute("StatType")]  public Enums.eStatType StatType;
    [XmlAttribute("Amount")]    public int Amount;

    // need this for xml serialization
    public StatUpgradeItem() : base() { }

    public StatUpgradeItem(int id) : base(id) { }

    public override void Use() { }
}

[System.Serializable]
public class StatusEffectItem : ConsumableItem
{
    [XmlAttribute("Effect")]  public Enums.eStatusEffect Effect;

    // need this for xml serialization
    public StatusEffectItem() : base() { }

    public StatusEffectItem(int id) : base(id) { }

    public override void Use() { }
}

[System.Serializable]
public class WeaponUpgradeItem : ConsumableItem
{
    // TODO I guess this assumes it upgrades weapon Att? Probably should have an enum with upgradable options and put it here
    [XmlAttribute("Amount")]  public int Amount;

    // need this for xml serialization
    public WeaponUpgradeItem() : base() { }

    public WeaponUpgradeItem(int id) : base(id) { }

    public override void Use() { }
}
#endregion

[System.Serializable]
public class EquippableItem : InventoryItem, IEquippable
{
    [XmlIgnore]                         public bool Equipped;
    [XmlAttribute("MiniumType")]        public Enums.eMinEquipRequirementType MinEquipType;
    [XmlAttribute("MiniumLevel")]       public int MinLevelToEquip;
    [XmlAttribute("MinimumStat")]       public Enums.eStatType StatType;
    [XmlAttribute("MinimumStatValue")]  public int MinStatValueToEquip;
    [XmlArray("UsableClasses")]         public List<Enums.eClassType> UsableClasses;
    [XmlArray("UsableRaces")]           public List<Enums.eRaceType> UsableRaces;

    // need this for xml serialization
    public EquippableItem() : base() { }

    public EquippableItem(int id) : base(id)
    {
        UsableClasses = new List<Enums.eClassType>();
        UsableRaces = new List<Enums.eRaceType>();
    }

    public virtual void Equip() { }
    public virtual void Unequip() { }
}

#region Equippable Sub-Types
[System.Serializable]
public class ArmourItem : EquippableItem
{
    [XmlAttribute("ArmourLocation")]    public Enums.eArmourLocation Location;
    [XmlAttribute("DefBoost")]          public int DefBoost;

    // need this for xml serialization
    public ArmourItem() : base() { }

    public ArmourItem(int id) : base(id) { }
}

[System.Serializable]
public class CompanionItem : EquippableItem
{
    // need this for xml serialization
    public CompanionItem() : base() { }

    public CompanionItem(int id) : base(id) { }
}

[System.Serializable]
public class EquippableStatBoost : EquippableItem
{
    // need this for xml serialization
    public EquippableStatBoost() : base() { }

    public EquippableStatBoost(int id) : base(id) { }
}

[System.Serializable]
public class WeaponItem : EquippableItem
{
    [XmlAttribute("BaseDamage")]    public int BaseDamage;
    [XmlAttribute("StatusEffect")]  public Enums.eStatusEffect Effect;
    [XmlAttribute("MultiTarget")]   public bool MultiTarget;
    // TODO elemental properties?

    // need this for xml serialization
    public WeaponItem() : base() { }

    public WeaponItem(int id) : base(id) { }
}
#endregion

#region Armour Sub-Types
[System.Serializable]
public class ArmArmourItem : ArmourItem
{
    // need this for xml serialization
    public ArmArmourItem() : base() { }

    public ArmArmourItem(int id) : base(id) { }
}

[System.Serializable]
public class BodyArmourItem : ArmourItem
{
    [XmlAttribute("Slots")] public int Slots;

    // need this for xml serialization
    public BodyArmourItem() : base() { }

    public BodyArmourItem(int id) : base(id) { }
}

[System.Serializable]
public class HeadArmourItem : ArmourItem
{
    // need this for xml serialization
    public HeadArmourItem() : base() { }

    public HeadArmourItem(int id) : base(id) { }
}
#endregion

#region Stat Boost Sub-Types
[System.Serializable]
public class StatBoostItem : EquippableStatBoost
{
    [XmlAttribute("StatType")]  public Enums.eStatType Stat;
    [XmlAttribute("Amount")]    public int Amount;

    // need this for xml serialization
    public StatBoostItem() : base() { }

    public StatBoostItem(int id) : base(id) { }
}

[System.Serializable]
public class LongTermEffectItem : EquippableStatBoost
{
    [XmlAttribute("EffectType")]    public Enums.eLongTermEffectType EffectType;
    [XmlAttribute("Delay")]         public float Delay;
    [XmlAttribute("Amount")]        public int Amount;

    // need this for xml serialization
    public LongTermEffectItem() : base() { }

    public LongTermEffectItem(int id) : base(id) { }
}

[System.Serializable]
public class ResistItem : EquippableStatBoost
{
    [XmlAttribute("ResistEffect")]  public Enums.eStatusEffect Effect;
    [XmlAttribute("Percent")]       public float Percent;

    // need this for xml serialization
    public ResistItem() : base() { }

    public ResistItem(int id) : base(id) { }
}
#endregion

#region Weapon Sub-Types
[System.Serializable]
public class MeleeWeaponItem : WeaponItem
{
    [XmlAttribute("TwoHanded")] public bool TwoHanded;

    // need this for xml serialization
    public MeleeWeaponItem() : base() { }

    public MeleeWeaponItem(int id) : base(id) { }
}

[System.Serializable]
public class RangedWeaponItem : WeaponItem
{
    [XmlAttribute("Range")] public float Range;
    // string for projectile prefab path?

    // need this for xml serialization
    public RangedWeaponItem() : base() { }

    public RangedWeaponItem(int id) : base(id) { }
}


// TODO account for magical weapons (MP cost and so on)

#endregion