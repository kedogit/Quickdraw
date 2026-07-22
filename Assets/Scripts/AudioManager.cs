using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer m_mixer;
    private static AudioManager m_instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static AudioManager GetInstance()
    {
        return m_instance;
    }

    private void Start()
    {
        AdjustMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        AdjustMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
    }

    public void AdjustMasterVolume(float masterVolumeValue)
    {
        m_mixer.SetFloat("MasterVolume", LerpToScale(masterVolumeValue));
    }

    public void AdjustMusicVolume(float musicVolumeValue)
    {
        m_mixer.SetFloat("MusicVolume", LerpToScale(musicVolumeValue));
    }

    private float LerpToScale(float percentage)
    {
        return Mathf.Lerp(-80, 0, percentage / 100);
    }
}
