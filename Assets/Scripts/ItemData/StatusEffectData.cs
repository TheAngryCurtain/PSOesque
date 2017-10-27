using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffectData", menuName = "Status Effect Item Data")]
public class StatusEffectData : ConsumableData
{
    public Enums.eStatusEffect m_Affect;
}
