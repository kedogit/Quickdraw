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
    ORC_ATTACK1
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
    [SerializeField] private AudioClip m_hitmarkerSFX;
    [SerializeField] private AudioClip m_playerHurtSFX;
    [SerializeField] private AudioClip m_playerDrawBowSFX;
    [SerializeField] private AudioClip m_playerShootSFX;
    [SerializeField] private AudioClip m_playerJumpSFX;
    [SerializeField] private AudioClip m_playerDashSFX;
    [SerializeField] private AudioClip m_grapplingHookThrowSFX;
    [SerializeField] private AudioClip m_grapplingHookLandSFX;
    [SerializeField] private AudioClip m_goblinAttack1SFX;
    [SerializeField] private AudioClip m_goblinAttack2SFX;
    [SerializeField] private AudioClip m_goblinDrawBowSFX;
    [SerializeField] private AudioClip m_goblinHurtSFX;
    [SerializeField] private AudioClip m_goblinDeathSFX;
    [SerializeField] private AudioClip m_orcAttack1SFX;

    private Dictionary<SONG, AudioClip> m_songsDictionary;
    private Dictionary<SFX, AudioClip> m_sfxDictionary;

    private static AudioManager m_instance;

    private List<AudioSource> m_audioSources;

    private AudioSource m_persistentMusicSource;
    private AudioSource m_situationalMusicSource;

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeMusicDictionary();
            InitializeSFXDictionary();
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
        AdjustMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        AdjustMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        Observer.GetInstance().SubscribeTo(EVENT.ON_PLAYER_DEATH, FadeToPersistentBGM);
    }

    public static AudioManager GetInstance()
    {
        return m_instance;
    }

    private void InitializeSFXDictionary()
    {
        m_sfxDictionary = new Dictionary<SFX, AudioClip>();
        m_sfxDictionary.Add(SFX.HITMARKER, m_hitmarkerSFX);
        m_sfxDictionary.Add(SFX.PLAYER_DRAW_BOW, m_playerDrawBowSFX);
        m_sfxDictionary.Add(SFX.PLAYER_HURT, m_playerHurtSFX);
        m_sfxDictionary.Add(SFX.PLAYER_SHOOT, m_playerShootSFX);
        m_sfxDictionary.Add(SFX.PLAYER_JUMP, m_playerJumpSFX);
        m_sfxDictionary.Add(SFX.PLAYER_DASH, m_playerDashSFX);
        m_sfxDictionary.Add(SFX.GRAPPLING_HOOK_THROW, m_grapplingHookThrowSFX);
        m_sfxDictionary.Add(SFX.GRAPPLING_HOOK_LAND, m_grapplingHookLandSFX);
        m_sfxDictionary.Add(SFX.GOBLIN_ATTACK1, m_goblinAttack1SFX);
        m_sfxDictionary.Add(SFX.GOBLIN_ATTACK2, m_goblinAttack2SFX);
        m_sfxDictionary.Add(SFX.GOBLIN_DRAW_BOW, m_goblinDrawBowSFX);
        m_sfxDictionary.Add(SFX.GOBLIN_HURT, m_goblinHurtSFX);
        m_sfxDictionary.Add(SFX.GOBLIN_DEATH, m_goblinDeathSFX);
        m_sfxDictionary.Add(SFX.ORC_ATTACK1, m_orcAttack1SFX);
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
    #endregion
}
