using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;

public class CompanyScreen : UIBaseScreen
{
    [SerializeField] private float mLogoScreenTime = 2f;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnUIScreenAnimEvent(UIScreenAnimEvent animEvent)
    {
        base.OnUIScreenAnimEvent(animEvent);

        switch (animEvent)
        {
            case UIScreenAnimEvent.Start:
                break;

            case UIScreenAnimEvent.End:
                if (m_ActiveState == UIScreenAnimState.Intro)
                {
                    Invoke("TriggerOutro", mLogoScreenTime);
                }
                break;

            case UIScreenAnimEvent.None:
            default:
                break;
        }
    }

    private void TriggerOutro()
    {
        PlayScreenAnimation(UIScreenAnimState.Outro);
    }
}
