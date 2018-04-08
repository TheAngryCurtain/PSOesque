using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class LobbyScreen : UIBaseScreen
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnUIScreenAnimEvent(UIScreenAnimEvent animEvent)
    {
        base.OnUIScreenAnimEvent(animEvent);

        switch (animEvent)
        {
            case UIScreenAnimEvent.Start:
                break;

            case UIScreenAnimEvent.End:
                break;

            case UIScreenAnimEvent.None:
            default:
                break;
        }
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        if (InputLocked()) return;

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
