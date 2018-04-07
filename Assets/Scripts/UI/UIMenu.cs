using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenu : MonoBehaviour
{
    public System.Action<int> OnItemSelected;
    public System.Action<UIMenuItem> OnItemHighlighted;

    [SerializeField] private string[] m_MenuLabels;

    [SerializeField] private Transform m_MenuContainer;
    [SerializeField] private GameObject m_MenuItemPrefab;

    [SerializeField] private Color m_DefaultFrameColor;
    [SerializeField] private Color m_DefaultTextColor;
    [SerializeField] private Color m_HighlightFrameColor;
    [SerializeField] private Color m_HighlightTextColor;

    private float m_ScrollDelay = 0.25f;
    private float m_CurrentTime = 0f;

    private UIMenuItem[] m_ListItems;
    private UIMenuItem m_ActiveItem;
    private int m_ActiveIndex = 0;

    public void PopulateMenu()
    {
        m_ListItems = new UIMenuItem[m_MenuLabels.Length];
        for (int i = 0; i < m_MenuLabels.Length; i++)
        {
            GameObject itemObj = (GameObject)Instantiate(m_MenuItemPrefab, m_MenuContainer);
            UIMenuItem item = itemObj.GetComponent<UIMenuItem>();
            if (item != null)
            {
                item.DefaultItem(m_DefaultFrameColor, m_DefaultTextColor, m_HighlightFrameColor, m_HighlightTextColor);
                item.SetLabel(m_MenuLabels[i]);

                m_ListItems[i] = item;
            }
        }

        SetActiveItem(m_ListItems[m_ActiveIndex]);
    }

    public void ClearMenu()
    {
        for (int i = 0; i < m_MenuContainer.childCount; i++)
        {
            m_ListItems[i] = null;
            Destroy(m_MenuContainer.GetChild(i).gameObject);
        }
    }

    private void SetActiveItem(UIMenuItem item)
    {
        if (m_ActiveItem != null && m_ActiveItem != item)
        {
            m_ActiveItem.Highlight(false);
        }

        m_ActiveItem = item;
        m_ActiveItem.Highlight(true);
        EventSystem.current.SetSelectedGameObject(m_ActiveItem.gameObject);

        if (OnItemHighlighted != null)
        {
            OnItemHighlighted(item);
        }
    }

    public void RefocusMenu()
    {
        m_ActiveIndex = 0;
        SetActiveItem(m_ListItems[m_ActiveIndex]);
    }

    public void RemoveMenuFocus()
    {
        if (m_ActiveItem != null)
        {
            m_ActiveItem.Highlight(false);
            m_ActiveItem = null;
        }
    }

    public bool HandleInput(Rewired.InputActionEventData data)
    {
        bool handled = false;
        switch (data.actionId)
        {
            //case RewiredConsts.Action.Navigate_Horizontal:
            //    float value = data.GetAxis();
            //    if (value != 0f && m_MenuItems[m_ActiveIndex].m_Togglable)
            //    {
            //        // will need to figure out how to cut out of this. will likely need to change the switch to an if/else

            //        // audio
            //        VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestUIAudioEvent(true, AudioManager.eUIClip.Navigate));
            //    }
            //    break;

            case RewiredConsts.Action.Navigate_Vertical:
                float value = data.GetAxis();
                if (value != 0f && m_CurrentTime <= 0f)
                {
                    if (value < 0f)
                    {
                        m_ActiveIndex = (m_ActiveIndex + 1) % m_MenuLabels.Length;
                    }
                    else if (value > 0f)
                    {
                        if (m_ActiveIndex - 1 < 0)
                        {
                            m_ActiveIndex = m_MenuLabels.Length;
                        }

                        m_ActiveIndex -= 1;
                    }

                    SetActiveItem(m_ListItems[m_ActiveIndex]);
                    m_CurrentTime = m_ScrollDelay;

                    // audio
                    //VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestUIAudioEvent(true, AudioManager.eUIClip.Navigate));

                    handled = true;
                }
                m_CurrentTime -= Time.deltaTime;
                break;

            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    OnItemSelected(m_ActiveIndex);

                    // audio
                    //VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestUIAudioEvent(true, AudioManager.eUIClip.Confirm));

                    handled = true;
                }
                break;
        }

        return handled;
    }
}
