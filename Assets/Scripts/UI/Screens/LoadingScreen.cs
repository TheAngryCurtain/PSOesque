using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.UI;
using Rewired;

public class LoadingScreen : UIBaseScreen
{
    [SerializeField] private Transform m_ReticleTransform;
    [SerializeField] private float m_MoveSpeed = 8f;

    private float m_Horizontal;
    private float m_Vertical;

    public override void Initialize()
    {
        base.Initialize();

        // for now, wait 2 seconds and move to the next screen. Will likely need to change this to an async load later
        Invoke("Advance", 2f);
    }

    public override void Shutdown()
    {
        base.Shutdown();
    }

    protected override void OnInputUpdate(InputActionEventData data)
	{
		if (InputLocked()) return;

		bool handled = false;

        switch (data.actionId)
		{
            case RewiredConsts.Action.Navigate_Horizontal:
                m_Horizontal = data.GetAxis();
                handled = true;
                break;

            case RewiredConsts.Action.Navigate_Vertical:
                m_Vertical = data.GetAxis();
                handled = true;
                break;
        }

        Vector3 moveDirection = (Vector3.up * m_Vertical + Vector3.right * m_Horizontal);
        if (moveDirection != Vector3.zero)
        {
            m_ReticleTransform.Translate(moveDirection * m_MoveSpeed * Time.deltaTime, UnityEngine.Space.World);
        }

        // pass to base
        if (!handled)
		{
			base.OnInputUpdate(data);
		}
	}

    private void Advance()
    {
        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Lobby);
    }
}
