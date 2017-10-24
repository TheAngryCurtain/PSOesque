using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] m_FactoryPrefabs;

    // TODO
    // build some game save class that stores a world seed, and then spawns dungeon seeds from that
    private int dungeonSeed = 0;

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
        VSEventManager.Instance.TriggerEvent(new GameEvents.RequestDungeonEvent(dungeonSeed, eLevelTheme.Forest)); // TEST for now, until other prefabs are finished
    }
}
