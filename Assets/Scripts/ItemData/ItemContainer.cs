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
[XmlInclude(typeof(MagicWeaponItem))]

[XmlInclude(typeof(HPRecover))]
[XmlInclude(typeof(MPRecover))]
[XmlInclude(typeof(Teleport))]
[XmlInclude(typeof(Revive))]
[XmlInclude(typeof(Resource))]
[XmlInclude(typeof(Scroll))]
[XmlInclude(typeof(Spell))]

[System.Serializable]
public class InventoryItem
{
    [XmlAttribute("ID")]            public int ID;
    [XmlAttribute("IconName")]      public string IconName;
    [XmlAttribute("Name")]          public string Name;
    [XmlAttribute("Description")]   public string Description;
    [XmlAttribute("Worth")]         public float Value;
    [XmlIgnore]                     public int Quantity;
    [XmlAttribute("ItemType")]      public Enums.eItemType Type;
    [XmlAttribute("Rarity")]        public Enums.eRarity Rarity;
    [XmlArray("Themes")]            public List<Enums.eLevelTheme> Themes;
    [XmlArray("Difficulties")]      public List<Enums.eDifficulty> Difficulties;


    // need this for xml serialization
    public InventoryItem() { }

    public InventoryItem(int id)
    {
        ID = id;

        Themes = new List<Enums.eLevelTheme>();
        Difficulties = new List<Enums.eDifficulty>();
    }
}

[System.Serializable]
public class ConsumableItem : InventoryItem, IUsable
{
    [XmlIgnore] public Enums.eConsumableType ConsumableType;

    // need this for xml serialization
    public ConsumableItem() : base() { }

    public ConsumableItem(int id) : base(id) { }

    public virtual void Use(Character target) { }
}

#region Consumable Sub-Types
[System.Serializable]
public class RecoveryItem : ConsumableItem
{
    [XmlAttribute("ConsumableType")]    public Enums.eConsumableStatType ConsumableStatType;
    [XmlAttribute("Amount")]            public int Amount;

    // need this for xml serialization
    public RecoveryItem() : base() { }

    public RecoveryItem(int id) : base(id) { }

    public override void Use(Character target) { }
}

[System.Serializable]
public class StatUpgradeItem : ConsumableItem
{
    [XmlAttribute("StatType")]  public Enums.eStatType StatType;
    [XmlAttribute("Amount")]    public int Amount;

    // need this for xml serialization
    public StatUpgradeItem() : base() { }

    public StatUpgradeItem(int id) : base(id)
    {
        ConsumableType = Enums.eConsumableType.StatUpgrade;
    }

    public override void Use(Character target) { }
}

[System.Serializable]
public class StatusEffectItem : ConsumableItem
{
    [XmlAttribute("Effect")]  public Enums.eStatusEffect Effect;

    // need this for xml serialization
    public StatusEffectItem() : base() { }

    public StatusEffectItem(int id) : base(id)
    {
        ConsumableType = Enums.eConsumableType.StatusEffect;
    }

    public override void Use(Character target) { }
}

[System.Serializable]
public class WeaponUpgradeItem : ConsumableItem
{
    // only applies to Att
    [XmlAttribute("Amount")]  public int Amount;

    // need this for xml serialization
    public WeaponUpgradeItem() : base() { }

    public WeaponUpgradeItem(int id) : base(id)
    {
        ConsumableType = Enums.eConsumableType.WeaponUpgrade;
    }

    public override void Use(Character target) { }
}

[System.Serializable]
public class CharacterSupportItem : ConsumableItem
{
    [XmlAttribute("SupportType")] public Enums.eCharacterSupportType SupportType;
    // bool for revive on death?
    // list of stats that resource will boost on companion?

    // need this for xml serialization
    public CharacterSupportItem() : base() { }

    public CharacterSupportItem(int id) : base(id)
    {
        ConsumableType = Enums.eConsumableType.CharacterSupport;
    }

    public override void Use(Character target) { }
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

    public ArmourItem(int id) : base(id)
    {
        Type = Enums.eItemType.Armour;
    }
}

[System.Serializable]
public class CompanionItem : EquippableItem
{
    // need this for xml serialization
    public CompanionItem() : base() { }

    public CompanionItem(int id) : base(id)
    {
        Type = Enums.eItemType.Companion;
    }
}

[System.Serializable]
public class EquippableStatBoost : EquippableItem
{
    [XmlIgnore] public Enums.eStatBoostType StatBoostType;

