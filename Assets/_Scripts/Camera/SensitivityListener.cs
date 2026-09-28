using Unity.Cinemachine;
using UnityEngine;

public class SensitivityListener : MonoBehaviour
{
    private CinemachineInputAxisController m_inputController;
    private const float m_defaultSens = 50f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_inputController = GetComponent<CinemachineInputAxisController>();
        AdjustSens();

        Observer.GetInstance().SubscribeTo(EVENT.ON_SENS_CHANGE, AdjustSens);
    }

    private void OnDestroy()
    {
        Observer.GetInstance().UnsubscribeTo(EVENT.ON_SENS_CHANGE, AdjustSens);
    }

    private void AdjustSens()
    {
        float sens = PlayerPrefs.GetFloat("Sensitivity", m_defaultSens);
        m_inputController.Controllers[0].Input.Gain = sens;
        m_inputController.Controllers[1].Input.Gain = -sens;
    }
}
