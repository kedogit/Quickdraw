using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtBorder : MonoBehaviour
{
    [SerializeField] private float m_borderDuration = 1f;
    [SerializeField] private float m_criticalHPThreshold = 50f;

    private Image m_imageComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get image component
        m_imageComponent = GetComponent<Image>();

        //make border invisible
        Color noAlpha = m_imageComponent.color;
        noAlpha.a = 0f;
        m_imageComponent.color = noAlpha;

        //tell the observer to show the border on player damage taken
        Observer.GetInstance()?.SubscribeTo(EVENT.ON_PLAYER_HURT, ShowBorder);
        Observer.GetInstance()?.SubscribeTo(EVENT.ON_PLAYER_EXIT_CRITICAL, HideBorder);
    }

    private void OnDestroy()
    {
        Observer.GetInstance()?.UnsubscribeTo(EVENT.ON_PLAYER_HURT, ShowBorder);
        Observer.GetInstance()?.UnsubscribeTo(EVENT.ON_PLAYER_EXIT_CRITICAL, HideBorder);
    }

    private void ShowBorder(Dictionary<string, object> eventParams)
    {
        float currentHPPercent = (float)eventParams["HPPercent"];

        //create a color with full alpha and start the coroutine with the start and end colors (same rgb, different alphas)
        Color fullAlpha = m_imageComponent.color;
        fullAlpha.a = 1f;
        m_imageComponent.color = fullAlpha;

        if (currentHPPercent > m_criticalHPThreshold)
        {
            HideBorder();
        }
    }

    private void HideBorder()
    {
        StartCoroutine(FadeBorder());
    }

    private IEnumerator FadeBorder()
    {
        Color startColor = m_imageComponent.color;
        Color endColor = m_imageComponent.color;
        endColor.a = 0f;


        float elapsed = 0f;
        while (elapsed < m_borderDuration)
        {
            m_imageComponent.color = Color.Lerp(startColor, endColor, elapsed / m_borderDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
