using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserActivatedDoor : RoomObject
{
    private enum eDoorOpenState { Closed, Open };

    [SerializeField] private Transform[] m_DoorParts;
    [SerializeField] private GameObject m_LockObj;

    private float m_DoorOpenSpeed = 3f;

    private eDoorOpenState m_State;
    private bool m_IsLocked = true;
    private float m_StartYScale;

    protected override void Start()
    {
        m_State = eDoorOpenState.Closed;
        m_StartYScale = m_DoorParts[0].localScale.y;
    }

    public void SetLocked(bool locked)
    {
        m_IsLocked = locked;
        m_LockObj.SetActive(m_IsLocked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!m_IsLocked && m_State == eDoorOpenState.Closed)
            {
                StartCoroutine(MoveDoorParts(true));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (m_State == eDoorOpenState.Open)
            {
                StartCoroutine(MoveDoorParts(false));
            }
        }
    }

    private IEnumerator MoveDoorParts(bool open)
    {
        float targetYScale = (open ? 0f : 1f);
        float yScaleDirection = (open ? -m_DoorOpenSpeed : m_DoorOpenSpeed);
        Vector3 newScale = m_DoorParts[0].localScale;
        float currentYScale = m_DoorParts[0].localScale.y;

        while (currentYScale > 0f && currentYScale < m_StartYScale + 0.01f)
        {
            newScale.y += yScaleDirection * Time.deltaTime;
            if (newScale.y < 0f)
            {
                newScale.y = 0f;
            }

            m_DoorParts[0].localScale = newScale;
            m_DoorParts[1].localScale = newScale;

            currentYScale = m_DoorParts[0].localScale.y;
            currentYScale = m_DoorParts[1].localScale.y;

            yield return null;
        }

        // need to reset the scale to not be negative
        newScale.y = (open ? 0.01f : m_StartYScale - 0.01f);
        m_DoorParts[0].localScale = newScale;
        m_DoorParts[1].localScale = newScale;

        // set state
        m_State = (open ? eDoorOpenState.Open : eDoorOpenState.Closed);
    }
}
