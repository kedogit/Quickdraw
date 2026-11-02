using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : Singleton<TransitionManager>
{
    [SerializeField] private Image m_blackScreen;
    [SerializeField] private float m_crossfadeDuration = 1f;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(Crossfade(sceneName));
    }

    private IEnumerator Crossfade(string sceneName)
    {
        m_blackScreen.enabled = true;

        //fetch colors to fade between
        Color solidBlackScreen = Color.black;
        Color alphaBlackScreen = solidBlackScreen;
        alphaBlackScreen.a = 0f;

        float elapsed = 0f;

        //slowly turn screen black
        while (m_blackScreen.color.a < 1f)
        {
            Color newColor = Color.Lerp(alphaBlackScreen, solidBlackScreen, elapsed / (m_crossfadeDuration / 2));
            m_blackScreen.color = newColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        //change scene when screen is fully black
        SceneManager.LoadScene(sceneName);

        elapsed = 0f;

        //slowly turn off the black screen
        while (m_blackScreen.color.a > 0f)
        {
            Color newColor = Color.Lerp(solidBlackScreen, alphaBlackScreen, elapsed / (m_crossfadeDuration / 2));
            m_blackScreen.color = newColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        m_blackScreen.enabled = false;
    }

    private IEnumerator FadeIn()
    {
        m_blackScreen.enabled = true;

        //fetch colors to fade between
        Color solidBlackScreen = Color.black;
        Color alphaBlackScreen = solidBlackScreen;
        alphaBlackScreen.a = 0f;

        float elapsed = 0f;

        //slowly turn off the black screen
        while (m_blackScreen.color.a > 0f)
        {
            Color newColor = Color.Lerp(solidBlackScreen, alphaBlackScreen, elapsed / m_crossfadeDuration);
            m_blackScreen.color = newColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        m_blackScreen.enabled = false;
    }
}
