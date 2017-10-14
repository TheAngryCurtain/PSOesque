using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Player : Character
{
    protected Vector3 m_Movement = Vector3.zero;

    private float m_Horizontal = 0f;
    private float m_Vertical = 0f;

    protected override void Awake()
    {
        base.Awake();

        InputManager.Instance.AddInputEventDelegate(OnInputUpdate, UpdateLoopType.Update);
        VSEventManager.Instance.TriggerEvent(new GameEvents.PlayerSpawnedEvent(this.gameObject));
    }

    protected override void OnDestroy()
    {
        InputManager.Instance.RemoveInputEventDelegate(OnInputUpdate);
    }

    protected virtual void OnInputUpdate(InputActionEventData data)
    {
        switch (data.actionId)
        {
            case RewiredConsts.Action.Move_Horizontal:
                m_Horizontal = data.GetAxis();
                break;

            case RewiredConsts.Action.Move_Vertical:
                m_Vertical = data.GetAxis();
                break;
        }
    }

    protected virtual void Update()
    {
        Move(m_Horizontal, m_Vertical);
    }

    protected virtual void Move(float h, float v)
    {
        m_Movement.x = h;
        m_Movement.z = v;

        if (m_Movement != Vector3.zero)
        {
            SetDestination(transform.position + m_Movement);
        }
        else
        {
            Stop();
        }
    }
}
