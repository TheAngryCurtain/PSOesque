using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] m_FactoryPrefabs;

    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_FactoryPrefabs.Length; i++)
        {
            Instantiate(m_FactoryPrefabs[i], null);
        }
    }

    private void Start()
    {
        VSEventManager.Instance.TriggerEvent(new GameEvents.RequestDungeonEvent());
    }
}
