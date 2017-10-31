using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLongTermData", menuName = "Long Term Effect Item Data")]
public class LongTermEffectData : StatBoostData
{
    public int m_HealthRegenAmount;
    public int m_MagicRegenAmount;

    public float m_HealthRegenTime;
    public float m_MagicRegenTime;
}
