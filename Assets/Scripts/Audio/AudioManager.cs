using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] bgMusicList, minigameMusicList, sfxList;
    public AudioSource bgMusicSource, minigameMusicSource, sfxSource;

    public static AudioManager instance;

    private void Awake()
    {
        if ( instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        } 
    }

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

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
            minigameMusicSource.Pause();

        if (bgMusicSource.clip != null)
            bgMusicSource.UnPause();
        else
        {
            Sound s = Array.Find(bgMusicList, x => x.name == musicName);
            if (s == null)
            {
                Debug.Log("Sound not found...");
                return;
            }

            bgMusicSource.clip = s.clip;
            bgMusicSource.Play();
        }
    }

    public void PlayMinigameMusic(string musicName)
    {
        if (bgMusicSource.isPlaying)
            bgMusicSource.Stop();

        if (minigameMusicSource.clip != null)
            minigameMusicSource.UnPause();
        else
        {
            Sound s = Array.Find(minigameMusicList, x => x.name == musicName);
            if (s == null)
            {
                Debug.Log("Sound not found...");
                return;
            }

            minigameMusicSource.clip = s.clip;
            minigameMusicSource.Play();
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
}
