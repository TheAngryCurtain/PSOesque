using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoostData", menuName = "Boost Item Data")]
[System.Serializable]
public class BoostData : StatBoostData
{
    public Enums.eStatType m_Stat;
    public int m_BoostAmount;
}
