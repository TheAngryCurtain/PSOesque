using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera m_Camera;

    public void ToggleCameraSettings(bool isInGameplay)
    {
        CameraController controller = m_Camera.GetComponent<CameraController>();
        controller.enabled = isInGameplay;

        PostProcessingBehaviour behaviour = m_Camera.GetComponent<PostProcessingBehaviour>();
        behaviour.enabled = isInGameplay;
    }
}
