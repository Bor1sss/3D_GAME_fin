using UnityEngine;
using UnityEngine.UI;

public class StaminaScript : MonoBehaviour
{
    private Image indicator;
    private GameObject content;

    void Start()
    {
        content = transform.Find("Content").gameObject;
        indicator = transform.Find("Content/Indicator").GetComponent<Image>();
        GameEventSystem.AddListener(OnGameStateChangedEvent, nameof(GameState));
        OnGameStateChangedEvent(nameof(GameState), null);
    }

    void Update()
    {
        indicator.fillAmount = Mathf.Clamp01(GameState.stamina / GameState.staminaLimit);
        indicator.color = new Color(
                0.15f + (1.0f - indicator.fillAmount) * 0.62f,
                0.15f + indicator.fillAmount * 0.62f,
                0.1f
            );
    }

    private void OnGameStateChangedEvent(string type, object payload)
    {
        // Sync with clock
        if (payload == null || nameof(GameState.isClockVisible).Equals(payload))
        {
            content.SetActive(GameState.isClockVisible);
        }
    }

    private void OnDestroy()
    {
        GameEventSystem.RemoveListener(OnGameStateChangedEvent, nameof(GameState));
    }
}
