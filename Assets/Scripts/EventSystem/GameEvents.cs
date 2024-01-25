using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameEvents : MonoBehaviour
{
    public static GameEvents _current;
    public static GameEvents Current => _current;

    private void Awake()
    {
        if (_current != null && _current != this)
        {
            Destroy(gameObject);
            return;
        }

        _current = this;
        DontDestroyOnLoad(gameObject);
    }

    public event Action<int> Tick;
    public void OnTick(int currentTick)
    { 
        Tick?.Invoke(currentTick); 
    }

    public event Action<int> Level;
    public void OnLevelChanged(int level)
    { 
        SceneManager.LoadScene(level);
    }  

    public event Action<GameOverReason> GameOver;
    public void OnGameOver(GameOverReason reason)
    {
        GameStats.EndStats();
        GameStats.GameOverReason = reason;
        SceneManager.LoadScene("EndScene");
    }

}