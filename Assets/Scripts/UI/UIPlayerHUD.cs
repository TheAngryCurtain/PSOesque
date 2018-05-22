using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHUD : MonoBehaviour
{
    [SerializeField] private int m_PlayerID;
    [SerializeField] private Text m_NameLabel;
    [SerializeField] private Text m_LevelLabel;

    [SerializeField] private Image m_HPRadialImage;
    [SerializeField] private Image m_MPRadialImage;
    [SerializeField] private Image m_XPRadialImage;

    [SerializeField] private float m_HPRadialMax = 1.0f;
    [SerializeField] private float m_MPRadialMax = 0.33f;
    [SerializeField] private float m_XPRadialMax = 0.33f;

    private void Start()
    {
		//VSEventManager.Instance.AddListener<GameEvents.UpdatePlayerEXPEvent>(OnEXPUpdated);
    }

    public void SetPlayerName(string name)
    {
        m_NameLabel.text = name;
    }

    public void SetPlayerLevel(int level)
    {
        m_LevelLabel.text = level.ToString();
    }

    public void SetHP(int current, int max)
    {
        SetDialFill(m_HPRadialImage, current, max, m_HPRadialMax);
    }

    public void SetMP(int current, int max)
    {
        SetDialFill(m_MPRadialImage, current, max, m_MPRadialMax);
    }

    public void SetXP(int current, int max)
    {
        SetDialFill(m_XPRadialImage, current, max, m_XPRadialMax);
    }

    private void SetDialFill(Image dial, int current, int max, float radialMax)
    {
        dial.fillAmount = ((float)current / (float)max) * radialMax;

        // TODO
        // cool delayed slow bar motion effect thing
    }
}
