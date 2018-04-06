using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class TitleScreen : UIBaseScreen
{
    public override void Initialize()
    {
        base.Initialize();

        // TODO
        // perhaps start some kind of gentle space audio track
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        bool handled = false;

        switch (data.actionId)
        {
            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    // TODO
                    // transition to main menu

                    // audio
                    //VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestGameplayAudioEvent(true, (AudioManager.eGamePlayClip)random));
                    handled = true;
                }
                break;
        }

        // pass to base
        if (!handled)
        {
            base.OnInputUpdate(data);
        }
    }
}
