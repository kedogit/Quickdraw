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

public enum SFX
{
    HITMARKER,
    PLAYER_DRAW_BOW,
    PLAYER_HURT,
    PLAYER_SHOOT,
    PLAYER_JUMP,
    PLAYER_DASH,
    GRAPPLING_HOOK_THROW,
    GRAPPLING_HOOK_LAND,
    GOBLIN_ATTACK1,
    GOBLIN_ATTACK2,
    GOBLIN_DRAW_BOW,
    GOBLIN_HURT,
    GOBLIN_DEATH,
    ORC_ATTACK1,
    MENU_HOVER,
    MENU_CONFIRM1,
    MENU_CONFIRM2
}

public enum ENEMY
{
    GOBLIN,
    ORC
}

public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_globalSFXVolume = 0.5f;
    [SerializeField] private float m_songCrossfadeDuration = 2f;
    [SerializeField] private float m_spatialAudioMinDistance = 5f;
    [SerializeField] private float m_spatialAudioMaxDistance = 20f;

    [Header("References")]
    [SerializeField] private AudioMixer m_mixer;
    [SerializeField] private AudioMixerGroup m_musicMixerGroup;
    [SerializeField] private AudioMixerGroup m_masterMixerGroup;

    [Header("Songs")]
    [SerializeField] private AudioClip m_overworldSong1;
    [SerializeField] private AudioClip m_battleSong1;
    [SerializeField] private AudioClip m_menuSong1;

    [Header("SFX")]
    [SerializeField] private List<AudioClip> m_SFX;
    [SerializeField] private List<AudioClip> m_goblinFootsteps;
    [SerializeField] private List<AudioClip> m_orcFootsteps;

    private Dictionary<SONG, AudioClip> m_songsDictionary;
    private Dictionary<SFX, AudioClip> m_sfxDictionary;
    private Dictionary<ENEMY, List<AudioClip>> m_footstepsDictionary;

    private static AudioManager m_instance;

    private List<AudioSource> m_audioSources;

    private AudioSource m_persistentMusicSource;
    private AudioSource m_situationalMusicSource;

    private const float m_defaultVolumeValue = 85f;

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeMusicDictionary();
            InitializeSFXDictionary();
            InitializeFootstepsDictionary();
            m_audioSources = new List<AudioSource>();
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
        AdjustMasterVolume(PlayerPrefs.GetFloat("MasterVolume", m_defaultVolumeValue));
        AdjustMusicVolume(PlayerPrefs.GetFloat("MusicVolume", m_defaultVolumeValue));
        Observer.GetInstance().SubscribeTo(EVENT.ON_PLAYER_DEATH, FadeToPersistentBGM);
    }

    public static AudioManager GetInstance()
    {
        return m_instance;
    }

    private void InitializeFootstepsDictionary()
    {
        m_footstepsDictionary = new Dictionary<ENEMY, List<AudioClip>>();
        m_footstepsDictionary.Add(ENEMY.GOBLIN, m_goblinFootsteps);
        m_footstepsDictionary.Add(ENEMY.ORC, m_orcFootsteps);
    }

    public void PlayFootstep(ENEMY enemyType, Vector3 position)
    {
        AudioSource audioSource = GetSource();
        audioSource.clip = m_footstepsDictionary[enemyType].GetRandomItem();

        //sets the spatial audio parameters
        audioSource.spatialBlend = 1;
        audioSource.minDistance = m_spatialAudioMinDistance;
        audioSource.maxDistance = m_spatialAudioMaxDistance;
        audioSource.gameObject.transform.position = position;

        audioSource.Play();
    }

    private void InitializeSFXDictionary()
    {
        m_sfxDictionary = new Dictionary<SFX, AudioClip>();

        //important: make sure the serialized list of audio clips matches the enum both in order and size so this loop can work seamlessly
        int index = 0;
        foreach (SFX soundEffect in System.Enum.GetValues(typeof(SFX))) 
        {
            m_sfxDictionary.Add(soundEffect, m_SFX[index]);
            index++;
        }
    }

    private AudioSource GetSource()
    {
        foreach (AudioSource source in m_audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        //creates a new game object
        GameObject newGameObject = new GameObject("Audio Source");
        newGameObject.transform.parent = this.transform;

        //adds an audio component and adds it to the list
        AudioSource newAudioSource = newGameObject.AddComponent<AudioSource>();
        newAudioSource.outputAudioMixerGroup = m_masterMixerGroup;
        newAudioSource.volume = m_globalSFXVolume;
        m_audioSources.Add(newAudioSource);

        return newAudioSource;
    }

    public void PlaySFX(SFX soundEffect)
    {
        AudioSource audioSource = GetSource();
        audioSource.clip = m_sfxDictionary[soundEffect];
        audioSource.spatialBlend = 0;
        audioSource.Play();
    }

    public void PlaySFX(AudioClip soundEffect)
    {
        AudioSource audioSource = GetSource();
        audioSource.clip = soundEffect;
        audioSource.spatialBlend = 0;
        audioSource.Play();
    }

    public void PlaySFX(SFX soundEffect, Vector3 position)
    {
        AudioSource audioSource = GetSource();
        audioSource.clip = m_sfxDictionary[soundEffect];

        //sets the spatial audio parameters
        audioSource.spatialBlend = 1;
        audioSource.minDistance = m_spatialAudioMinDistance;
        audioSource.maxDistance = m_spatialAudioMaxDistance;
        audioSource.gameObject.transform.position = position;

        audioSource.Play();
    }

    #region MUSIC
    private void InitializeMusicDictionary()
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
        if (percentage == 0)
        {
            return -80;
        }
        return Mathf.Log10((percentage / 100)) * 20;
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
    #endregion
}
