using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class CharacterCreatorScreen : UIBaseScreen
{
    public int m_ActiveIndex = 0;

    private float m_ScrollDelay = 0.25f;
    private float m_CurrentTime = 0f;
    private int m_ControllingPlayerID = -1;

    public override void Initialize(object[] screenParams)
    {
        base.Initialize(screenParams);

        // TODO
        // Need to revist this screen when characters are more fleshed out

        m_ControllingPlayerID = (int)screenParams[2];
        Debug.LogFormat("Controlling Player: {0}", m_ControllingPlayerID);
    }

    public override void Shutdown()
    {
        base.Shutdown();
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        if (ScreenInputLocked()|| (m_ControlledBySinglePlayer && data.playerId != m_ControllingPlayerID)) return;

        bool handled = false;
        switch (data.actionId)
        {
            //case RewiredConsts.Action.Navigate_Vertical:
            //    float value = data.GetAxis();
            //    if (value != 0f && m_CurrentTime <= 0f)
            //    {
            //        if (value < 0f)
            //        {
            //            m_ActiveIndex = (m_ActiveIndex + 1) % m_ListItems.Count;
            //        }
            //        else if (value > 0f)
            //        {
            //            if (m_ActiveIndex - 1 < 0)
            //            {
            //                m_ActiveIndex = m_ListItems.Count;
            //            }

            //            m_ActiveIndex -= 1;
            //        }

            //        m_CurrentTime = m_ScrollDelay;

            //        // audio
            //        //VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestUIAudioEvent(true, AudioManager.eUIClip.Navigate));

            //        handled = true;
            //    }
            //    m_CurrentTime -= Time.deltaTime;
            //    break;

            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    handled = true;
                }
                break;

            case RewiredConsts.Action.Cancel:
                if (data.GetButtonDown())
                {
                    handled = true;
                }
                break;

            case RewiredConsts.Action.Y_Action:
                if (data.GetButtonDown())
                {
                    string popupTitle = "Character";
                    string popupContent = "Are you finished creating this character?";
                    ePopupType popupType = ePopupType.YesNo;
                    PopupManager.Instance.ShowPopup(popupType, popupTitle, popupContent, OnCreationFinished);

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

    private void OnCreationFinished(bool result)
    {
        if (result)
        {
            // TODO
            // save the character

            object[] screenParams = new object[]
            {
                UI.Enums.ScreenId.Lobby,
                Enums.eScene.Lobby
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
}
