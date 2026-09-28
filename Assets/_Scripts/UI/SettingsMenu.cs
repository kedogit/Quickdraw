using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider m_sens;
    [SerializeField] private Slider m_masterVolume;
    [SerializeField] private Slider m_musicVolume;

    [SerializeField] private TextMeshProUGUI m_sensDisplay;
    [SerializeField] private TextMeshProUGUI m_masterVolumeDisplay;
    [SerializeField] private TextMeshProUGUI m_musicVolumeDisplay;

    private const float m_sensMinValue = 0f;
    private const float m_sensMaxValue = 100f;

    private const float m_volumeMinValue = 0f;
    private const float m_volumeMaxValue = 100f;

    // TODO!!!!
    // combine both sensitivity sliders into one, or add a lock button to keep the same value for both
    void Start()
    {
        float sliderValue = PlayerPrefs.GetFloat("Sensitivity");
        InitializeSlider(sliderValue, m_sensMinValue, m_sensMaxValue, m_sensDisplay, m_sens);

        sliderValue = PlayerPrefs.GetFloat("MasterVolume");
        InitializeSlider(sliderValue, m_volumeMinValue, m_volumeMaxValue, m_masterVolumeDisplay, m_masterVolume);

        sliderValue = PlayerPrefs.GetFloat("MusicVolume");
        InitializeSlider(sliderValue, m_volumeMinValue, m_volumeMaxValue, m_musicVolumeDisplay, m_musicVolume);

        m_sens.onValueChanged.AddListener(OnSensChanged);
        m_masterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
        m_musicVolume.onValueChanged.AddListener(OnMusicVolumeChanged);
    }

    private void InitializeSlider(float defaultValue, float minValue, float maxValue, TextMeshProUGUI label, Slider slider)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = defaultValue;
        UpdateSliderDisplay(defaultValue, label);
    }

    public void OnSensChanged(float sliderValue)
    {
        //float truncatedValue = MathF.Round(sliderValue, 2);
        float truncatedValue = MathF.Truncate(sliderValue);

        UpdatePlayerPrefValue("Sensitivity", truncatedValue);
        UpdateSliderDisplay(truncatedValue, m_sensDisplay);
    }

    public void OnMasterVolumeChanged(float sliderValue)
    {
        float truncatedValue = MathF.Truncate(sliderValue);

        UpdatePlayerPrefValue("MasterVolume", truncatedValue);
        UpdateSliderDisplay(truncatedValue, m_masterVolumeDisplay);

        AudioManager.GetInstance().AdjustMasterVolume(truncatedValue);
    }

    public void OnMusicVolumeChanged(float sliderValue)
    {
        float truncatedValue = MathF.Truncate(sliderValue);

        UpdatePlayerPrefValue("MusicVolume", truncatedValue);
        UpdateSliderDisplay(truncatedValue, m_musicVolumeDisplay);

        AudioManager.GetInstance().AdjustMusicVolume(truncatedValue);
    }

    private void UpdateSliderDisplay(float newValue, TextMeshProUGUI label)
    {
        label.text = newValue.ToString();
    }

    private void UpdatePlayerPrefValue(String playerPrefKey, float newValue)
    {
        PlayerPrefs.SetFloat(playerPrefKey, newValue);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();

        //if in-game
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Observer.GetInstance().TriggerEvent(EVENT.ON_SENS_CHANGE);
        }
    }
}
