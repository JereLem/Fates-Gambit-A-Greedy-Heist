using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscMenu : MonoBehaviour
{
    private bool isPaused = false;
    public PlayerMovement player;

    void Start()
    {
        gameObject.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
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

    private void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        // Activate/deactivate the first child of the GameObject
        Transform firstChild = transform.GetChild(0);
        if (firstChild != null)
        {
            firstChild.gameObject.SetActive(isPaused);
            player.enableMovementAll = isPaused ? false : true;
        }
    }
}
