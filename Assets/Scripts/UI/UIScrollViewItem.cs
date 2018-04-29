using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollViewItem : MonoBehaviour
{
    [SerializeField] private Text m_NameLabel;
    [SerializeField] private Image m_PlayerIcon;

    [SerializeField] private Color m_DefaultColor; // text
    [SerializeField] private Color m_SelectedColor; // text
    [SerializeField] private Color m_InactiveColor; // text

    public bool m_Disabled = false;

    public void SetData(string name, Sprite iconSprite, bool disabled)
    {
        m_NameLabel.text = name;

        if (iconSprite != null)
        {
            m_PlayerIcon.sprite = iconSprite;
        }

        m_Disabled = disabled;
        if (m_Disabled)
        {
            m_NameLabel.color = m_InactiveColor;
        }
    }

    public void SetSelected(bool selected)
    {
        if (m_Disabled) return;

        m_NameLabel.color = (selected ? m_SelectedColor : m_DefaultColor);
    }
}
