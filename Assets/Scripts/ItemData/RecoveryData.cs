using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecoveryData", menuName = "Recovery Item Data")]
[System.Serializable]
public class RecoveryData : ConsumableData
{
    public int m_HealthAmount;
    public int m_MagicAmount;
    public bool m_CanRevive = false;
}