    // need this for xml serialization
    public EquippableStatBoost() : base() { }

    public EquippableStatBoost(int id) : base(id)
    {
        Type = Enums.eItemType.StatBoost;
    }
}

[System.Serializable]
public class WeaponItem : EquippableItem
{
    [XmlAttribute("BaseDamage")]     public int BaseDamage;
    [XmlAttribute("StatusEffect")]   public Enums.eStatusEffect Effect;
    [XmlAttribute("MultiTarget")]    public bool MultiTarget;
    [XmlAttribute("Range")]          public float Range;
    [XmlAttribute("ProjectileName")] public string ProjectileName;
    [XmlIgnore]                      protected GameObject ProjectilePrefab;
    [XmlIgnore]                      public Enums.eWeaponType WeaponType;

    // TODO elemental properties? for non-spells probably?

    // need this for xml serialization
    public WeaponItem() : base() { }

    public WeaponItem(int id) : base(id)
    {
        Type = Enums.eItemType.Weapon;
    }
}
#endregion

#region Armour Sub-Types
[System.Serializable]
public class ArmArmourItem : ArmourItem
{
    // need this for xml serialization
    public ArmArmourItem() : base() { }

    public ArmArmourItem(int id) : base(id)
    {
        Location = Enums.eArmourLocation.Arm;
    }
}

[System.Serializable]
public class BodyArmourItem : ArmourItem
{
    [XmlAttribute("Slots")] public int Slots;

    // need this for xml serialization
    public BodyArmourItem() : base() { }

    public BodyArmourItem(int id) : base(id)
    {
        Location = Enums.eArmourLocation.Body;
    }
}

[System.Serializable]
public class HeadArmourItem : ArmourItem
{
    // need this for xml serialization
    public HeadArmourItem() : base() { }

    public HeadArmourItem(int id) : base(id)
    {
        Location = Enums.eArmourLocation.Head;
    }
}
#endregion

#region Equippable Stat Boost Sub-Types
[System.Serializable]
public class StatBoostItem : EquippableStatBoost
{
    [XmlAttribute("StatType")]  public Enums.eStatType Stat;
    [XmlAttribute("Amount")]    public int Amount;

    // need this for xml serialization
    public StatBoostItem() : base() { }

    public StatBoostItem(int id) : base(id)
    {
        StatBoostType = Enums.eStatBoostType.Basic;
    }
}

[System.Serializable]
public class LongTermEffectItem : EquippableStatBoost
{
    [XmlAttribute("EffectType")]    public Enums.eLongTermEffectType EffectType;
    [XmlAttribute("Delay")]         public float Delay;
    [XmlAttribute("Amount")]        public int Amount;

    // need this for xml serialization
    public LongTermEffectItem() : base() { }

    public LongTermEffectItem(int id) : base(id)
    {
        StatBoostType = Enums.eStatBoostType.LongTerm;
    }
}

[System.Serializable]
public class ResistItem : EquippableStatBoost
{
    [XmlAttribute("ResistEffect")]  public Enums.eStatusEffect Effect;
    [XmlAttribute("Percent")]       public float Percent;

    // need this for xml serialization
    public ResistItem() : base() { }

    public ResistItem(int id) : base(id)
    {
        StatBoostType = Enums.eStatBoostType.Resist;
    }
}
#endregion

#region Weapon Sub-Types
[System.Serializable]
public class MeleeWeaponItem : WeaponItem
{
    [XmlAttribute("TwoHanded")] public bool TwoHanded;

    // need this for xml serialization
    public MeleeWeaponItem() : base() { }

    public MeleeWeaponItem(int id) : base(id)
    {
        WeaponType = Enums.eWeaponType.Melee;
    }
}

[System.Serializable]
public class RangedWeaponItem : WeaponItem
{
    // need this for xml serialization
    public RangedWeaponItem() : base() { }

    public RangedWeaponItem(int id) : base(id)
    {
        WeaponType = Enums.eWeaponType.Ranged;
    }
}

[System.Serializable]
public class MagicWeaponItem : WeaponItem
{
    [XmlAttribute("Focus Type")]    public Enums.eMagicFocusType FocusType;
    [XmlAttribute("MagicType")]     public Enums.eMagicType MagicType;
    [XmlAttribute("Radius")]        public float Radius;
    [XmlAttribute("Effect")]        public Enums.eStatusEffect MagicEffect;
    [XmlAttribute("Cost")]          public int MPCost;
    [XmlIgnore]                     protected int RoomID;
    [XmlIgnore]                     protected Vector3 TargetPosition;

