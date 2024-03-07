using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameEvents : MonoBehaviour
{
    public static GameEvents _current;
    public static GameEvents Current => _current;

    private GameManager gameManager;

    private void Awake()
    {
        _current = this;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GameStats.Instance = FindObjectOfType<GameStats>();
    }

    public event Action<int> Tick;
    public void OnTick(int currentTick)
    { 
        Tick?.Invoke(currentTick); 
    }

    public event Action<bool> PickPocketing;
    public void OnPickPocketing(bool isPickpocketing)
    {
        // 10% chance that all police officers are alerted when pickpocketing happens
        if (isPickpocketing && UnityEngine.Random.Range(0f, 1f) <= 0.1f)

        {
            AlertAllPoliceOfficers();
        }
        gameManager.StartRandomMinigame();
        PickPocketing?.Invoke(isPickpocketing);

    }


    public event Action<int> Level;
    public void OnLevelChanged(int level)
    { 
        SceneManager.LoadScene(level);
        GameStats.Instance.EndStats(); // Here the timer value is stored
        GameStats.Instance.SetNextLevel(level);
        Level?.Invoke(level);
    }  

    public event Action<GameOverReason> GameOver;
    public void OnGameOver(GameOverReason reason)
    {
        GameStats.Instance.GameOverReason = reason;
        GameStats.Instance.EndStats(); // Here we get the final time
        SceneManager.LoadScene("EndScene");
        GameStats.Instance.GetCumulativeGameDuration();
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

}