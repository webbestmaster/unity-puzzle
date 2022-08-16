using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Header("Background sounds"), SerializeField]
    private AudioClip[] clipList;

    private AudioSource audioSource;
    private bool isPlaying = false;
    private int currentIndex = 0;
    private bool isShuft = true;

    // public static SoundManager Instance { get; private set; }
    private static SoundManager Instance; //  { get; private set; }

    private void Awake()
    {
        
        // Debug.Log(Application.systemLanguage);

        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.mute = false;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0.5f;
        // pitch - actually playing speed
        // audioSource.pitch = 32f;
    }
    
    private void Start()
    {
        SetupAudioSource();
        currentIndex = isShuft ? GetNextClipIndex() : currentIndex;
        isPlaying = true;
    }

    private void Update()
    {
        PlayAudio();
    }

    private int GetNextClipIndex()
    {
        int clipListLength = clipList.Length;

        if (clipListLength <= 1)
        {
            return 0;
        }

        if (isShuft)
        {
            int nextIndex = Random.Range(0, clipListLength);

            return nextIndex == currentIndex ? GetNextClipIndex() : nextIndex;
        }

        // just text looped index
        return (int)Math.Round(Mathf.Repeat(currentIndex + 1, clipListLength));
    }

    private void PlayAudio()
    {
        AudioClip clip = clipList[currentIndex];

        bool isTrackEnded = isPlaying && audioSource.isPlaying == false && audioSource.time > 0;

        // playing state
        if (isPlaying)
        {
            if (isTrackEnded)
            {
                currentIndex = GetNextClipIndex();
                clip = clipList[currentIndex];
            }

            if (audioSource.clip != clip)
            {
                audioSource.clip = clip;
            }

            if (!audioSource.isPlaying)
            {
                // Debug.Log("PLAY!!! " + currentIndex);
                audioSource.Play();
            }

            return;
        }

        // stop/pause state
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
}

/*
    Singleton.Instance.SomeMethod(someArgument);
*/