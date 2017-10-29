using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeKeeper : MonoBehaviour
{
	private Light m_Light;
    private TimeSpan m_Time;

	private float m_CurrentTime = 0.25f;
	private float m_InitialLightIntensity;
	private float m_SecondsInDay = 86400f;
    private float m_IntensityModifier = 1f;
    private Enums.eTimeOfDay m_TimeOfDay = Enums.eTimeOfDay.Sunrise;
    //private float m_TimeMultiplier = 1f;

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
        UpdateTime();
	}

	private void UpdateLight()
	{
		m_Light.transform.localRotation = Quaternion.Euler((m_CurrentTime * 360f) - 90, 170, 0);
		m_Light.intensity = m_InitialLightIntensity * m_IntensityModifier;
	}

    private void UpdateTime()
    {
        // defines a full 24 hour in game day where 0 is midnight, 0.25 is sunrise, 0.5 is noon, 0.75 is sunset
        m_Time = DateTime.Now.TimeOfDay;

        float totalSeconds = (m_Time.Seconds + (m_Time.Minutes * 60f) + (m_Time.Hours * 60f * 60f));
        m_CurrentTime = (totalSeconds / m_SecondsInDay);// * m_TimeMultiplier;

        Enums.eTimeOfDay current = m_TimeOfDay;
        if (m_CurrentTime <= 0.23f || m_CurrentTime >= 0.75f)
        {
            // if the current time is sunset here, night has fallen!

            m_IntensityModifier = 0;
        }
        else if (m_CurrentTime <= 0.25f)
        {
            // if the current is night at this point, morning has come!

            m_IntensityModifier = Mathf.Clamp01((m_CurrentTime - 0.23f) * (1 / 0.02f));
            current = Enums.eTimeOfDay.Sunrise;
        }
        else if (m_CurrentTime >= 0.73f)
        {
            m_IntensityModifier = Mathf.Clamp01(1 - ((m_CurrentTime - 0.73f) * (1 / 0.02f)));
            current = Enums.eTimeOfDay.Sunset;
        }

        // notify if it's changed
        if (current != m_TimeOfDay)
        {
            m_TimeOfDay = current;
            VSEventManager.Instance.TriggerEvent(new GameEvents.TimeOfDayChangeEvent(m_TimeOfDay));
        }

        // wrap around
        if (m_CurrentTime >= 1)
        {
            m_CurrentTime = 0;
        }
    }
}
