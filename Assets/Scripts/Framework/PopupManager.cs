using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public enum ePopupType { Confirm, YesNo };

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GameObject m_PopupPrefab;
    [SerializeField] private UIPromptInfo[] m_Prompts;

    public bool PopupOpen { get { return m_CurrentPopup != null; } }

    private UIPopup m_CurrentPopup = null;
    private ePopupType m_CurrentPopupType;
    private System.Action<bool> m_CloseCallback = null;

    public void ShowPopup(ePopupType type, string title, string content, System.Action<bool> callback)
    {
        GameObject popupObj = (GameObject)Instantiate(m_PopupPrefab, m_Canvas.transform);
        m_CurrentPopup = popupObj.GetComponent<UIPopup>(); // TODO should probably use a stack for easily managing multiple popups
        m_CurrentPopupType = type;
        m_CloseCallback = callback;

        List<UIPromptInfo> prompts = new List<UIPromptInfo>() { m_Prompts[0] };
        switch (m_CurrentPopupType)
        {
            case ePopupType.Confirm:
                m_Prompts[0].m_LabelText = "Okay";
                break;

            case ePopupType.YesNo:
                m_Prompts[0].m_LabelText = "Yes";
                m_Prompts[1].m_LabelText = "No";

                prompts.Add(m_Prompts[1]);
                break;
        }

        m_CurrentPopup.SetData(title, content, prompts);
        m_CurrentPopup.Show();

        InputManager.Instance.AddInputEventDelegate(OnInputUpdate, UpdateLoopType.Update);
    }

    public void ClosePopup()
    {
        InputManager.Instance.RemoveInputEventDelegate(OnInputUpdate);

        m_CurrentPopup.Hide();

        Destroy(m_CurrentPopup.gameObject);
        m_CurrentPopup = null;
        m_CloseCallback = null;
    }

    private void OnInputUpdate(InputActionEventData data)
    {
        switch (data.actionId)
        {
            case RewiredConsts.Action.Confirm:
                if (data.GetButtonDown())
                {
                    if (m_CloseCallback != null)
                    {
                        m_CloseCallback(true);
                    }
                }
                break;

            case RewiredConsts.Action.Cancel:
                if (data.GetButtonDown() && m_CurrentPopupType == ePopupType.YesNo)
                {
                    if (m_CloseCallback != null)
                    {
                        m_CloseCallback(false);
                    }
                }
                break;
        }
    }
}
