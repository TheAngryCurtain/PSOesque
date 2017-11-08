using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConsumableData : ItemData, IUsable
{
    public virtual void Use() { }
}
