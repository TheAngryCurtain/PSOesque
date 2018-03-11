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
[System.Serializable]
public class InventoryItem
{
    [XmlAttribute("ID")]            public int ID;
    [XmlAttribute("Icon")]          public string IconName;
    [XmlAttribute("Name")]          public string Name;
    [XmlAttribute("Description")]   public string Description;
    [XmlAttribute("Value")]         public float Value;
    [XmlIgnore]                     public int Quantity;
    [XmlAttribute("Type")]          public Enums.eItemType Type;

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

    public override void Use() { }
}

[System.Serializable]
public class StatUpgradeItem : ConsumableItem
{
    [XmlAttribute("StatType")]  public Enums.eStatType StatType;
    [XmlAttribute("Amount")]    public int Amount;

    // need this for xml serialization
    public StatUpgradeItem() : base() { }

    public override void Use() { }
}

[System.Serializable]
public class StatusEffectItem : ConsumableItem
{
    [XmlAttribute("Effect")]  public Enums.eStatusEffect Effect;

    // need this for xml serialization
    public StatusEffectItem() : base() { }

    public override void Use() { }
}

[System.Serializable]
public class WeaponUpgradeItem : ConsumableItem
{
    // TODO I guess this assumes it upgrades weapon Att? Probably should have an enum with upgradable options and put it here
    [XmlAttribute("Amount")]  public int Amount;

    // need this for xml serialization
    public WeaponUpgradeItem() : base() { }

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

    // need this for xml serialization
    public EquippableItem() : base() { }

    public virtual void Equip() { }
    public virtual void Unequip() { }
}