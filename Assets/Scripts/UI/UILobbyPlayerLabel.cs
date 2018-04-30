using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyPlayerLabel : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Text m_Label;

    public void Init(PlayerLobbyData data)
    {
        m_Label.text = data.m_PlayerName;

        this.gameObject.SetActive(true);
    }

    public void AnimateShow(bool transitionIn)
    {
        this.gameObject.SetActive(true);
        string trigger = (transitionIn ? "Intro" : "Outro");
        m_Animator.SetTrigger(trigger);
    }
}
