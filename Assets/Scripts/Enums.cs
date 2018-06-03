using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// probably should refactor this to be in the Gameplay namespace. Rename the file to Gameplay Enums
namespace Enums
{
    // Gameplay
    public enum eLevelTheme { Forest, All };
    public enum eItemSource { Crate, Enemy };
    public enum eRarity { Common, Uncommon, Special, Unique, OneOfAKind };
    public enum eCrateType { Common, Rare, VeryRare };
    public enum eEnemyType { Weak, Regular, Tough, ExtraTough, Boss };
    public enum eItemType { Consumable, Companion, StatBoost, Armour, Weapon, Money };
    public enum eConsumableType { Recovery, StatUpgrade, StatusEffect, WeaponUpgrade, CharacterSupport };
    public enum eStatBoostType { Basic, LongTerm, Resist };
    public enum eMinEquipRequirementType { None = -1, Stat, Level };
    public enum eArmourLocation { Head, Body, Arm };
    public enum eLongTermEffectType { HP, MP, Special };
    public enum eWeaponType { Melee, Ranged, Magic };
    public enum eMagicFocusType { Directional, AOE, Room, Self };
    public enum eMagicType { Attack, Support };
    public enum eSpellType { Fire, Water, Earth, Ice, Electricity, Wind, Ghost, Undead, Sun, Moon }; // just made these up on the fly
    public enum eCharacterSupportType { Revive, Teleport, Resource, Scroll };
    public enum eDifficulty { Easy, Medium, Hard, VeryHard, Hardest, All };
    public enum eEquipmentLocation { BackPrimary, BackSecondary, HandRight, ArmRight, HandLeft, ArmLeft };
    public enum eStatusEffect { None = -1, Burn, Confusion, Curse, Paralysis, Poison, Sleep, Slow, All };
    public enum eStatType { HP, MP, Att, Def, Mgc, Acc, Evn, Spd, Lck, Count };
    //public enum eAttributeType { Stealth, BodyTemp };
    public enum eTimeOfDay { Sunrise, Sunset };
    public enum eClassType { Melee, Ranged, Magic, All };
    public enum eRaceType { Human, All };
    public enum eGender { Male, Female };
    public enum eConsumableStatType { HP, MP };
    public enum eHubRoom { None = -1, Shop, Teleporter, QuestKiosk, Home, Bank, Hospital, Trophy, Contractor };

    // Scene Loading
    public enum eScene { Boot = 0, Main, Lobby, CharacterCreator, Hub, Game };
}
