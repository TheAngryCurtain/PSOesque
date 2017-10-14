using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Player : Character
{
    protected Vector3 m_Movement = Vector3.zero;

    private float m_Horizontal = 0f;
    private float m_Vertical = 0f;
    private Transform m_CamTransform;

    protected override void Awake()
    {
        base.Awake();

        // TODO do this better
        m_CamTransform = Camera.main.transform;

        InputManager.Instance.AddInputEventDelegate(OnInputUpdate, UpdateLoopType.Update);
        VSEventManager.Instance.TriggerEvent(new GameEvents.PlayerSpawnedEvent(this.gameObject));
    }

    protected override void OnDestroy()
    {

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

    protected virtual void FixedUpdate()
    {
        m_Movement = m_CamTransform.forward * m_Vertical + m_CamTransform.right * m_Horizontal;
        m_Movement.y = 0f;

        Move(m_Movement);
    }
}
