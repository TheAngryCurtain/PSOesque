using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : Item, IUsable
{
    public void Use()
    {
        RecoveryData rData = (RecoveryData)m_ItemData;
        if (rData != null)
        {
            // valid
            return;
        }

        StatusEffectData seData = (StatusEffectData)m_ItemData;
        if (seData != null)
        {
            // valid
            return;
        }

        StatUpgradeData suData = (StatUpgradeData)m_ItemData;
        if (suData != null)
        {
            // valid
            return;
        }

        WeaponUpgradeData wuData = (WeaponUpgradeData)m_ItemData;
        if (wuData != null)
        {
            //valid
            return;
        }

        Debug.LogError("Consumable Item didn't have a valid ItemData sub-class on it");
    }
}
