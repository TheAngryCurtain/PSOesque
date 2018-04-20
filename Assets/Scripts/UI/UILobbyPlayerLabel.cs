using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyPlayerLabel : MonoBehaviour
{
    [SerializeField] private Text m_Label;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Init(PlayerLobbyData data)
    {
        m_Label.text = data.m_PlayerName;

        // TODO this could be better
        gameObject.SetActive(true);
    }
}
