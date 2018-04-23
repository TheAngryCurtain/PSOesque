using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class HUDScreen : UIBaseScreen
{
    [SerializeField] private UIPlayerHUD m_PlayerHUD;

    public override void Initialize(object[] screenParams)
    {
        base.Initialize(screenParams);

        // test
        m_PlayerHUD.SetPlayerData();
    }
}
