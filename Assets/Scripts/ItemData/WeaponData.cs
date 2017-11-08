using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData : EquippableData
{
    public int m_BaseDamage;
    public Enums.eStatusEffect m_DamageType;
    public float m_AttackRange;
}
