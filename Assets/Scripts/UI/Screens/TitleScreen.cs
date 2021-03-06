﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class TitleScreen : UIBaseScreen
{
    [SerializeField] private Animator m_ScreenContentAnimator;
    [SerializeField] private UIAnimatorRelay m_ContentRelay;
    [SerializeField] private List<UIPromptInfo> m_ContentPromptInfo;
    [SerializeField] private UIMenu m_MainMenu;

	private enum eMenuOption { Offline, Online, Settings, Exit };

    private enum eScreenState { Title, Menu };
    private eScreenState m_State;

    public override void Initialize(object[] screenParams)
    {
        base.Initialize(screenParams);

        // TODO
        // perhaps start some kind of gentle space audio track

        m_State = eScreenState.Title;

        // passing this along will handle input blocking while the screen content is animating
        m_ContentRelay.OnAnimationEvent += ContentAnimationEvent;

        m_MainMenu.OnItemSelected += OnMenuItemSelected;
        m_MainMenu.PopulateMenu();
    }

    public override void Shutdown()
    {
        base.Shutdown();

        m_MainMenu.OnItemSelected -= OnMenuItemSelected;
        m_ContentRelay.OnAnimationEvent -= ContentAnimationEvent;
    }

    private void OnMenuItemSelected(int index)
    {
		eMenuOption option = (eMenuOption)index;
		switch (option)
        {
			case eMenuOption.Offline:
			case eMenuOption.Online:
                object[] screenParams = null;

                // need to check if we currently have any characters created
                // if not, we need to divert to the character creator
                bool hasCreatedCharacter = (CharacterManager.Instance.SavedCharacterCount > 0);
                if (hasCreatedCharacter)
                {
                    screenParams = new object[]
                    {
                        UI.Enums.ScreenId.Lobby,
                        Enums.eScene.Lobby
                    };
                }
                else
                {
                    screenParams = new object[]
                    {
                        UI.Enums.ScreenId.CharacterCreator,
                        Enums.eScene.CharacterCreator,
                        0, // controlling character index
                        false // is it possible to abandon creation?
                    };
                }

                UIManager.Instance.TransitionToScreen(ScreenId.Loading, screenParams);
                break;

			case eMenuOption.Settings:
                break;

			case eMenuOption.Exit:
                break;
        }
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        if (ScreenInputLocked() || (m_ControlledBySinglePlayer && data.playerId != 0 )) return;

        bool handled = false;
        if (m_State == eScreenState.Title)
        {
            switch (data.actionId)
            {
                case RewiredConsts.Action.Confirm:
                    if (data.GetButtonDown())
                    {
                        m_ScreenContentAnimator.SetTrigger("OpenMenu");

                        m_MainMenu.RefocusMenu();
                        UIManager.Instance.RefreshPrompts(m_ContentPromptInfo);

                        // audio
                        //VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestGameplayAudioEvent(true, (AudioManager.eGamePlayClip)random));
                        handled = true;
                    }
                    break;
            }
        }
        else
        {
            handled = m_MainMenu.HandleInput(data);

            switch (data.actionId)
            {
                case RewiredConsts.Action.Cancel:
                    if (data.GetButtonDown())
                    {
                        m_ScreenContentAnimator.SetTrigger("CloseMenu");

                        UIManager.Instance.RefreshPrompts();

                        // audio
                        //VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestGameplayAudioEvent(true, (AudioManager.eGamePlayClip)random));
                        handled = true;
                    }
                    break;
            }
        }

        // pass to base
        if (!handled)
        {
            base.OnInputUpdate(data);
        }
    }

    // receive the event from the screen content animator and pass it on
    public void ContentAnimationEvent(UIScreenAnimEvent animEvent)
    {
        OnUIScreenAnimEvent(animEvent);

        switch (animEvent)
        {
            case UIScreenAnimEvent.Start:
                break;

            case UIScreenAnimEvent.End:
                if (m_State == eScreenState.Title)
                {
                    m_State = eScreenState.Menu;
                }
                else
                {
                    m_State = eScreenState.Title;
                }
                break;
        }
    }
}
