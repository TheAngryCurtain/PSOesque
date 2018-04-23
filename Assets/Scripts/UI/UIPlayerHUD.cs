using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHUD : MonoBehaviour
{
    [SerializeField] private Text m_NameLabel;
    [SerializeField] private Text m_LevelLabel;

    [SerializeField] private Image m_HPRadialImage;
    [SerializeField] private Image m_MPRadialImage;
    [SerializeField] private Image m_XPRadialImage;

    private float m_MaxRadialValue = 0.8f;

    public void SetPlayerData()
    {
        // TODO
        // should probably pass in Character Stats to populate this? or make a custom class for this

        // testing
        SetPlayerName("Test Name");
        SetPlayerLevel(114);

        SetHP(100f, 100f);
        SetMP(100f, 100f);
        SetXP(100f, 100f);
    }

    private void SetPlayerName(string name)
    {
        m_NameLabel.text = name;
    }

    private void SetPlayerLevel(int level)
    {
        m_LevelLabel.text = level.ToString();
    }

    private void SetHP(float current, float max)
    {
        SetDialFill(m_HPRadialImage, current, max);
    }

    private void SetMP(float current, float max)
    {
        SetDialFill(m_MPRadialImage, current, max);
    }

    private void SetXP(float current, float max)
    {
        SetDialFill(m_XPRadialImage, current, max);
    }

    private void SetDialFill(Image dial, float current, float max)
    {
        dial.fillAmount = (current / max) * m_MaxRadialValue;
    }
}
