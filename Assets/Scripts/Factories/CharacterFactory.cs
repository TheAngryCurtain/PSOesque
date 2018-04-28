using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    [SerializeField] private GameObject m_CameraPrefab; // temp
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

                CameraController camController = playerCam.GetComponent<CameraController>();
                if (camController != null)
                {
                    camController.SetPlayerInfo(i, playerObj.transform);
                    if (connectedPlayerCount > 1)
                    {
                        Camera camera = playerCam.GetComponent<Camera>();
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
                    p.Init(i, LobbyManager.Instance.GetLobbyDataForPlayer(i).m_SaveSlot);
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
