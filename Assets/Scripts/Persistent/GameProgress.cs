using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameProgress
{
    // TODO
    // store world seed
    // store things like available themes/levels/depths/etc?
    // store completed quests?
    // store any companion/party/npc stats here?

    public HubProgress m_HubProgress;

    public GameProgress() { }

    public void Init()
    {
        m_HubProgress = new HubProgress();
    }
}
