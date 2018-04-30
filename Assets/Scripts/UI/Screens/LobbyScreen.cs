using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class UILobbyCharacterProgress
{
    public CharacterProgress Progress;
    public bool AssignedToPlayer = false;

    public UILobbyCharacterProgress(CharacterProgress cP, bool assigned)
    {
        Progress = cP;
        AssignedToPlayer = assigned;
    }
}

public class LobbyScreen : UIBaseScreen
{
    [SerializeField] private UILobbyPlayerLabel[] m_PlayerLabels;
    [SerializeField] private UICharacterSelector[] m_CharacterSelectors;

    private List<UILobbyCharacterProgress> m_CharacterList;
    private int m_CreatorPlayerID = -1;

    public override void Initialize(object[] screenParams)
    {
        base.Initialize(screenParams);

        LobbyManager.Instance.OnPlayerAdded += OnPlayerAdded;
        LobbyManager.Instance.OnPlayerRemoved += OnPlayerRemoved;

        // populate character list
        int totalSlots = CharacterProgressData.MAX_CHARACTER_SLOTS;
        m_CharacterList = new List<UILobbyCharacterProgress>(totalSlots);
        for (int i = 0; i < totalSlots; i++)
        {
            CharacterProgress progress = CharacterManager.Instance.GetProgressForCharacterInSlot(i);
            if (progress != null)
            {
                UILobbyCharacterProgress lCP = new UILobbyCharacterProgress(progress, false);
                m_CharacterList.Add(lCP);
            }
        }

        for (int i = 0; i < m_CharacterSelectors.Length; i++)
        {
            m_CharacterSelectors[i].Init(i);
            m_CharacterSelectors[i].m_PlayerIndex = i;
            m_CharacterSelectors[i].SetData(m_CharacterList);
            m_CharacterSelectors[i].OnCharacterSelected += OnCharacterSelected;
        }
    }

    //private void CheckForPreviousLobbyState()
    //{
    //    for (int i = 0; i < LobbyManager.MAX_PLAYERS; i++)
    //    {
    //        PlayerLobbyData playerData = LobbyManager.Instance.GetLobbyDataForPlayer(i);
    //        if (playerData != null)
    //        {
    //            // probably won't be able to rebuild lobby in multiplayer like this, but you can't create a player for online in the lobby anyway!
    //            LobbyManager.Instance.RequestAddPlayer(i, CharacterManager.Instance.GetProgressForCharacterInSlot(i));

    //            if (!playerData.m_Confirmed)
    //            {
    //                // player data exists, ready up
    //                LobbyManager.Instance.SetConfirmed(i, true);
    //                Debug.LogFormat("Previous Data >>> P{0} Ready", i + 1);
    //            }
    //        }
    //    }
    //}

    private void OnCharacterSelected(int playerIndex, int selectedIndex)
    {
        m_CharacterSelectors[playerIndex].SetIsActive(false);

        if (selectedIndex > -1)
        {
            LobbyManager.Instance.RequestAddPlayer(playerIndex, m_CharacterList[selectedIndex].Progress);

            // hide that item in the list so other players can't pick it
            HideCharacterInList(selectedIndex);
        }
    }

    private void UpdateConfirmed(int index, bool confirmed)
    {
        LobbyManager.Instance.SetConfirmed(index, confirmed);
    }

    public override void Shutdown()
    {
        base.Shutdown();

        for (int i = 0; i < m_CharacterSelectors.Length; i++)
        {
            m_CharacterSelectors[i].OnCharacterSelected -= OnCharacterSelected;
        }
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
            case RewiredConsts.Action.Y_Action:
                if (data.GetButtonDown())
                {
                    string popupTitle = string.Format("P{0}", data.playerId + 1);
                    string popupContent = "Would you like to create a new character?";
                    ePopupType popupType = ePopupType.YesNo;
                    PopupManager.Instance.ShowPopup(popupType, popupTitle, popupContent, OnCharacterCreationPopupClosed);

                    // save which player opened it
                    m_CreatorPlayerID = data.playerId;

                    handled = true;
                }
                break;

            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    if (data.playerId == 0 && LobbyManager.Instance.AllPlayersReady && !AnyCharacterSelectorOpen()) // TODO only server/host should be able to advance
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
                            if (!playerData.m_Confirmed)
                            {
                                // player data exists, ready up
                                LobbyManager.Instance.SetConfirmed(playerId, true);
                                Debug.LogFormat("P{0} Ready", playerId + 1);
                            }
                        }
                        else
                        {
                            // no player exists, open the character selector
                            if (!m_CharacterSelectors[data.playerId].m_IsActive)
                            {
                                m_CharacterSelectors[data.playerId].SetIsActive(true);
                            }
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

                                // need to add back in the character data from any player who backed out
                                ShowCharacterInList(m_CharacterList[playerId]);
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

    private bool AnyCharacterSelectorOpen()
    {
        for (int i = 0; i < m_CharacterSelectors.Length; i++)
        {
            if (m_CharacterSelectors[i].m_IsActive)
            {
                return true;
            }
        }

        return false;
    }

    private void HideCharacterInList(int index)
    {
        if (index > -1 && index < m_CharacterList.Count)
        {
            m_CharacterList[index].AssignedToPlayer = true;
        }
        else
        {
            Debug.LogWarningFormat("Tried to remove null from character list");
        }

        RefreshCharacterList();
    }

    private void ShowCharacterInList(UILobbyCharacterProgress prog)
    {
        if (prog != null)
        {
            int index = m_CharacterList.IndexOf(prog);
            if (index > -1)
            {
                m_CharacterList[index].AssignedToPlayer = false;
            }
            else
            {
                Debug.LogWarningFormat("chracter progress not in character list");
            }
        }
        else
        {
            Debug.LogWarningFormat("Tried to add null to character list");
        }

        RefreshCharacterList();
    }

    private void RefreshCharacterList()
    {
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

    private void OnCharacterCreationPopupClosed(bool result)
    {
        if (result)
        {
            object[] screenParams = new object[]
            {
                UI.Enums.ScreenId.CharacterCreator,
                Enums.eScene.CharacterCreator,
                m_CreatorPlayerID,
                true // possible to abandon creation?
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
                    //CheckForPreviousLobbyState();
                }
                break;
        }
    }
}
