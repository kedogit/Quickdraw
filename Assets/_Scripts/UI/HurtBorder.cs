using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HurtBorder : MonoBehaviour
{
    [SerializeField] private float m_borderDuration = 1f;

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
    }

    private void OnDestroy()
    {
        Observer.GetInstance()?.UnsubscribeTo(EVENT.ON_PLAYER_HURT, ShowBorder);
    }

    private void ShowBorder()
    {
        //create a color with full alpha and start the coroutine with the start and end colors (same rgb, different alphas)
        Color fullAlpha = m_imageComponent.color;
        fullAlpha.a = 1f;
        StartCoroutine(FadeBorder(fullAlpha, m_imageComponent.color));
    }

    private IEnumerator FadeBorder(Color startColor, Color endColor)
    {
        float elapsed = 0f;
        while (elapsed < m_borderDuration)
        {
            m_imageComponent.color = Color.Lerp(startColor, endColor, elapsed / m_borderDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
