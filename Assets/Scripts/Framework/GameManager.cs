using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] m_FactoryPrefabs;

    // TODO
    // build some game save class that stores a world seed, and then spawns dungeon seeds from that
    private int dungeonSeed = 0;

    private void Start()
    {
        VSEventManager.Instance.AddListener<UIEvents.SceneLoadedEvent>(OnSceneLoaded);

        // start the flow
        SceneLoader.Instance.RequestSceneLoad(Enums.eScene.Main);
        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Company);
    }

    private void OnSceneLoaded(UIEvents.SceneLoadedEvent e)
    {
        switch (e.LoadedScene)
        {
            case Enums.eScene.Hub:
                // only spawn the character factory
                Instantiate(m_FactoryPrefabs[0], null);
                break;

            case Enums.eScene.Game:
                // spawn everything else
                for (int i = 1; i < m_FactoryPrefabs.Length; i++)
                {
                    Instantiate(m_FactoryPrefabs[i], null);
                }

                VSEventManager.Instance.TriggerEvent(new GameEvents.RequestDungeonEvent(dungeonSeed, Enums.eLevelTheme.Forest));
                break;
        }
    }
}
