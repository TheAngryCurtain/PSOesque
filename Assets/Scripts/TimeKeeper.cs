using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeKeeper : MonoBehaviour
{
	private Light m_Light;

	private float m_CurrentTime = 0.25f;
	private float m_InitialLightIntensity;
	private float m_SecondsInDay = 120f;
	private float m_TimeMultiplier = 1f;

	private void Awake()
	{
		m_Light = GameObject.FindObjectOfType<Light>();
		if (m_Light != null)
		{
			m_InitialLightIntensity = m_Light.intensity;
		}
		else
		{
			Debug.LogError("No Light object found in scene");
		}
	}

	private void Update()
	{
		UpdateLight();

		// This value defines a full 24 hour in game day where 0 is midnight, 0,25 is sunrise, 0,5 is noon, 0,75 is sunset
		m_CurrentTime += (Time.deltaTime / m_SecondsInDay) * m_TimeMultiplier;

		if (m_CurrentTime >= 1) {
			m_CurrentTime = 0;
		}
	}

	private void UpdateLight()
	{
		m_Light.transform.localRotation = Quaternion.Euler((m_CurrentTime * 360f) - 90, 170, 0);

		float intensityMultiplier = 1;
		if (m_CurrentTime <= 0.23f || m_CurrentTime >= 0.75f)
		{
			intensityMultiplier = 0;
		}
		else if (m_CurrentTime <= 0.25f)
		{
			intensityMultiplier = Mathf.Clamp01((m_CurrentTime - 0.23f) * (1 / 0.02f));
		}
		else if (m_CurrentTime >= 0.73f)
		{
			intensityMultiplier = Mathf.Clamp01(1 - ((m_CurrentTime - 0.73f) * (1 / 0.02f)));
		}

		m_Light.intensity = m_InitialLightIntensity * intensityMultiplier;
	}
}
