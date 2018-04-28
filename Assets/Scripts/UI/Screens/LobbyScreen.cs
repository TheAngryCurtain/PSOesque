using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class LobbyScreen : UIBaseScreen
{
    [SerializeField] private UILobbyPlayerLabel[] m_PlayerLabels;
    [SerializeField] private UICharacterSelector[] m_CharacterSelectors;

    private List<CharacterProgress> m_CharacterList;

    public override void Initialize(object[] screenParams)
    {
        base.Initialize(screenParams);

        LobbyManager.Instance.OnPlayerAdded += OnPlayerAdded;
        LobbyManager.Instance.OnPlayerRemoved += OnPlayerRemoved;

        // TEST for now, create 4 temp players
        for (int i = 0; i < LobbyManager.MAX_PLAYERS; i++)
        {
            CharacterProgress playerProgress = new CharacterProgress();
            playerProgress.Init();

            CharacterManager.Instance.AddCharacterProgressInSlot(playerProgress);
        }

        // populate character list
        int totalSlots = CharacterProgressData.MAX_CHARACTER_SLOTS;
        m_CharacterList = new List<CharacterProgress>(totalSlots);
        for (int i = 0; i < totalSlots; i++)
        {
            CharacterProgress progress = CharacterManager.Instance.GetProgressForCharacterInSlot(i);
            if (progress != null)
            {
                m_CharacterList.Add(progress);
            }
        }

        for (int i = 0; i < m_CharacterSelectors.Length; i++)
        {
            m_CharacterSelectors[i].Init();
            m_CharacterSelectors[i].m_PlayerIndex = i;
            m_CharacterSelectors[i].SetData(m_CharacterList);
            m_CharacterSelectors[i].OnCharacterSelected += OnCharacterSelected;
        }
    }

    private void OnCharacterSelected(int playerIndex, int selectedIndex)
    {
        m_CharacterSelectors[playerIndex].SetIsActive(false);

        if (selectedIndex > -1)
        {
            LobbyManager.Instance.RequestAddPlayer(playerIndex, selectedIndex);

            // remove that character from the list
            UpdateCharacterList(selectedIndex);
        }
    }

    private void UpdateConfirmed(int index, bool confirmed)
    {
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

        // this is ugly
        if (m_CharacterSelectors[data.playerId].m_IsActive)
        {
            handled = m_CharacterSelectors[data.playerId].OnInputUpdate(data);
            if (handled)
            {
                return;
            }
        }

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
                            if (!m_CharacterSelectors[data.playerId].m_IsActive)
                            {
                                m_CharacterSelectors[data.playerId].SetIsActive(true);
                            }

                            //int saveSlot = 0;
                            //LobbyManager.Instance.RequestAddPlayer(playerId, saveSlot);
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

    private void UpdateCharacterList(int indexToRemove = -1)
    {
        if (indexToRemove > -1)
        {
            m_CharacterList.RemoveAt(indexToRemove);
        }

        for (int i = 0; i < m_CharacterSelectors.Length; i++)
        {
            m_CharacterSelectors[i].SetData(m_CharacterList);
        }
    }

    private void OnContinuePopupClosed(bool result)
    {
        if (result)
        {
            object[] screenParams = new object[]
            {
                UI.Enums.ScreenId.HUD,
                Enums.eScene.Game
            };

            UIManager.Instance.TransitionToScreen(ScreenId.Loading, screenParams);
        }

        PopupManager.Instance.ClosePopup();
    }

    private void OnExitPopupClosed(bool result)
    {
        if (result)
        {
            // clear all players
            int playerCount = LobbyManager.Instance.ConnectedPlayerCount;
            for (int i = 0; i < playerCount; i++)
            {
                LobbyManager.Instance.RequestRemovePlayer(i);
            }

            object[] screenParams = new object[]
            {
                UI.Enums.ScreenId.Title,
                Enums.eScene.Main
            };

            UIManager.Instance.TransitionToScreen(ScreenId.Loading, screenParams);
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
                break;
        }
    }

    //private void ContinueFlow()
    //{
    //    UIManager.Instance.TransitionToScreen(ScreenId.Title);
    //}
}
