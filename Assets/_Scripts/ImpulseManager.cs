using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public enum CAMERA_EFFECT
{
    BOW_RECOIL,
    GROUND_QUAKE,
    DAMAGE_TAKEN
}

public class ImpulseManager : Singleton<ImpulseManager>
{
    [SerializeField] CinemachineImpulseSource m_bowRecoilImpulse;
    [SerializeField] CinemachineImpulseSource m_groundQuakeImpulse;
    [SerializeField] CinemachineImpulseSource m_damageTakenImpulse;

    private bool m_shakeEnabled = false;

    private void Start()
    {
        ToggleShake();
        Observer.GetInstance().SubscribeTo(EVENT.ON_SHAKE_TOGGLE, ToggleShake);
    }


    public void PlayImpulse(CAMERA_EFFECT impulse)
    {
        if (m_shakeEnabled)
        {
            switch (impulse)
            {
                case CAMERA_EFFECT.BOW_RECOIL:
                    m_bowRecoilImpulse.GenerateImpulse();
                    break;
                case CAMERA_EFFECT.GROUND_QUAKE:
                    m_groundQuakeImpulse.GenerateImpulse();
                    break;
                case CAMERA_EFFECT.DAMAGE_TAKEN:
                    m_damageTakenImpulse.GenerateImpulse();
                    break;
            }
        }
    }

    private void ToggleShake()
    {
        m_shakeEnabled = PlayerPrefs.GetInt("CameraShake", 1) == 1 ? true : false;
    }
}
