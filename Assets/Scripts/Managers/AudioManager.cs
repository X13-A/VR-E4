using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Extract sound + cut: https://soundly.cc/fr
// Preview cut: https://ytcutter.cc/ 

public class AudioManager : Singleton<AudioManager>, IEventHandler
{
    [Header("UI")]
    [SerializeField] private float maxSFXVolume = 1;

    [Header("Gameplay")]
    [SerializeField] private float maxGameplayVolume = 1;
    [SerializeField] private AudioClip ambient;
    [SerializeField] private AudioClip maxAmmo;
    [SerializeField] private AudioClip loseGame;
    [SerializeField] private AudioClip winGame;

    [Header("Guns")]
    [SerializeField] private AudioClip SKS_shot;

    [Header("Zombie")]
    [SerializeField] private AudioClip spawnZombie;
    [SerializeField] private AudioClip hitZombie;
    [SerializeField] private AudioClip groanZombie1;
    [SerializeField] private AudioClip groanZombie2;
    [SerializeField] private AudioClip groanZombie3;
    [SerializeField] private AudioClip groanZombie4;
    [SerializeField] private AudioClip deathZombie1;
    [SerializeField] private AudioClip deathZombie2;

    [Header("Music")]
    [SerializeField] private float maxMenuVolume = 1;
    [SerializeField] private AudioClip menu;

    Dictionary<string, Tuple<AudioClip, string>> audioClips;
    List<Tuple<string, float>> audioTypes;
    float saveSFXVolume = 0;
    float saveGameplayVolume = 0;
    float saveMenuVolume = 0;
    bool mute = false;
    public float MaxSFXVolume => maxSFXVolume;
    public float MaxGameplayVolume => maxGameplayVolume;
    public float MaxMenuVolume => maxMenuVolume;
    public bool Mute => mute;

    void Awake()
    {
        /* /!\ Add all audio clips to the dictionary /!\ */
        // TODO: Should use enums here
        audioClips = new Dictionary<string, Tuple<AudioClip, string>>()
        {
            { "ambient", Tuple.Create(ambient, "gameplay") },
            { "maxAmmo", Tuple.Create(maxAmmo, "gameplay") },
            { "loseGame", Tuple.Create(loseGame, "gameplay") },
            { "winGame", Tuple.Create(winGame, "gameplay") },
            { "spawnZombie", Tuple.Create(spawnZombie, "gameplay") },
            { "hitZombie", Tuple.Create(hitZombie, "gameplay") },
            { "groanZombie1", Tuple.Create(groanZombie1, "gameplay") },
            { "groanZombie2", Tuple.Create(groanZombie2, "gameplay") },
            { "groanZombie3", Tuple.Create(groanZombie3, "gameplay") },
            { "groanZombie4", Tuple.Create(groanZombie4, "gameplay") },
            { "deathZombie1", Tuple.Create(deathZombie1, "gameplay") },
            { "deathZombie2", Tuple.Create(deathZombie2, "gameplay") },
            { "SKS_shot", Tuple.Create(SKS_shot, "gameplay") },
            { "menu", Tuple.Create(menu, "menu") }
        };

        /* /!\ Add all audio types to the lists in UpdateVolumeList() function /!\ */
        UpdateVolumeList();
    }

