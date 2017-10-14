using UnityEngine;

public class Door : MonoBehaviour, ISwitchable
{
    [SerializeField] private GameObject m_DoorObj;

    public void OnSwitchPressed()
    {
        m_DoorObj.transform.localPosition -= Vector3.up;
    }
}
