using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEnd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get UI elements and assign values
        Transform endScreen = GameObject.Find("EndScreen").transform;
        TMP_Text pedestriansPickpocketed = endScreen.Find("pedestriansPickpocketed").GetComponent<TMP_Text>();
        pedestriansPickpocketed.text = GameStats.pedestriansPickpocketed.ToString();

        TMP_Text pickpocketedValue = endScreen.Find("pickpocketedValue").GetComponent<TMP_Text>();
        pickpocketedValue.text = GameStats.pickpocketedValue.ToString();

        TMP_Text gameDuration = endScreen.Find("GameDuration").GetComponent<TMP_Text>();
        gameDuration.text = GameStats.GameDuration.ToString();

        TMP_Text gameOverReason = endScreen.Find("GameOverReason").GetComponent<TMP_Text>();

        switch (GameStats.GameOverReason)
        {
            case GameOverReason.PlayerCaught:
                gameOverReason.text = "You got caught!";
                break;
            case GameOverReason.RanOutOfTime:
                gameOverReason.text = "You ran out of time!";
                break;
            case GameOverReason.WonTheGame:
                gameOverReason.text = "Awesome! You won the game!";
                break;
            default:
                break;
        }
    }

}