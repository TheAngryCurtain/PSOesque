using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Enums;

public class UIAnimatorRelay : MonoBehaviour
{
    public System.Action<UIScreenAnimEvent> OnAnimationEvent;

    public void AnimatorEvent(UIScreenAnimEvent animEvent)
    {
        if (OnAnimationEvent != null)
        {
            OnAnimationEvent(animEvent);
        }
    }
}
