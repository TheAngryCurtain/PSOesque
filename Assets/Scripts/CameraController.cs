using UnityEngine;
using Rewired;

public class CameraController : MonoBehaviour
{
    public enum eCameraMode { Free, Locked, Static };

	[SerializeField] private Transform m_Target;
    [SerializeField] private Transform m_LockObject;

    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private float m_Pitch = 2f;
    [SerializeField] private float m_Zoom = 5f;
    [SerializeField] private float m_YawSpeed = 100f;
    [SerializeField] private float m_LockedViewDistance = 5f;
    [SerializeField] private float m_MinCameraY = 0.3f;
    [SerializeField] private float m_CurrentYaw = 0f;

    private eCameraMode m_CameraMode = eCameraMode.Static;

    private void Awake()
    {
        InputManager.Instance.AddInputEventDelegate(OnInputUpdate, Rewired.UpdateLoopType.Update);
        VSEventManager.Instance.AddListener<GameEvents.PlayerSpawnedEvent>(OnPlayerSpawned);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameEvents.PlayerSpawnedEvent>(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(GameEvents.PlayerSpawnedEvent e)
    {
        m_Target = e.PlayerObj.transform;
    }

    public void SetLockTarget(Transform lockObj)
    {
        m_LockObject = lockObj;
    }

    private void OnInputUpdate(InputActionEventData data)
    {
        if (m_CameraMode == eCameraMode.Free)
        {
            float horizontal = 0f;

            switch (data.actionId)
            {
                case RewiredConsts.Action.Move_Horizontal:
                    horizontal = data.GetAxis();
                    break;
            }

            m_CurrentYaw += horizontal * m_YawSpeed * Time.deltaTime;
        }
    }

    public void ChangeMode(eCameraMode mode)
    {
        m_CameraMode = mode;
    }

    private void LateUpdate()
    {
        if (m_Target != null)
        {
            if (m_CameraMode == eCameraMode.Free || m_CameraMode == eCameraMode.Static)
            {
                transform.position = m_Target.position - m_Offset * m_Zoom;
                transform.LookAt(m_Target.position + Vector3.up * m_Pitch);

                transform.RotateAround(m_Target.position, Vector3.up, m_CurrentYaw);
            }
            else if (m_CameraMode == eCameraMode.Locked)
            {

                Vector3 lockToTarget = (m_LockObject.position - m_Target.position).normalized;

                // don't adjust the camera's height, just rotation
                lockToTarget.y = 0f;

                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lockToTarget), m_LockRotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(lockToTarget);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                //transform.position = Vector3.Slerp(transform.position, (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance, Time.deltaTime);

                Vector3 cameraPos = (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance;
                if (cameraPos.y < m_MinCameraY)
                {
                    cameraPos.y = m_MinCameraY;
                }
                transform.position = cameraPos;
            }
        }
    }
}
