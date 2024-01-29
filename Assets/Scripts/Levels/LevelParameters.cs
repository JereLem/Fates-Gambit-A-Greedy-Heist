using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelParameters : MonoBehaviour
{
    [Header("Level parameters")]
    [SerializeField] public int levelNumber;
    [SerializeField] public string levelName;
    [SerializeField] public int timerDuration;
    [SerializeField] public int targetValue;


    // Get ticks
    private void Start()
    {
        // Subscribe to the OnTick event
        GameEvents._current.Tick += OnTickHandler;
    }


    // If timelimit is reached --> stop the game
    private void OnTickHandler(int currentTick)
    {
        if (currentTick >= timerDuration)
        {
            GameEvents._current.OnGameOver(GameOverReason.RanOutOfTime);
        }
    }
}
