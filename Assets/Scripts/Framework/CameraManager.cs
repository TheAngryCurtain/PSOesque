using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerCameraData
{
    public float X;
    public float Y;
    public float W;
    public float H;

    public PlayerCameraData(float x, float y, float w, float h)
    {
        X = x;
        Y = y;
        W = w;
        H = h;
    }
}

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera m_Camera;
    public Camera MainCamera { get { return m_Camera; } }

    private PlayerCameraData[][] m_PlayerCameraConfigurations;

    public override void Awake()
    {
        base.Awake();

        m_PlayerCameraConfigurations = new PlayerCameraData[4][];
                                                                                    // P1                                            P2                                         P3                                            P4
        m_PlayerCameraConfigurations[0] = new PlayerCameraData[] { new PlayerCameraData(0f, 0f, 1f, 1f) };                                                                                                                                                      // 1 player
        m_PlayerCameraConfigurations[1] = new PlayerCameraData[] { new PlayerCameraData(0f, 0f, 0.5f, 1f),      new PlayerCameraData(0.5f, 0f, 0.5f, 1f) };                                                                                                     // 2 players
        m_PlayerCameraConfigurations[2] = new PlayerCameraData[] { new PlayerCameraData(0f, 0.5f, 0.5f, 0.5f),  new PlayerCameraData(0.5f, 0.5f, 0.5f, 0.5f), new PlayerCameraData(0.25f, 0f, 0.5f, 0.5f) };                                                    // 3 players
        m_PlayerCameraConfigurations[3] = new PlayerCameraData[] { new PlayerCameraData(0f, 0.5f, 0.5f, 0.5f),  new PlayerCameraData(0.5f, 0.5f, 0.5f, 0.5f), new PlayerCameraData(0f, 0f, 0.5f, 0.5f),         new PlayerCameraData(0.5f, 0f, 0.5f, 0.5f) };   // 4 players
    }

    public PlayerCameraData GetCameraDataForPlayer(int playerID, int totalPlayers)
    {
        return m_PlayerCameraConfigurations[totalPlayers][playerID];
    }

    //public void ToggleCameraSettings(bool isInGameplay)
    //{
    //    CameraController controller = m_Camera.GetComponent<CameraController>();
    //    controller.enabled = isInGameplay;

    //    PostProcessingBehaviour behaviour = m_Camera.GetComponent<PostProcessingBehaviour>();
    //    behaviour.enabled = isInGameplay;
    //}
}
