using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEnd : MonoBehaviour
{
    // Panels
    public GameObject winPanel;
    public GameObject losePanel;

    private GameObject activePanel;

    [SerializeField] public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure that GameStats has been instantiated
        if (GameStats.Instance == null)
        {
            Debug.LogError("GameStats instance is not available.");
            return;
        }

        // Determine the active panel based on the GameOverReason
        activePanel = null;
        string gameOverText = "";
        
        // Set delay
        float delay = 1.0f;

        switch (GameStats.Instance.GameOverReason)
        {
            case GameOverReason.PlayerCaught:
                animator.SetBool("lost", true);
                activePanel = losePanel;
                gameOverText = "You got caught!";
                break;
            case GameOverReason.RanOutOfTime:
                animator.SetBool("lost", true);
                activePanel = losePanel;
                gameOverText = "You ran out of time!";
                break;
            case GameOverReason.WonTheGame:
                animator.SetBool("won", true);
                activePanel = winPanel;
                gameOverText = "Awesome! You won the game!";
                break;
            default:
                break;
        }

        if (activePanel != null)
        {
            // Activate the panel
            float activationDelay = delay;
            Invoke("ActivatePanel", activationDelay);

            // Get UI elements and assign values within the active panel
            TMP_Text pedestriansPickpocketed = activePanel.transform.Find("pedestriansPickpocketed").GetComponent<TMP_Text>();
            pedestriansPickpocketed.text = GameStats.Instance.pedestriansPickpocketed.ToString();

            TMP_Text pickpocketedValue = activePanel.transform.Find("pickpocketedValue").GetComponent<TMP_Text>();
            pickpocketedValue.text = GameStats.Instance.pickpocketedValue.ToString();

            TMP_Text gameDuration = activePanel.transform.Find("GameDuration").GetComponent<TMP_Text>();
            
            float gameDurationInSeconds = GameStats.Instance.cumulativeGameDuration;
            
            // Convert seconds to total milliseconds
            int totalMilliseconds = Mathf.RoundToInt(gameDurationInSeconds * 1000);

            // Time calculations
            int remainingSeconds = totalMilliseconds / 1000;
            int remainingMinutes = remainingSeconds / 60;

            remainingSeconds %= 60;
            int remainingMilliseconds = totalMilliseconds % 1000;

            // Format as mm:ss:fff (minutes:seconds:milliseconds)
            string formattedGameDuration = string.Format("{0:00}:{1:00}:{2:000}", remainingMinutes, remainingSeconds, remainingMilliseconds);

            gameDuration.text = formattedGameDuration;

            TMP_Text gameOverReason = activePanel.transform.Find("GameOverReason").GetComponent<TMP_Text>();
            gameOverReason.text = gameOverText;
        }
    }

    // Method to activate the panel
    void ActivatePanel()
    {
        if (activePanel != null)
        {
            activePanel.SetActive(true);
        }
    }
}