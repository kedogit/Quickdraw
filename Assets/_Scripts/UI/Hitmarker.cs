using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hitmarker : MonoBehaviour
{
    [SerializeField] private GameObject m_hitMarkerImageGO;
    [SerializeField] private float m_hitmarkerDuration = 0.2f;
    [SerializeField] private Color m_regularHitColor;
    [SerializeField] private Color m_criticalHitColor;
    [SerializeField] private Color m_killColor;

    private Image m_hitmarkerImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Observer.GetInstance().SubscribeTo(EVENT.ON_ENEMY_HURT, ShowHitmarker);
        m_hitmarkerImage = m_hitMarkerImageGO?.GetComponent<Image>();
    }

    private void OnDestroy()
    {
        Observer.GetInstance().UnsubscribeTo(EVENT.ON_ENEMY_HURT, ShowHitmarker);
    }
    public void ShowHitmarker(Dictionary<string, object> eventParams)
    {
        string hitmarkerType = eventParams["HitmarkerType"] as string;

        if (hitmarkerType == "Critical")
        {
            m_hitmarkerImage.color = m_criticalHitColor;
        }
        else if (hitmarkerType == "Kill")
        {
            m_hitmarkerImage.color = m_killColor;
        }
        else
        {
            m_hitmarkerImage.color = m_regularHitColor;
        }

        m_hitMarkerImageGO.SetActive(true);
        StartCoroutine(EraseHitmarker());
        AudioManager.GetInstance().PlaySFX(SFX.HITMARKER);
    }

    private IEnumerator EraseHitmarker()
    {
        yield return new WaitForSeconds(m_hitmarkerDuration);
        m_hitMarkerImageGO.SetActive(false);
    }
}
