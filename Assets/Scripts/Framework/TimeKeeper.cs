using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeKeeper : MonoBehaviour
{
    [SerializeField] private float m_TimeMultiplier = 1f;

    private Light m_Light;
    private TimeSpan m_Time;

	private float m_CurrentTime = 0f;
	private float m_InitialLightIntensity;
	private float m_SecondsInDay = 86400f;
    private float m_LightIntensity = 1f;
    private float m_AmbientIntensity = 1f;
    private Enums.eTimeOfDay m_TimeOfDay = Enums.eTimeOfDay.Sunrise;

    public static float Midnight = 0f;
    public static float PreSunrise = 0.23f;
    public static float Sunrise = 0.25f;
    public static float Noon = 0.5f;
    public static float PreSunset = 0.85f;
    public static float Sunset = 0.87f;

    private Material m_SkyboxMaterial;

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

        m_SkyboxMaterial = RenderSettings.skybox;

        // init
        m_SkyboxMaterial.SetFloat("_Blend2", 1f);
        m_SkyboxMaterial.SetFloat("_Blend3", 1f);
    }

    private void Update()
	{
		UpdateLight();
        UpdateTime();
	}

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 200), "Current Time: " + m_CurrentTime.ToString());
    }
#endif

    private void UpdateLight()
	{
		m_Light.transform.localRotation = Quaternion.Euler((m_CurrentTime * 360f) - 90, 170, 0);
		m_Light.intensity = m_InitialLightIntensity * m_LightIntensity;
	}

    private void UpdateTime()
    {
        // currentTime is in the range [0,1], so 0.25f increments represent 6 hours
        // 0 (0am) is midnight, 0.25 (6am) is sunrise, 0.5(12pm) is noon, 0.87(9pm) is sunset
        m_CurrentTime += (Time.deltaTime / m_SecondsInDay) * m_TimeMultiplier;

        Enums.eTimeOfDay current = m_TimeOfDay;
        if (m_CurrentTime <= PreSunrise || m_CurrentTime >= Sunset)
        {

            // if the current time is sunset here, night has fallen!

            m_LightIntensity = 0;
        }
        else if (m_CurrentTime <= Sunrise)
        {
            // if the current is night at this point, morning has come!

            float increment = Mathf.Clamp01((m_CurrentTime - PreSunrise) * (1 / 0.02f));
            m_LightIntensity = increment;
            current = Enums.eTimeOfDay.Sunrise;

            m_SkyboxMaterial.SetFloat("_Blend2", 1f - increment);
            m_SkyboxMaterial.SetFloat("_Blend3", 1f - increment);
        }
        else if (m_CurrentTime >= PreSunset)
        {
            float increment = Mathf.Clamp01(1 - ((m_CurrentTime - PreSunset) * (1 / 0.02f)));
            m_LightIntensity = increment;
            current = Enums.eTimeOfDay.Sunset;

            m_SkyboxMaterial.SetFloat("_Blend2", 1f - increment);
            m_SkyboxMaterial.SetFloat("_Blend3", 1f - increment);
        }

        m_AmbientIntensity = m_LightIntensity;
        RenderSettings.ambientIntensity = m_AmbientIntensity;

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
