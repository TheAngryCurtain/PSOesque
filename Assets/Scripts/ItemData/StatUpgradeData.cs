using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatUpgradeData", menuName = "Stat Upgrade Item Data")]
public class StatUpgradeData : ConsumableData
{
    public Enums.eStatType m_Stat;
    public int m_BoostAmount;
}
