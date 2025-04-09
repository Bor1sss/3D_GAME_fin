using UnityEngine;

public class ForestSoundsScript : MonoBehaviour
{
    private AudioSource daySound;
    private AudioSource nightSound;
    private string[] listenableEvents = { nameof(GameState) };

    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        daySound = audioSources[0];
        nightSound = audioSources[1];
        SwitchSounds();
        GameEventSystem.AddListener(OnGameEvent, listenableEvents);
    }

    private void SwitchSounds()
    {
        if (GameState.isDay)
        {
            if (nightSound.isPlaying) 
            {
                nightSound.Stop();
            }
            if (!daySound.isPlaying) 
            {
                daySound.Play();
            }
        }
        else
        {
            if (!nightSound.isPlaying)
            {
                nightSound.Play();
            }
            if (daySound.isPlaying)
            {
                daySound.Stop();
            }
        }
    }

    private void OnGameEvent(string type, object payload)
    {
        if (nameof(GameState).Equals(type) && nameof(GameState.isDay).Equals(payload))
        {
            SwitchSounds();
        }
    }

    private void OnDestroy()
    {
        GameEventSystem.RemoveListener(OnGameEvent, listenableEvents);
    }
}
