using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Enums;
using Rewired;

public class LoadingScreen : UIBaseScreen
{
	[SerializeField] private RectTransform m_LoadingIcon;

	private Vector3 m_Movement = Vector3.zero;

	protected override void OnInputUpdate(InputActionEventData data)
	{
		if (InputLocked()) return;

		bool handled = false;
		float value;

		switch (data.actionId)
		{
		case RewiredConsts.Action.Navigate_Horizontal:
			value = data.GetAxis();
			if (value != 0f)
			{
				m_Movement.x = value;
			}
			break;

		case RewiredConsts.Action.Navigate_Vertical:
			value = data.GetAxis();
			if (value != 0f)
			{
				m_Movement.y = value;
			}
			break;
		}

		if (m_Movement != Vector3.zero)
		{
			m_LoadingIcon.Translate(m_Movement);
		}

		// pass to base
		if (!handled)
		{
			base.OnInputUpdate(data);
		}
	}
}
