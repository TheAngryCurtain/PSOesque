using UnityEngine;
using System.Collections;
using Rewired;

public class CameraController : MonoBehaviour
{
    public enum eCameraMode { Free, Locked, Static };

	[SerializeField] private Transform m_Target;
    [SerializeField] private Transform m_LockObject;
    [SerializeField] private Transform m_Transform;

    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private float m_Pitch = 2f;
    [SerializeField] private float m_Zoom = 5f;
    [SerializeField] private float m_YawSpeed = 100f;
    [SerializeField] private float m_LockedViewDistance = 5f;
    [SerializeField] private float m_MinCameraY = 0.3f;
    [SerializeField] private float m_CurrentYaw = 0f;

    private eCameraMode m_CameraMode = eCameraMode.Free;

    private Transform m_PlayerTransform;
    private float m_LerpSpeed = 1f;

    private void Awake()
    {
        InputManager.Instance.AddInputEventDelegate(OnInputUpdate, Rewired.UpdateLoopType.Update);
        VSEventManager.Instance.AddListener<GameEvents.PlayerSpawnedEvent>(OnPlayerSpawned);

        VSEventManager.Instance.AddListener<GameEvents.DoorOpenedEvent>(OnDoorOpened);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameEvents.PlayerSpawnedEvent>(OnPlayerSpawned);
        VSEventManager.Instance.RemoveListener<GameEvents.DoorOpenedEvent>(OnDoorOpened);
    }

    private void OnDoorOpened(GameEvents.DoorOpenedEvent e)
    {
        // adjust camera settings for quick movement
        m_CameraMode = eCameraMode.Static;
        m_LerpSpeed = Time.deltaTime * 0.25f;

        m_Target = e.DoorTransform;
        StartCoroutine(SetTargetAfterDelay(m_PlayerTransform, 3f));
    }

    private IEnumerator SetTargetAfterDelay(Transform target, float delay)
    {
        yield return new WaitForSeconds(delay);

        // change camera back
        m_CameraMode = eCameraMode.Free;
        m_LerpSpeed = 1f;
        m_Target = target;
    }

    private void OnPlayerSpawned(GameEvents.PlayerSpawnedEvent e)
    {
        m_PlayerTransform = e.PlayerObj.transform;
        m_Target = m_PlayerTransform;
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
            float vertical = 0f;

            switch (data.actionId)
            {
                case RewiredConsts.Action.Camera_Horizontal:
                    horizontal = data.GetAxis();
                    break;

                case RewiredConsts.Action.Camera_Vertical:
                    vertical = data.GetAxis();
                    break;
            }

            m_CurrentYaw += horizontal * m_YawSpeed * Time.deltaTime;

            m_Offset.y += vertical * -1f * Time.deltaTime;
            m_Offset.y = Mathf.Clamp(m_Offset.y, -3f, -0.75f);
        }
        else
        {
            m_CurrentYaw = 0f;
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
                Vector3 newPosition = m_Target.position - m_Offset * m_Zoom;
                m_Transform.position = Vector3.Lerp(m_Transform.position, newPosition, m_LerpSpeed);
                m_Transform.LookAt(m_Target.position + Vector3.up * m_Pitch);
                m_Transform.RotateAround(m_Target.position, Vector3.up, m_CurrentYaw);
            }
            else if (m_CameraMode == eCameraMode.Locked)
            {

                Vector3 lockToTarget = (m_LockObject.position - m_Target.position).normalized;

                // don't adjust the camera's height, just rotation
                lockToTarget.y = 0f;

                //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(lockToTarget), m_LockRotationSpeed * Time.deltaTime);
                m_Transform.rotation = Quaternion.LookRotation(lockToTarget);
                m_Transform.eulerAngles = new Vector3(0, m_Transform.eulerAngles.y, 0);
                //m_Transform.position = Vector3.Slerp(m_Transform.position, (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance, Time.deltaTime);

                Vector3 cameraPos = (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance;
                if (cameraPos.y < m_MinCameraY)
                {
                    cameraPos.y = m_MinCameraY;
                }
                m_Transform.position = cameraPos;
            }
        }
    }
}
