using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums
{
    public enum eLevelTheme { Forest, All };
    public enum eItemSource { Crate, Enemy };
    public enum eRarity { Common, Uncommon, Special, Unique, OneOfAKind };
    public enum eCrateType { Common, Rare, VeryRare };
    public enum eEnemyType { Weak, Regular, Tough, ExtraTough, Boss };
    public enum eItemType { Consumable, StatBoost, Armour, Weapon, Rare, Money };
    public enum eDifficulty { Easy, Medium, Hard, VeryHard, Hardest, All }
    public enum eEquipmentLocation { BackPrimary, BackSecondary, HandRight, WristRight, HandLeft, WristLeft };
    public enum eStatusEffect { Burn, Confusion, Curse, Paralysis, Poison, Sleep, Slow, All };
    public enum eStatType { HP, MP, Att, Def, Mgc, Acc, Evn, Spd, Lck, Count };
    public enum eTimeOfDay { Sunrise, Sunset };
    public enum eClassType { Melee, Ranged, Magic, All };
    public enum eRaceType { Human };
    public enum eGender { Male, Female };
}
