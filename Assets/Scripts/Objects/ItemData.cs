using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO more this somewhere else that makes more sense
public enum eLevelTheme { Forest };
public enum eItemSource { Crate, Enemy };
public enum eRarity { Common, Uncommon, Special, Unique, OneOfAKind };

[CreateAssetMenu(fileName = "NewItemData", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    // TODO
    // should be able to add tags (or something) to this to say which themes it should show up in
    // as well as which enemy types (Weak, Regular, Tough, Very Tough, Boss) it should drop from
    // maybe also support species of enemies as well? I guess that would be a combination of theme + type though.
    // that way, we could query and item list for these particular tags and get subsets.

    // Find related
    public List<eLevelTheme> m_Themes;
    public List<eDifficulty> m_Difficulties;
    public List<eItemSource> m_Sources;
    public List<eCrateType> m_CrateTypes;
    public List<eEnemyType> m_EnemyTypes;

    public eItemType m_ItemType;
    public eRarity m_ItemRarity;

    public int[] m_ProababilityByDifficulty = new int[5];

    // Details
    public string m_ItemName;
    public string m_ItemDescription;
    public float m_ItemValue;
}
