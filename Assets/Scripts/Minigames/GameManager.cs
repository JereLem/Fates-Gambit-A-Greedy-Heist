using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int randomMinigame;

    [Header("Minigame Objects")]
    [SerializeField] private GameObject simonSaysPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Function to start a random minigame
    public void StartRandomMinigame()
    {
        int randomMinigame = UnityEngine.Random.Range(0, 3);

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

    // Function to start the SimonSays game
    private void StartSimonSaysGame()
    {
        if (simonSaysPrefab != null)
        {
            GameObject simonSaysObject = Instantiate(simonSaysPrefab);
            SimonSays simonSays = simonSaysObject.GetComponent<SimonSays>();

            if (simonSays != null)
            {
                simonSays.StartGame();
            }
        }

    }

    // Function to start the Arrow game
    private void StartArrowGame()
    {
    }

    // Function to start the Safe game
    private void StartSafeGame()
    {
    }

    // Function to start the ElectricalCircuit game
    private void StartCircuitGame()
    {
    }

}
