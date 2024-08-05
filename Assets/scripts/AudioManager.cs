using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set;}

    public Sound[] MusicSound, SfxSound;
    public AudioSource MusicSource, SfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Play music based on the currently active scene
        PlaySceneMusic();
    }

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Play music based on the newly loaded scene
        PlaySceneMusic();
    }

    private void PlaySceneMusic()
    {
        // Get the name of the current active scene
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Play appropriate music based on the current scene
        if (currentSceneName == "Main Menu")
        {
            PlayMusic("Main Menu Music");
        }
        else if (currentSceneName == "Boxey TDS")
        {
            PlayMusic("Game play music");
        }
    }

    public void PlayMusic(string name)
    {
        // Find the Sound object in the MusicSound array
        Sound s = Array.Find(MusicSound, X => X.name == name);
        if (s == null)
        {
            Debug.LogError("Sound not found: " + name);
            return;
        }

        // Set the clip and play the music
        MusicSource.clip = s.clip;
        MusicSource.Play();
    }

    public void PlaySFX(string name)
    {
        // Find the Sound object in the SfxSound array
        Sound s = Array.Find(SfxSound, X => X.name == name);
        if (s == null)
        {
            Debug.LogError("Sound not found: " + name);
            return;
        }

        // Play the sound effect
        SfxSource.PlayOneShot(s.clip);
    }

    public void ToggleMusic()
    {
        // Toggle the mute state of the music source
        MusicSource.mute = !MusicSource.mute;
    }

    public void ToggleSFX()
    {
        // Toggle the mute state of the SFX source
        SfxSource.mute = !SfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        // Set the volume of the music source
        MusicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        // Set the volume of the SFX source
        SfxSource.volume = volume;
    }
}