using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollViewItem : MonoBehaviour
{
    [SerializeField] private Text m_NameLabel;
    [SerializeField] private Image m_PlayerIcon;

    [SerializeField] private Color m_DefaultColor;
    [SerializeField] private Color m_SelectedColor;

    public void SetData(string name, Sprite iconSprite)
    {
        m_NameLabel.text = name;

        if (iconSprite != null)
        {
            m_PlayerIcon.sprite = iconSprite;
        }
    }

    public void SetSelected(bool selected)
    {
        m_NameLabel.color = (selected ? m_SelectedColor : m_DefaultColor);
    }
}
