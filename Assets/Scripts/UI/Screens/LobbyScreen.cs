using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class LobbyScreen : UIBaseScreen
{
    [SerializeField] private UILobbyPlayerLabel[] m_PlayerLabels;

    public override void Initialize()
    {
        base.Initialize();

        LobbyManager.Instance.OnPlayerAdded += OnPlayerAdded;
    }

    private void UpdateConfirmed(int index, bool confirmed)
    {
        // TODO update player name bar things

        LobbyManager.Instance.SetConfirmed(index, confirmed);
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
        m_PlayerLabels[playerData.m_PlayerIndex].Init(playerData);
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        if (InputLocked()) return;

        // TODO
        // if all are confirmed, update the callout to say advance

        bool handled = false;
        switch (data.actionId)
        {
            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    if (data.playerId == 0 && LobbyManager.Instance.AllPlayersReady) // only player 1 advances
                    {
                        // TODO advance to next screen
                        Debug.Log("CONTINUE");
                    }
                    else
                    {
                        LobbyManager.Instance.SetConfirmed(data.playerId, true);
                        Debug.LogFormat("P{0} Confirmed", data.playerId);
                    }

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
