using Cinemachine;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Device;

public class DayNightScript : MonoBehaviour
{
    private float dayDuration = 2000.0f;
    private float dayTime;
    private float rotationAngle;
    private float dawnTime  = 4.0f;
    private float noonTime  = 7.0f;
    private float duskTime  = 17.0f;
    private float nightTime = 20.0f;
    private Light sun;
    private Light moon;
    private float maxSkyboxExposure = 1.3f; // From Default Skybox
    private float minAmbientLight = 0.1f;
    private string[] listenableEvents = { nameof(GameState) };
    private bool isDayPrev;

    private bool isDay => dayTime >= dawnTime && dayTime < nightTime;

    void Start()
    {
        rotationAngle = -360.0f / dayDuration;
        dayTime = 12.0f - this.transform.eulerAngles.z / 360f * 24.0f;
        while (dayTime >= 24)
        {
            dayTime -= 24f;
        }
        while (dayTime < 0)
        {
            dayTime += 24f;
        }
        sun = transform.Find("Sun").GetComponent<Light>();
        moon = transform.Find("Moon").GetComponent<Light>();
        GameState.gameTime24 = dayTime;
        GameState.isDay = isDay;
        GameEventSystem.AddListener(OnGameEvent, listenableEvents);
    }

    void Update()
    {
        bool isDayPrev = isDay;
        dayTime += 24f * Time.deltaTime / dayDuration;
        if (dayTime >= 24)
        {
            dayTime -= 24f;
        }
        GameState.gameTime24 = dayTime;
        GameState.isDay = isDay;
        if (isDayPrev != isDay) // day -> night || night -> day
        {
            OnDayNightChanged();
        }

        float coef;
        if (dayTime >= dawnTime && dayTime < nightTime)
        {
            coef = Mathf.Sin((dayTime - dawnTime) * Mathf.PI / (nightTime - dawnTime));
            sun.intensity = coef;
            moon.intensity = 0f;
            if (RenderSettings.sun != sun)
            {
                RenderSettings.sun = sun;
                moon.intensity = 0f;
            }
        }
        else
        {
            float arg = dayTime < dawnTime ? dayTime : dayTime - 24.0f;
            coef = 0.3f * Mathf.Cos(arg * Mathf.PI / (dawnTime - (-dawnTime)));
            sun.intensity = 0f;
            moon.intensity = coef;
            if (RenderSettings.sun != moon)
            {
                RenderSettings.sun = moon;
                sun.intensity = 0f;
            }
        }

        RenderSettings.ambientIntensity = Mathf.Clamp(coef, minAmbientLight, 1.0f);
        RenderSettings.skybox.SetFloat("_Exposure", coef * maxSkyboxExposure);

        this.transform.Rotate(0, 0, rotationAngle * Time.deltaTime);
    }

    private void OnDayNightChanged()
    {
        if (isDay && GameState.daySkybox != null)
        {
            RenderSettings.skybox = GameState.daySkybox;
        }
        if (!isDay && GameState.nightSkybox != null)
        {
            RenderSettings.skybox = GameState.nightSkybox;
        }
    }

    private void OnGameEvent(string type, object payload)
    {
        if (nameof(GameState.daySkybox).Equals(payload) && isDay)
        {
            RenderSettings.skybox = GameState.daySkybox;
        }
        else if (nameof(GameState.nightSkybox).Equals(payload) && !isDay)
        {
            RenderSettings.skybox = GameState.nightSkybox;
        }
    }

    private void OnDestroy()
    {
        GameEventSystem.RemoveListener(OnGameEvent, listenableEvents);
    }
}
