using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherFactory : MonoBehaviour
{
    private enum eWeatherCondition { Rain, Snow, Hail, Sand };
    private enum eWeatherSeverity { Light, Normal, Heavy }
    private enum eSkyCondition { Clear, Cloudy, Overcast }

    [SerializeField] private GameObject[] m_WeatherPrefabs;

    private void Start()
    {
        VSEventManager.Instance.AddListener<GameEvents.TimeOfDayChangeEvent>(OnTimeChanged);
    }

    private void OnTimeChanged(GameEvents.TimeOfDayChangeEvent e)
    {
        // TODO
        // BOTW-style weather where it predicts the weather a few hours at a time and display it on the UI
        // then it changes and it will have effects on things like enemy spawns, player stamina(?), or something
        // should be dependant on current planet/level the player is on (set of weather parmas generated for the level, like likelihood of precipitation, length of days(!!), day/night temps (temperature stuff?!), etc)
    }
}
