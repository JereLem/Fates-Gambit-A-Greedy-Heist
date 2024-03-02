using EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance { get; set; }

    public int pedestriansPickpocketed = 0;
    public int pickpocketedValue = 0;
    public float GameDuration { get; set; }
    public GameOverReason GameOverReason { get; set; }
    public TickSystem tickSystem;

    public float cumulativeGameDuration = 0f;  // Store the cumulative game duration
    public int currentLevel = 0;

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

    private void LoadTickSystemAsync()
    {
        int targetSceneIndex = currentLevel;
        SceneManager.LoadSceneAsync(targetSceneIndex).completed += LoadSceneComplete;
    }

    private void LoadSceneComplete(AsyncOperation operation)
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == currentLevel)
        {
            UnityEngine.Debug.Log(currentLevel);
            tickSystem = GameObject.FindGameObjectWithTag("EventSystem")?.GetComponent<TickSystem>();
            if (tickSystem == null)
            {
                Debug.LogWarning("TickSystem not found in the scene.");
            }
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0)
        {
            cumulativeGameDuration = 0f; // Reset cumulative time when returning to scene 0
            currentLevel = 0;
        }
        else if (currentSceneIndex == currentLevel)
        {
            UnityEngine.Debug.Log(currentLevel);
            tickSystem = GameObject.FindGameObjectWithTag("EventSystem")?.GetComponent<TickSystem>();
            if (tickSystem == null)
            {
                Debug.LogWarning("TickSystem not found in the scene.");
            }
        }
    }

    public void SetTickSystem(TickSystem tick)
    {
        tickSystem = tick;
    }

    public void EndStats()
    {
        if (tickSystem != null)
        {
            GameDuration = (float)tickSystem.GetCurrentTick() / (float)tickSystem.GetTickSpeed();
            cumulativeGameDuration += GameDuration;
            UnityEngine.Debug.Log($"Level: {currentLevel}, Game Duration: {GameDuration}, Cumulative Duration: {cumulativeGameDuration}");
        }
    }
    public void SetNextLevel(int nextLevel)
    {
        currentLevel = nextLevel;
        LoadTickSystemAsync();
    }


    public float GetCumulativeGameDuration()
    {
        return cumulativeGameDuration;
    }
}
