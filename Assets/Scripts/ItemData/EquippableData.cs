using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquippableData : ItemData, IEquippable
{
    public int m_MinLevelToEquip;
    public List<Enums.eClassType> m_UsableByClass;

    [SerializeField]
    protected bool m_Equipped = false;
    public bool Equipped { get { return m_Equipped; } }

    public virtual void Equip() { }
    public virtual void Unequip() { }
}
