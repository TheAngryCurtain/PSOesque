using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : Singleton<ObjectFactory>
{
    public enum eObject { Chest, SwitchDoor, Switch, Spawner, EnemyDoor };

    [SerializeField] private GameObject[] m_ObjectPrefabs;

    public GameObject GetObjectPrefab(eObject objType)
    {
        return m_ObjectPrefabs[(int)objType];
    }
}
