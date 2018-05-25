using UnityEngine;
using Rewired;

public class PlayableCharacter : Character
{
    [SerializeField] private WorldSpaceCallout m_Callout;

    protected int m_PlayerId;
    protected int m_SaveSlot;

    protected Vector3 m_Movement = Vector3.zero;

    protected float m_Horizontal = 0f;
    protected float m_Vertical = 0f;
    protected Transform m_CamTransform;

    private IInteractable m_CurrentInteractable = null;

    protected override void Awake()
    {
        base.Awake();

        CharacterManager.Instance.RegisterCharacter(this);
    }

    public void AssignCamera(Transform cam)
    {
        m_CamTransform = cam;
        m_Callout.AssignCamera(cam);
    }

    public void SetPlayerActive(bool active)
    {
        if (active)
        {
            InputManager.Instance.AddInputEventDelegate(OnInputUpdate, UpdateLoopType.Update);
        }
        else
        {
            InputManager.Instance.RemoveInputEventDelegate(OnInputUpdate);
        }
    }

    protected override void OnDestroy()
    {

    }

    protected virtual void OnInputUpdate(InputActionEventData data)
    {
        if (data.playerId == m_PlayerId)
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
                            m_CurrentInteractable.Interact(m_Callout, m_SaveSlot);
                            m_CurrentInteractable = null;
                        }
                    }
                    break;
            }

            m_Movement = m_CamTransform.forward * m_Vertical + m_CamTransform.right * m_Horizontal;
            m_Movement.y = 0f;
        }
    }

    protected virtual void FixedUpdate()
    {
        Move(m_Movement.normalized);
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
                m_CurrentInteractable.Highlight(m_Callout);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (m_CurrentInteractable != null)
        {
            m_CurrentInteractable.Unhighlight(m_Callout);
            m_CurrentInteractable = null;
        }
    }
}
