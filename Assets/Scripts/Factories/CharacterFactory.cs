using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    [SerializeField] private GameObject m_CameraPrefab; // temp
    [SerializeField] private GameObject m_CanvasPrefab; // temp
    [SerializeField] private GameObject m_PlayerPrefab;
    [SerializeField] private GameObject m_CompanionPrefab; // temp

    [Header("Misc.")]
    [SerializeField] private GameObject m_HitParticleObj;

    private ParticleSystem m_HitEffect;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.DungeonBuiltEvent>(OnDungeonBuilt);
        VSEventManager.Instance.AddListener<GameEvents.AttackLandedEvent>(OnAttackLanded);

        GameObject particleObj = (GameObject)Instantiate(m_HitParticleObj, null);
        m_HitEffect = particleObj.GetComponentInChildren<ParticleSystem>();
    }

    // private void OnCompanionEquipped()

    private void OnDungeonBuilt(GameEvents.DungeonBuiltEvent e)
    {
        //Instantiate(m_CameraPrefab, null);
        //CameraManager.Instance.ToggleCameraSettings(true);

        // TODO
        // this should get moved to some other event when the item is equipped, so this is spawned
        Instantiate(m_CompanionPrefab, null);

        if (LobbyManager.Instance.OfflineGame)
        {
            int connectedPlayerCount = LobbyManager.Instance.ConnectedPlayerCount;
            for (int i = 0; i < connectedPlayerCount; i++)
            {
                GameObject playerObj = (GameObject)Instantiate(m_PlayerPrefab, e.StartPosition, e.StartRotation);
                GameObject playerCam = (GameObject)Instantiate(m_CameraPrefab, CameraManager.Instance.MainCamera.transform);
                GameObject hudCanvasObj = (GameObject)Instantiate(m_CanvasPrefab, null);

                int playerSaveSlot = LobbyManager.Instance.GetLobbyDataForPlayer(i).m_SaveSlot;

                // set each player camera to show the hud
                Camera camera = playerCam.GetComponent<Camera>();

                // hacky hack
                // Unity doesn't support multiple AudioListeners, which is a problem for splitscreen! You downloaded a unity package has been downloaded for it, but for now, disable on player cameras
                AudioListener listener = camera.GetComponent<AudioListener>();
                listener.enabled = false;

                Canvas playerHudCanvas = hudCanvasObj.GetComponent<Canvas>();
                if (playerHudCanvas != null)
                {
                    playerHudCanvas.worldCamera = camera;
                    playerHudCanvas.planeDistance = 1;
                }

                // there is a better way to do this (put a script on the canvas, get it, and it stores this info), but this is faster and it's late
                Transform hudObj = hudCanvasObj.transform.GetChild(0).GetChild(i);
                UIPlayerHUD currentPlayerHUD = hudObj.GetComponent<UIPlayerHUD>();
                if (currentPlayerHUD != null)
                {
                    CharacterProgress currentProgress = CharacterManager.Instance.GetProgressForCharacterInSlot(playerSaveSlot);
                    CharacterStats currentStats = currentProgress.m_Stats;

                    currentPlayerHUD.SetPlayerName(currentStats.PlayerName);
                    currentPlayerHUD.SetPlayerLevel(currentStats.Level);

                    currentStats.OnLevelChanged += currentPlayerHUD.SetPlayerLevel;
                    currentStats.OnEXPAdded += currentPlayerHUD.SetXP;
                    currentStats.OnHPModified += currentPlayerHUD.SetHP;
                    currentStats.OnMPModified += currentPlayerHUD.SetMP;

                    // trigger a restore to fire the events
                    currentStats.Restore(Enums.eStatType.HP);
                    currentStats.Restore(Enums.eStatType.MP);
                    currentStats.AddEXP(0);
                }

                // init camera controller for each player
                CameraController camController = playerCam.GetComponent<CameraController>();
                if (camController != null)
                {
                    camController.SetPlayerInfo(i, playerObj.transform);
                    if (connectedPlayerCount > 1)
                    {
                        if (camera != null)
                        {
                            PlayerCameraData camData = CameraManager.Instance.GetCameraDataForPlayer(i, connectedPlayerCount - 1);
                            camera.rect = new Rect(camData.X, camData.Y, camData.W, camData.H);
                        }
                    }
                }

                Player p = playerObj.GetComponent<Player>();
                if (p != null)
                {
                    p.Init(i, playerSaveSlot);
                    p.AssignCamera(playerCam.transform);
                }
            }
        }
    }

    private void OnAttackLanded(GameEvents.AttackLandedEvent e)
    {
        m_HitEffect.transform.parent.gameObject.transform.position = e.HitLocation; // this is also truely horrible
        m_HitEffect.Play();
    }
}