    // need this for xml serialization
    public MagicWeaponItem() : base() { }

    public MagicWeaponItem(int id) : base(id)
    {
        WeaponType = Enums.eWeaponType.Magic;
    }
}

#endregion

[System.Serializable]
public class HPRecover : RecoveryItem
{
    // need this for xml serialization
    public HPRecover() : base() { }

    public HPRecover(int id) : base(id)
    {
        Type = Enums.eItemType.Consumable;
        ConsumableType = Enums.eConsumableType.Recovery;
        ConsumableStatType = Enums.eConsumableStatType.HP;
    }

    public override void Use(Character target)
    {
        base.Use(target);

        VSEventManager.Instance.TriggerEvent(new GameEvents.UpdateCharacterStatEvent(Enums.eStatType.HP, Amount));
    }
}

[System.Serializable]
public class MPRecover : RecoveryItem
{
    // need this for xml serialization
    public MPRecover() : base() { }

    public MPRecover(int id) : base(id)
    {
        Type = Enums.eItemType.Consumable;
        ConsumableType = Enums.eConsumableType.Recovery;
        ConsumableStatType = Enums.eConsumableStatType.MP;
    }

    public override void Use(Character target)
    {
        base.Use(target);

        VSEventManager.Instance.TriggerEvent(new GameEvents.UpdateCharacterStatEvent(Enums.eStatType.MP, Amount));
    }
}

[System.Serializable]
public class Teleport : CharacterSupportItem
{
    [XmlIgnore]                     private GameObject TeleportPrefab;
    [XmlAttribute("PrefabName")]    public string PrefabName;
    [XmlAttribute("LifeTime")]      public float LifeTime;
    [XmlIgnore]                     private float PlacementTime;

    // life time and placement time maybe able to be stored in teh Teleporter class instead of here

    // need this for xml serialization
    public Teleport() : base() { }

    public Teleport(int id) : base(id)
    {
        SupportType = Enums.eCharacterSupportType.Teleport;
    }

    public override void Use(Character target)
    {
        base.Use(target);

        // TODO
        // load prefab
        // instantiate prefab at character target position
        // set start time
        // will probably need to register this (teleporter script instance) with the level generator thing with a level id and position so it can be loaded back into the level in the correct location
        // perhaps a teleporter can be assigned as the "main" one that the player loads in at, and this one could be set as the one to load in at, if it exists.
    }
}

[System.Serializable]
public class Revive : CharacterSupportItem
{
    // need this for xml serialization
    public Revive() : base() { }

    public Revive(int id) : base(id)
    {
        SupportType = Enums.eCharacterSupportType.Revive;
    }

    public override void Use(Character target)
    {
        base.Use(target);

        // TODO
        // should probably only work if the target is dead...
        // send an event to revive the target player
    }
}

[System.Serializable]
public class Resource : CharacterSupportItem
{
    // TODO
    // set up a list of buffs/debuffs so that each resource fed to a companion can raise/lower stats

    // need this for xml serialization
    public Resource() : base() { }

    // a resource is used for feeding companions to bolster their stats
    public Resource(int id) : base(id)
    {
        SupportType = Enums.eCharacterSupportType.Resource;
    }

    public override void Use(Character target)
    {
        base.Use(target);
    }
}

[System.Serializable]
public class Scroll : CharacterSupportItem
{
    [XmlAttribute("TeachSpellType")] public Enums.eSpellType TeachSpellType;

    // need this for xml serialization
    public Scroll() : base() { }

    public Scroll(int id) : base(id)
    {
        SupportType = Enums.eCharacterSupportType.Scroll;
    }

    public override void Use(Character target)
    {
        base.Use(target);

        // TODO
        // look up the base level spell for the given type and add it to the player inventory for later equipping.
    }
}

[System.Serializable]
public class Spell : MagicWeaponItem
{
    [XmlAttribute("SpellLevel")]    private int SpellLevel;
    [XmlAttribute("SpellType")]     public Enums.eSpellType SpellType;

    // need this for xml serialization
    public Spell() : base() { }

    public Spell(int id) : base(id)
    {


        SpellLevel = 0;
    }
}

[System.Serializable]
public class Hood : HeadArmourItem
{
    
}

// TODO
// NOTE: Probably only need a different class for Hoods/other more specific items IF they will have special stat applications (hoods for stealth, etc)
// add individual armour items: different head, body, and arm pieces. Add them to the master list up top and remove the base classes