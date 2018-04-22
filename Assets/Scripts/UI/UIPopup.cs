﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private Text m_TitleLabel;
    [SerializeField] private Text m_ContentText;
    [SerializeField] private Transform m_PromptsBar;
    [SerializeField] private GameObject m_PromptPrefab;

    public void SetData(string title, string content, List<UIPromptInfo> promptInfo)
    {
        m_TitleLabel.text = title;
        m_ContentText.text = content;

        for (int i = 0; i < promptInfo.Count; i++)
        {
            GameObject promptObj = (GameObject)Instantiate(m_PromptPrefab, m_PromptsBar);
            UIPrompt prompt = promptObj.GetComponent<UIPrompt>();
            if (prompt != null)
            {
                prompt.SetIcon(promptInfo[i].m_IconSprite); // TODO need a way to associate the icons with the input so that they can be set together
                prompt.SetLabel(promptInfo[i].m_LabelText);
            }
        }
    }

    private void Enable(bool enable)
    {
        // TODO refactor this to use an animator and animate in/out
        this.gameObject.SetActive(enable);
    }

    public void Show()
    {
        Enable(true);
    }

    public void Hide()
    {
        Enable(false);
    }
}