using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class UICharacterSelector : MonoBehaviour
{
    [SerializeField] private Transform m_ScrollContent;
    [SerializeField] private GameObject m_ScrollItemPrefab;

    public System.Action<int, int> OnCharacterSelected;

    public bool m_IsActive = false;
    public int m_PlayerIndex = -1;
    public int m_ActiveIndex = 0;

    private List<UIScrollViewItem> m_ListItems;
    private UIScrollViewItem m_CurrentlySelected = null;
    private float m_ScrollDelay = 0.25f;
    private float m_CurrentTime = 0f;

    public void Init()
    {
        m_ListItems = new List<UIScrollViewItem>();
    }

    public void SetData(List<CharacterProgress> characterData)
    {
        m_ListItems.Clear();

        for (int i = 0; i < characterData.Count; i++)
        {
            GameObject itemObj = (GameObject)Instantiate(m_ScrollItemPrefab, m_ScrollContent);
            UIScrollViewItem scrollItem = itemObj.GetComponent<UIScrollViewItem>();
            if (scrollItem != null)
            {
                scrollItem.SetData(characterData[i].m_Stats.PlayerName, null); // eventually pass the icon sprite in here
            }
            else
            {
                Debug.LogErrorFormat("UIScrollViewItem is missing the script!");
            }

            m_ListItems.Add(scrollItem);
        }

        SetSelected(m_ActiveIndex);
        ResizeContainer();
    }

    private void ResizeContainer()
    {
        float itemHeight = 40.5f - 10f; // this is lazy. Should just grab the value
        RectTransform rTransform = m_ScrollContent.GetComponent<RectTransform>();
        rTransform.sizeDelta = new Vector2(rTransform.sizeDelta.x, itemHeight * m_ListItems.Count);
    }

    public bool OnInputUpdate(InputActionEventData data)
    {
        bool handled = false;
        switch (data.actionId)
        {
            case RewiredConsts.Action.Navigate_Vertical:
                float value = data.GetAxis();
                if (value != 0f && m_CurrentTime <= 0f)
                {
                    if (value < 0f)
                    {
                        m_ActiveIndex = (m_ActiveIndex + 1) % m_ListItems.Count;
                    }
                    else if (value > 0f)
                    {
                        if (m_ActiveIndex - 1 < 0)
                        {
                            m_ActiveIndex = m_ListItems.Count;
                        }

                        m_ActiveIndex -= 1;
                    }

                    m_CurrentTime = m_ScrollDelay;
                    SetSelected(m_ActiveIndex);

                    // audio
                    //VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestUIAudioEvent(true, AudioManager.eUIClip.Navigate));

                    handled = true;
                }
                m_CurrentTime -= Time.deltaTime;
                break;

            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    if (OnCharacterSelected != null)
                    {
                        OnCharacterSelected(m_PlayerIndex, m_ActiveIndex);
                    }

                    handled = true;
                }
                break;

            case RewiredConsts.Action.Cancel:
                if (data.GetButtonDown())
                {
                    if (OnCharacterSelected != null)
                    {
                        OnCharacterSelected(m_PlayerIndex, -1);
                    }

                    handled = true;
                }
                break;
        }

        return handled;
    }

    public void SetIsActive(bool active)
    {
        m_IsActive = active;
        gameObject.SetActive(active);
    }

    private void SetSelected(int index)
    {
        if (m_CurrentlySelected != null)
        {
            m_CurrentlySelected.SetSelected(false);
        }

        m_CurrentlySelected = m_ListItems[index];
        m_CurrentlySelected.SetSelected(true);
    }
}
