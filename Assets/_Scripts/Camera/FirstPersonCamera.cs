using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private float m_dashZoomIntensity = 10f;
    [SerializeField] private float m_zoomDuration = 1f;

    private float m_elapsed;
    private CinemachineCamera m_cineComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_cineComponent = GetComponent<CinemachineCamera>();
        Observer.GetInstance()?.SubscribeTo(EVENT.ON_PLAYER_DASH_START, DashTriggered);
    }

    private void DashTriggered()
    {
        float currentFOV = m_cineComponent.Lens.FieldOfView;
        float targetFOV = currentFOV + m_dashZoomIntensity;
        StartCoroutine(ZoomLerp(currentFOV, targetFOV));
    }

    private IEnumerator ZoomLerp(float startFOV, float endFOV)
    {
        float halvedDuration = m_zoomDuration / 2;
        m_elapsed = 0f;
        while (m_elapsed <= halvedDuration)
        {
            m_elapsed += Time.deltaTime;
            float currentFOV = Mathf.Lerp(startFOV, endFOV, m_elapsed / (halvedDuration));
            m_cineComponent.Lens.FieldOfView = currentFOV;
            yield return null;
        }

        m_elapsed = 0f;
        while (m_elapsed <= halvedDuration)
        {
            m_elapsed += Time.deltaTime;
            float currentFOV = Mathf.Lerp(endFOV, startFOV, m_elapsed / (halvedDuration));
            m_cineComponent.Lens.FieldOfView = currentFOV;
            yield return null;
        }
    }
}
