using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider m_horSens;
    [SerializeField] private Slider m_vertSens;
    [SerializeField] private Slider m_masterVolume;
    [SerializeField] private Slider m_musicVolume;

    [SerializeField] private TextMeshProUGUI m_horSensDisplay;
    [SerializeField] private TextMeshProUGUI m_vertSensDisplay;
    [SerializeField] private TextMeshProUGUI m_masterVolumeDisplay;
    [SerializeField] private TextMeshProUGUI m_musicVolumeDisplay;

    private const float m_sensMinValue = 0f;
    private const float m_sensMaxValue = 1f;

    private const float m_volumeMinValue = 0f;
    private const float m_volumeMaxValue = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float sliderValue = PlayerPrefs.GetFloat("HorizontalSens");
        InitializeSlider(sliderValue, m_sensMinValue, m_sensMaxValue, m_horSensDisplay, m_horSens);

        sliderValue = PlayerPrefs.GetFloat("VerticalSens");
        InitializeSlider(sliderValue, m_sensMinValue, m_sensMaxValue, m_vertSensDisplay, m_vertSens);

        sliderValue = PlayerPrefs.GetFloat("MasterVolume");
        InitializeSlider(sliderValue, m_volumeMinValue, m_volumeMaxValue, m_masterVolumeDisplay, m_masterVolume);

        sliderValue = PlayerPrefs.GetFloat("MusicVolume");
        InitializeSlider(sliderValue, m_volumeMinValue, m_volumeMaxValue, m_musicVolumeDisplay, m_musicVolume);

        m_horSens.onValueChanged.AddListener(OnHorSensChanged);
        m_vertSens.onValueChanged.AddListener(OnVertSensChanged);
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

    public void OnHorSensChanged(float sliderValue)
    {
        float truncatedValue = MathF.Round(sliderValue, 2);

        UpdatePlayerPrefValue("HorizontalSens", truncatedValue);
        UpdateSliderDisplay(truncatedValue, m_horSensDisplay);
    }

    public void OnVertSensChanged(float sliderValue)
    {
        float truncatedValue = MathF.Round(sliderValue, 2);

        UpdatePlayerPrefValue("VerticalSens", truncatedValue);
        UpdateSliderDisplay(truncatedValue, m_vertSensDisplay);
    }

    public void OnMasterVolumeChanged(float sliderValue)
    {
        float truncatedValue = MathF.Truncate(sliderValue);

        UpdatePlayerPrefValue("MasterVolume", truncatedValue);
        UpdateSliderDisplay(truncatedValue, m_masterVolumeDisplay);
    }

    public void OnMusicVolumeChanged(float sliderValue)
    {
        float truncatedValue = MathF.Truncate(sliderValue);

        UpdatePlayerPrefValue("MusicVolume", truncatedValue);
        UpdateSliderDisplay(truncatedValue, m_musicVolumeDisplay);
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
    }
}
