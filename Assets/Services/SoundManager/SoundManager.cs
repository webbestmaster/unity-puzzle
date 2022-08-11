using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [Header("Background sounds")]
    [SerializeField] private AudioClip[] clipList;
    public static SoundManager Instance { get; private set; }
    
    private void Awake()
    {
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

    private void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.clip = clipList[Random.Range(0, clipList.Length)];
        
        audioSource.Play();
    }
}

/*
    Singleton.Instance.SomeMethod(someArgument);
*/