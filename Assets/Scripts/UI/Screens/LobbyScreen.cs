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
        LobbyManager.Instance.OnPlayerRemoved += OnPlayerRemoved;
    }

    private void UpdateConfirmed(int index, bool confirmed)
    {
        // TODO update player name bar things

        LobbyManager.Instance.SetConfirmed(index, confirmed);
    }

    public override void Shutdown()
    {
        base.Shutdown();
    }

    private void OnPlayerAdded(PlayerLobbyData playerData)
    {
        Debug.LogFormat("P{0} Joined", playerData.m_PlayerIndex + 1);

        UILobbyPlayerLabel label = m_PlayerLabels[playerData.m_PlayerIndex];
        label.Init(playerData);
        label.AnimateShow(true);
    }

    private void OnPlayerRemoved(PlayerLobbyData playerData)
    {
        Debug.LogFormat("P{0} Dropped", playerData.m_PlayerIndex + 1);

        UILobbyPlayerLabel label = m_PlayerLabels[playerData.m_PlayerIndex];
        label.AnimateShow(false);
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        if (ScreenInputLocked()) return; // can be controller by any player

        // TODO
        // if all are confirmed, update the callout to say advance?

        bool handled = false;
        switch (data.actionId)
        {
            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    if (data.playerId == 0 && LobbyManager.Instance.AllPlayersReady) // TODO only server/host should be able to advance
                    {
                        string popupTitle = "Continue";
                        string popupContent = "Are you ready?";
                        ePopupType popupType = ePopupType.YesNo;
                        PopupManager.Instance.ShowPopup(popupType, popupTitle, popupContent, OnContinuePopupClosed);
                    }
                    else
                    {
                        int playerId = data.playerId;
                        PlayerLobbyData playerData = LobbyManager.Instance.GetLobbyDataForPlayer(playerId);
                        if (playerData != null)
                        {
                            // player data exists, ready up
                            LobbyManager.Instance.SetConfirmed(playerId, true);
                            Debug.LogFormat("P{0} Ready", playerId + 1);
                        }
                        else
                        {
                            // no player exists, request them
                            LobbyManager.Instance.RequestAddPlayer(playerId);
                        }
                    }

                    handled = true;
                }
                break;

            case RewiredConsts.Action.Cancel:
                if (data.GetButtonDown())
                {
                    int playerId = data.playerId;
                    PlayerLobbyData playerData = LobbyManager.Instance.GetLobbyDataForPlayer(playerId);
                    if (playerData != null)
                    {
                        if (playerData.m_Confirmed)
                        {
                            // player is ready, un-ready them
                            LobbyManager.Instance.SetConfirmed(playerId, false);
                            Debug.LogFormat("P{0} Un-ready", playerId + 1);
                        }
                        else
                        {
                            if (playerId == 0) // need to change this to a host player id check
                            {
                                string popupTitle = "Exit";
                                string popupContent = "Are you sure you want to leave the lobby?";
                                ePopupType popupType = ePopupType.YesNo;
                                PopupManager.Instance.ShowPopup(popupType, popupTitle, popupContent, OnExitPopupClosed);
                            }
                            else
                            {
                                // player wants to drop out
                                LobbyManager.Instance.RequestRemovePlayer(playerId);
                            }
                        }
                    }
                }
                break;
        }

        // pass to base
        if (!handled)
        {
            base.OnInputUpdate(data);
        }
    }

    private void OnContinuePopupClosed(bool result)
    {
        if (result)
        {
            UIManager.Instance.ScreenAfterLoadID = ScreenId.None;
            UIManager.Instance.TransitionToScreen(ScreenId.Loading);

            SceneLoader.Instance.RequestSceneLoad(Enums.eScene.Game);
        }

        PopupManager.Instance.ClosePopup();
    }

    private void OnExitPopupClosed(bool result)
    {
        if (result)
        {
            UIManager.Instance.ScreenAfterLoadID = ScreenId.Title;
            UIManager.Instance.TransitionToScreen(ScreenId.Loading);

            SceneLoader.Instance.RequestSceneLoad(Enums.eScene.Main);
        }

        PopupManager.Instance.ClosePopup();
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
                if (m_ActiveState == UIScreenAnimState.Intro)
                {
                    LobbyManager.Instance.Init();
                }
                else
                {
                    // need to clear the lobby manager going back
                    //Destroy(LobbyManager.Instance.gameObject);
                }
                break;
        }
    }

    //private void ContinueFlow()
    //{
    //    UIManager.Instance.TransitionToScreen(ScreenId.Title);
    //}
}
