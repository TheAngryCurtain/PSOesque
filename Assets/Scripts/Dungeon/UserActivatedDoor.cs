using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserActivatedDoor : RoomObject
{
    private enum eDoorState { Closed, Opening, Open };

    [SerializeField] private Transform[] m_DoorParts;
    [SerializeField] private float m_DoorOpenSpeed = 5f;

    private eDoorState m_State;

    protected override void Start()
    {
        base.Start();

        m_State = eDoorState.Closed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (m_State == eDoorState.Closed)
            {
                StartCoroutine(MoveDoorParts(true));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (m_State == eDoorState.Open)
            {
                StartCoroutine(MoveDoorParts(false));
            }
        }
    }

    private IEnumerator MoveDoorParts(bool open)
    {
        float targetYScale = (open ? 0f : 1f);
        float yScaleDirection = (open ? -m_DoorOpenSpeed : m_DoorOpenSpeed);
        float currentYScale = m_DoorParts[0].localScale.y;
        while (currentYScale > 0f || currentYScale < 1f)
        {
            Vector3 newScale = Vector3.one;
            newScale.y += yScaleDirection * Time.deltaTime;

            m_DoorParts[0].localScale = newScale;
            currentYScale = m_DoorParts[0].localScale.y;

            yield return null;
        }
    }
}