    void UpdateVolumeList()
    {
        audioTypes = new List<Tuple<string, float>>()
        {
            Tuple.Create("sfx", maxSFXVolume),
            Tuple.Create("gameplay", maxGameplayVolume),
            Tuple.Create("menu", maxMenuVolume),
        };
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlaySoundEvent>(PlaySound);
        EventManager.Instance.AddListener<StopSoundEvent>(StopSound);
        EventManager.Instance.AddListener<StopSoundAllEvent>(StopAllSound);
        EventManager.Instance.AddListener<SoundMixAllEvent>(MixSFX);
        EventManager.Instance.AddListener<StopSoundByTypeEvent>(StopSoundByType);
        EventManager.Instance.AddListener<MuteAllSoundEvent>(MuteAllSound);
        EventManager.Instance.AddListener<SoundMixSoundEvent>(SoundMixSound);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlaySoundEvent>(PlaySound);
        EventManager.Instance.RemoveListener<StopSoundEvent>(StopSound);
        EventManager.Instance.RemoveListener<StopSoundAllEvent>(StopAllSound);
        EventManager.Instance.RemoveListener<SoundMixAllEvent>(MixSFX);
        EventManager.Instance.RemoveListener<StopSoundByTypeEvent>(StopSoundByType);
        EventManager.Instance.RemoveListener<MuteAllSoundEvent>(MuteAllSound);
        EventManager.Instance.RemoveListener<SoundMixSoundEvent>(SoundMixSound);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void SoundMixSound(SoundMixSoundEvent e)
    {
        GameObject childObject = GameObject.Find(e.eNameClip);
        if (childObject == null) return;
        float max = getMaxVolumeByName(e.eNameClip);
        float volume = max * e.eVolume;
        AudioSource audioSource = childObject.GetComponent<AudioSource>();
        audioSource.volume = volume;
    }

    void MuteAllSound(MuteAllSoundEvent e)
    {
        if (e.eMute)
        {
            mute = true;
            saveGameplayVolume = maxGameplayVolume;
            saveMenuVolume = maxMenuVolume;
            saveSFXVolume = maxSFXVolume;
            EventManager.Instance.Raise(new SoundMixAllEvent()
            {
                eGameplayVolume = 0,
                eMenuVolume = 0,
                eSFXVolume = 0,
            });
        }
        else
        {
            mute = false;
            EventManager.Instance.Raise(new SoundMixAllEvent()
            {
                eGameplayVolume = saveGameplayVolume,
                eMenuVolume = saveMenuVolume,
                eSFXVolume = saveSFXVolume,
            });
        }
    }

    void MixSFX(SoundMixAllEvent e)
    {
        if (e.eSFXVolume.HasValue)
        {
            maxSFXVolume = e.eSFXVolume.Value;
        }

        if (e.eGameplayVolume.HasValue)
        {
            maxGameplayVolume = e.eGameplayVolume.Value;
        }

        if (e.eMenuVolume.HasValue)
        {
            maxMenuVolume = e.eMenuVolume.Value;
        }

        UpdateVolumeList();
        UpdateSound();
    }

    void PlaySound(PlaySoundEvent e)
    {
        AudioSource audioSource;
        Transform soundTransform = GetExistingClip(e.eNameClip);
        if (soundTransform != null && e.eCanStack == false)
        {
            return;
        }

        GameObject childObject = new GameObject(e.eNameClip);
        childObject.transform.parent = transform;
        audioSource = childObject.AddComponent<AudioSource>();
        audioClips.TryGetValue(e.eNameClip, out Tuple<AudioClip, string> audioClip);
        audioSource.time = 0;
        audioSource.clip = audioClip.Item1;
        audioSource.volume = getVolume(audioClip.Item2) * e.eVolumeMultiplier;
        audioSource.loop = e.eLoop;
        audioSource.pitch = e.ePitch;
        audioSource.Play();

        if (e.eDestroyWhenFinished)
        {
            StartCoroutine(DestroyAfterPlaying(audioSource));
        }
    }

    /// <summary>
    /// Wait until the audio clip has finished playing, then destroy it
    /// </summary>
    private IEnumerator DestroyAfterPlaying(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        Destroy(audioSource.gameObject);
    }

    Transform GetExistingClip(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    void StopSound(StopSoundEvent e)
    {
        GameObject childObject = GameObject.Find(e.eNameClip);
        if (childObject == null) return;
        Destroy(childObject);
    }

    void StopSoundByType(StopSoundByTypeEvent e)
    {
        foreach (Transform child in transform)
        {
            audioClips.TryGetValue(child.name, out Tuple<AudioClip, string> audioClip);
            if (audioClip.Item2 == e.eType)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void StopAllSound(StopSoundAllEvent e)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateSound()
    {
        foreach (Transform child in transform)
        {
            AudioSource audioSource = child.GetComponent<AudioSource>();
            audioClips.TryGetValue(child.name, out Tuple<AudioClip, string> audioClip);
            audioSource.volume = getVolume(audioClip.Item2);
        }
    }

    float getVolume(string type)
    {
        foreach (Tuple<string, float> audioType in audioTypes)
        {
            if (audioType.Item1 == type)
            {
                return audioType.Item2;
            }
        }

        return 0;
    }   

    float getMaxVolumeByName(string name)
    {
        audioClips.TryGetValue(name, out Tuple<AudioClip, string> audioClip);
        return getVolume(audioClip.Item2);
    }
}
