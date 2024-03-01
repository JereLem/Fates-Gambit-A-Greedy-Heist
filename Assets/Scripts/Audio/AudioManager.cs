using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] bgMusicList, minigameMusicList, sfxList;
    public AudioSource bgMusicSource, minigameMusicSource, sfxSource;

    public static AudioManager instance;
    private string currentScene;

    [Header("Volume settings")]
    [SerializeField] public float bgMusicVolume = 0.5f;
    [SerializeField] public float minigameMusicVolume = 0.7f;
    [SerializeField] public float sfxVolume = 1.0f; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.SetBgMusicVolume(bgMusicVolume);
            instance.SetMinigameMusicVolume(minigameMusicVolume);
            instance.SetSFXVolume(sfxVolume);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;

        switch (currentScene)
        {
            case "StartMenu":
                PlayMusic("MainMenuMusic");
                break;
            case "Level1":
                PlayMusic("Lv1MainMusic");
                break;
            case "Level2":
                PlayMusic("Lv2MainMusic");
                break;
            case "PatronScreen":
                PlayMusic("PatronMusic");
                break;
            case "EndScene":
                PlayMusic("CaughtScreenMusic");
                break;
            default:
                print("Current scene: " + currentScene);
                break;
        }
    }

    public void PlayMusic(string musicName)
    {
        if (minigameMusicSource.isPlaying)
            minigameMusicSource.Stop();

        if (bgMusicSource.isPlaying)
            bgMusicSource.Stop();

        Sound s = Array.Find(bgMusicList, x => x.name == musicName);
        if (s == null)
        {
            Debug.Log("Sound not found...");
            return;
        }

        bgMusicSource.clip = s.clip;
        bgMusicSource.Play();
    }

    public void PlayMinigameMusic(string musicName)
    {
        if (bgMusicSource.isPlaying)
            bgMusicSource.Stop();

        if (minigameMusicSource.isPlaying)
            minigameMusicSource.Stop();

        Sound s = Array.Find(minigameMusicList, x => x.name == musicName);
        if (s == null)
        {
            Debug.Log("Sound not found...");
            return;
        }

        minigameMusicSource.clip = s.clip;
        minigameMusicSource.Play();
    }


    public void StopMinigameMusic()
    {
        if (minigameMusicSource.isPlaying)
        {
            minigameMusicSource.Stop();
            PlayMusic(currentScene);
        }
    }


    public void PlaySFX(string sfxName)
    {
        Sound s = Array.Find(sfxList, x => x.name == sfxName);
        if (s == null)
        {
            Debug.Log("Sound not found...");
            return;
        }

        sfxSource.PlayOneShot(s.clip);
    }


    // Setting methods
    public void SetBgMusicVolume(float volume)
    {
        if (bgMusicSource != null)
        {
            bgMusicSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetMinigameMusicVolume(float volume)
    {
        if (minigameMusicSource != null)
        {
            minigameMusicSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}
