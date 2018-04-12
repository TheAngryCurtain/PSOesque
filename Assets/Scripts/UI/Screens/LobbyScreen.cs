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

        LobbyManager.Instance.OnPlayerAdded += OnPlayerAdded;
    }

    public override void Shutdown()
    {
        LobbyManager.Instance.OnPlayerAdded -= OnPlayerAdded;

        base.Shutdown();
    }

    private void OnPlayerAdded(PlayerLobbyData playerData)
    {
        // TODO
        // update a player name bar
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        if (InputLocked()) return;

        bool handled = false;
        switch (data.actionId)
        {
            case RewiredConsts.Action.Confirm:
                handled = true;
                break;
        }

        // pass to base
        if (!handled)
        {
            base.OnInputUpdate(data);
        }
    }

    // receive the event from the screen content animator and pass it on
    public override void OnUIScreenAnimEvent(UIScreenAnimEvent animEvent)
    {
        base.OnUIScreenAnimEvent(animEvent);

        switch (animEvent)
        {
            case UIScreenAnimEvent.Start:
                break;

            case UIScreenAnimEvent.End:
                LobbyManager.Instance.Init();
                break;
        }
    }

    //private void ContinueFlow()
    //{
    //    UIManager.Instance.TransitionToScreen(ScreenId.Title);
    //}
}
