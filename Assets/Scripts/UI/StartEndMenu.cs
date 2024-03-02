using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartEndMenu : MonoBehaviour
{
    // Start new game
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameStats.Instance.SetNextLevel(1);
    }

    // Exit game
    public void Exit()
    {
        Application.Quit();
    }

    // Return to main menu
    public void Return()
    {
        SceneManager.LoadScene(0);
    }
}
