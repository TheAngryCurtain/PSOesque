using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuItem : MonoBehaviour
{
    [SerializeField] private Image m_Frame;
    [SerializeField] private Text m_Label;

    private enum eStateColor { Default_Frame, Default_Text, Highlight_Frame, Highlight_Text };
    private Color[] m_StateColors;

    public void DefaultItem(Color dFrameColor, Color dTextColor, Color hFrameColor, Color hTextColor)
    {
        m_StateColors = new Color[] { dFrameColor, dTextColor, hFrameColor, hTextColor };

        m_Frame.color = m_StateColors[(int)eStateColor.Default_Frame];
        m_Label.color = m_StateColors[(int)eStateColor.Default_Text];
    }

    public void SetLabel(string text)
    {
        m_Label.text = text;
    }

    public void Highlight(bool highlight)
    {
        if (highlight)
        {
            m_Frame.color = m_StateColors[(int)eStateColor.Highlight_Frame];
            m_Label.color = m_StateColors[(int)eStateColor.Highlight_Text];
        }
        else
        {
            m_Frame.color = m_StateColors[(int)eStateColor.Default_Frame];
            m_Label.color = m_StateColors[(int)eStateColor.Default_Text];
        }
    }
}
