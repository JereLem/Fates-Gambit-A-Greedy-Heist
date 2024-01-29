using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameEvents : MonoBehaviour
{
    public static GameEvents _current;
    public static GameEvents Current => _current;

    // Variables for minigames
    private int randomMinigame;
    [SerializeField] private GameObject simonSaysPrefab;

    private void Awake()
    {
        _current = this;
    }

    public event Action<int> Tick;
    public void OnTick(int currentTick)
    { 
        Tick?.Invoke(currentTick); 
    }

    public event Action<bool> PickPocketing;
    public void OnPickPocketing(bool isPickpocketing)
    {
        PickPocketing?.Invoke(isPickpocketing);
        // 10% chance that all police officers are alerted when pickpocketing happens
        if (isPickpocketing && UnityEngine.Random.Range(0f, 1f) <= 0.1f)

        {
            AlertAllPoliceOfficers();
        }
        randomMinigame = UnityEngine.Random.Range(0, 3);
        
        switch (randomMinigame)
        {
            case 0:
                StartSimonSaysGame();
                break;
            case 1:
                StartSimonSaysGame();
                break;
            case 2:
                StartSimonSaysGame();
                break;
        }
    }


    public event Action<int> Level;
    public void OnLevelChanged(int level)
    { 
        SceneManager.LoadScene(level);
        Level?.Invoke(level);
    }  

    public event Action<GameOverReason> GameOver;
    public void OnGameOver(GameOverReason reason)
    {
        GameStats.EndStats();
        GameStats.GameOverReason = reason;
        SceneManager.LoadScene("EndScene");
        GameOver?.Invoke(reason);
    }

    // Function to alert all police officers
    void AlertAllPoliceOfficers()
    {
        PoliceNPC[] policeOfficers = GameObject.FindObjectsOfType<PoliceNPC>();

        foreach (PoliceNPC policeOfficer in policeOfficers)
        {
            // Activate the alert state for each police officer
            policeOfficer.OfficerOnAlert();
        }
    }

    // Function to start the simonsaysgame
    public void StartSimonSaysGame()
    {
        if (simonSaysPrefab != null)
        {
            GameObject simonSaysObject = Instantiate(simonSaysPrefab);
            SimonSays simonsays = simonSaysObject.GetComponent<SimonSays>();

            // Check if simonsays component is null
            if (simonsays == null)
            {
                Debug.LogError("SimonSays component not found on the instantiated object.");
                return;
            }

            simonsays.StartGame();
        }
        else
        {
            Debug.LogError("SimonSays prefab is not assigned in the inspector.");
        }
    }


}