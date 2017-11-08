using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponUpgradeData", menuName = "Weapon Upgrade Item Data")]
[System.Serializable]
public class WeaponUpgradeData : ConsumableData
{
    public int m_UpgradeAmount;
}
