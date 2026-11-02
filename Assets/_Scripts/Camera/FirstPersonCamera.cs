using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private float m_dashZoomIntensity = 10f;
    [SerializeField] private float m_speedLineVelocityCutoff = 15f;
    [SerializeField] private float m_zoomDuration = 1f;
    [SerializeField] private ParticleSystem m_speedLines;

    private float m_elapsed;
    private CinemachineCamera m_cineComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_speedLines.Stop();
        m_cineComponent = GetComponent<CinemachineCamera>();
        Observer.GetInstance()?.SubscribeTo(EVENT.ON_PLAYER_DASH_START, DashTriggered);
    }

    private void OnDestroy()
    {
        Observer.GetInstance()?.UnsubscribeTo(EVENT.ON_PLAYER_DASH_START, DashTriggered);
    }

    private void DashTriggered()
    {
        //when dash is triggered, initiate the zoom effect
        float currentFOV = m_cineComponent.Lens.FieldOfView;
        float targetFOV = currentFOV + m_dashZoomIntensity;
        StartCoroutine(ZoomLerp(currentFOV, targetFOV));
    }

    private IEnumerator ZoomLerp(float startFOV, float endFOV)
    {
        //generate speed lines and prepare the zoom in
        m_speedLines.Play();
        float halvedDuration = m_zoomDuration / 2;

        //zoom in
        m_elapsed = 0f;
        while (m_elapsed <= halvedDuration)
        {
            m_elapsed += Time.deltaTime;
            float currentFOV = Mathf.Lerp(startFOV, endFOV, m_elapsed / (halvedDuration));
            m_cineComponent.Lens.FieldOfView = currentFOV;
            yield return null;
        }

        //zoom out
        m_elapsed = 0f;
        while (m_elapsed <= halvedDuration)
        {
            m_elapsed += Time.deltaTime;
            float currentFOV = Mathf.Lerp(endFOV, startFOV, m_elapsed / (halvedDuration));
            m_cineComponent.Lens.FieldOfView = currentFOV;
            yield return null;
        }

        //stop the speed lines when player's speed is below threshold
        Rigidbody playerRigidbody = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Rigidbody>();
        while (playerRigidbody.linearVelocity.magnitude > m_speedLineVelocityCutoff)
        {
            yield return null;
        }
        m_speedLines.Stop();
    }
}
