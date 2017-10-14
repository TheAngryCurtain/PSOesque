using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    [SerializeField] private GameObject m_CameraPrefab;
    [SerializeField] private GameObject m_PlayerPrefab;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.DungeonBuiltEvent>(OnDungeonBuilt);
    }

    private void OnDungeonBuilt(GameEvents.DungeonBuiltEvent e)
    {
        Instantiate(m_CameraPrefab, null);
        Instantiate(m_PlayerPrefab, e.StartLocation.position, e.StartLocation.rotation);
    }
}
