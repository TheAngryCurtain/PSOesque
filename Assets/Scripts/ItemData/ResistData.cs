using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResistData", menuName = "Resist Item Data")]
public class ResistData : StatBoostData
{
    public Enums.eStatusEffect m_Resistance;
}
