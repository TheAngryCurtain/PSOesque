﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class LegalScreen : UIBaseScreen
{
    [SerializeField] private float mLogoScreenTime = 2f;

    public override void Initialize(object[] screenParams)
    {
        base.Initialize(screenParams);
    }

    public override void OnUIScreenAnimEvent(UIScreenAnimEvent animEvent)
    {
        base.OnUIScreenAnimEvent(animEvent);

        switch (animEvent)
        {
            case UIScreenAnimEvent.Start:
                break;

            case UIScreenAnimEvent.End:
                if (m_ActiveState == UIScreenAnimState.Intro)
                {
                    Invoke("ContinueFlow", mLogoScreenTime);
                }
                break;

            case UIScreenAnimEvent.None:
            default:
                break;
        }
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        if (ScreenInputLocked() || (m_ControlledBySinglePlayer && data.playerId != 0)) return;

        bool handled = false;
        switch (data.actionId)
        {
#if UNITY_EDITOR
            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    CancelInvoke("ContinueFlow");
                    ContinueFlow();

                    handled = true;
                }
                break;
#endif
        }

        // pass to base
        if (!handled)
        {
            base.OnInputUpdate(data);
        }
    }

    private void ContinueFlow()
    {
        UIManager.Instance.TransitionToScreen(ScreenId.Title);
    }
}
