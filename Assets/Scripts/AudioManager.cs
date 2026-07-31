using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SONG
{
    OVERWORLD1,
    BATTLE1,
    MENU1
}

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioMixer m_mixer;
    [SerializeField] private AudioMixerGroup m_musicMixerGroup;

    [Header("Songs")]
    [SerializeField] private AudioClip m_overworldSong1;
    [SerializeField] private AudioClip m_battleSong1;
    [SerializeField] private AudioClip m_menuSong1;

    [Header("Settings")]
    [SerializeField] private float m_songCrossfadeDuration = 2f;

    private Dictionary<SONG, AudioClip> m_songsDictionary;
    private static AudioManager m_instance;
    private AudioSource m_persistentMusicSource;
    private AudioSource m_situationalMusicSource;

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeDictionary();
            m_persistentMusicSource = InitializeMusic("PersistentMusic");
            m_situationalMusicSource = InitializeMusic("SituationalMusic");
            m_situationalMusicSource.clip = m_songsDictionary[SONG.BATTLE1];
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        AdjustMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        AdjustMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        Observer.GetInstance().SubscribeTo(EVENT.ON_PLAYER_DEATH, FadeToPersistentBGM);
    }

    private void InitializeDictionary()
    {
        m_songsDictionary = new Dictionary<SONG, AudioClip>();
        m_songsDictionary.Add(SONG.OVERWORLD1, m_overworldSong1);
        m_songsDictionary.Add(SONG.BATTLE1, m_battleSong1);
        m_songsDictionary.Add(SONG.MENU1, m_menuSong1);
    }

    private AudioSource InitializeMusic(string name)
    {
        //creates a music gameobject
        GameObject musicGameObject = new GameObject(name);
        musicGameObject.transform.parent = this.transform;

        //add the audio component and set basic settings
        AudioSource audioComponent = musicGameObject.AddComponent<AudioSource>();
        audioComponent.loop = true;
        audioComponent.outputAudioMixerGroup = m_musicMixerGroup;

        return audioComponent;
    }

    public void ChangePersistentBGM(SONG song)
    {
        if (m_situationalMusicSource.isPlaying)
        {
            FadeToPersistentBGM();
        }

        m_persistentMusicSource.clip = m_songsDictionary[song];
        m_persistentMusicSource.Play();
    }

    public static AudioManager GetInstance()
    {
        return m_instance;
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

    private IEnumerator FadeBetweenBGM(AudioSource originalSource, AudioSource newSource)
    {
        //set the new source's volume to 0 (failsafe) and unpause it
        newSource.volume = 0f;
        newSource.Play();

        float elapsed = 0f;
        float step = 0f;

        //crossfade the songs, reducing one's volume while equally raising the other's
        while (elapsed < m_songCrossfadeDuration)
        {
            elapsed += Time.deltaTime;
            step = elapsed / m_songCrossfadeDuration;

            originalSource.volume = Mathf.Lerp(1f, 0f, step);
            newSource.volume = Mathf.Lerp(0f, 1f, step);
            yield return null;
        }

        //pause the original source once the transition is done
        originalSource.Pause();
    }

    public void FadeToBattleBGM()
    {
        if (!m_situationalMusicSource.isPlaying)
        {
            StartCoroutine(FadeBetweenBGM(m_persistentMusicSource, m_situationalMusicSource));
        }
    }

    public void FadeToPersistentBGM()
    {
        if (!m_persistentMusicSource.isPlaying)
        {
            StartCoroutine(FadeBetweenBGM(m_situationalMusicSource, m_persistentMusicSource));
        }
    }
}
