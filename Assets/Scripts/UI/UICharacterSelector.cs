using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class UICharacterSelector : MonoBehaviour
{
    [SerializeField] private Transform m_ScrollContent;
    [SerializeField] private Text m_HeaderLabel;
    [SerializeField] private GameObject m_ScrollItemPrefab;

    public System.Action<int, int> OnCharacterSelected;

    public bool m_IsActive = false;
    public int m_PlayerIndex = -1;
    public int m_ActiveIndex = 0;

    private List<UIScrollViewItem> m_ListItems;
    private UIScrollViewItem m_CurrentlySelected = null;
    private float m_ScrollDelay = 0.25f;
    private float m_CurrentTime = 0f;

    public void Init(int playerID)
    {
        m_ListItems = new List<UIScrollViewItem>();
        m_HeaderLabel.text = string.Format("P{0}", playerID + 1);
    }

    public void SetData(List<UILobbyCharacterProgress> characterData)
    {
        EmptyList();
        for (int i = 0; i < characterData.Count; i++)
        {
            GameObject itemObj = (GameObject)Instantiate(m_ScrollItemPrefab, m_ScrollContent);
            UIScrollViewItem scrollItem = itemObj.GetComponent<UIScrollViewItem>();
            if (scrollItem != null)
            {
                scrollItem.SetData(characterData[i].Progress.m_Stats.PlayerName, null, characterData[i].AssignedToPlayer); // eventually pass the icon sprite in here
            }
            else
            {
                Debug.LogErrorFormat("UIScrollViewItem is missing the script!");
            }

            m_ListItems.Add(scrollItem);
        }

        SetSelected();
    }

    private void EmptyList()
    {
        m_ListItems.Clear();

        int itemObjCount = m_ScrollContent.childCount;
        for (int i = 0; i < itemObjCount; i++)
        {
            Destroy(m_ScrollContent.GetChild(i).gameObject);
        }
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
                    SetSelected();

                    // audio
                    //VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestUIAudioEvent(true, AudioManager.eUIClip.Navigate));

                    handled = true;
                }
                m_CurrentTime -= Time.deltaTime;
                break;

            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    if (!m_ListItems[m_ActiveIndex].m_Disabled && OnCharacterSelected != null)
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

        // reset index to the top
        if (m_IsActive)
        {
            m_ActiveIndex = 0;
        }
    }

    private void SetSelected()
    {
        if (m_CurrentlySelected != null)
        {
            m_CurrentlySelected.SetSelected(false);
        }

        // in the event that another player picks the character you have selected
        // we need to put the index back in range when that character is removed
        if (m_ActiveIndex >= m_ListItems.Count)
        {
            m_ActiveIndex = m_ListItems.Count - 1;
        }

        m_CurrentlySelected = m_ListItems[m_ActiveIndex];
        m_CurrentlySelected.SetSelected(true);
    }
}
