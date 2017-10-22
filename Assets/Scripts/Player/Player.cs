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

    private IInteractable m_CurrentInteractable = null;

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

            case RewiredConsts.Action.Interact:
                if (data.GetButtonDown())
                {
                    if (m_CurrentInteractable != null)
                    {
                        m_CurrentInteractable.Interact();
                    }
                }
                break;
        }
    }

    protected virtual void FixedUpdate()
    {
        m_Movement = m_CamTransform.forward * m_Vertical + m_CamTransform.right * m_Horizontal;
        m_Movement.y = 0f;

        Move(m_Movement);
    }

    // TODO set up a better way of interactions?
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            IInteractable interactObj = other.gameObject.GetComponent<IInteractable>();
            if (interactObj != null && interactObj != m_CurrentInteractable)
            {
                m_CurrentInteractable = interactObj;
                m_CurrentInteractable.Highlight();
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (m_CurrentInteractable != null)
        {
            m_CurrentInteractable.Unhighlight();
            m_CurrentInteractable = null;
        }
    }
}
