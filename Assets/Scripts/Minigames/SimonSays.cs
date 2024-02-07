using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;

public class SimonSays : MonoBehaviour
{

    // List to keep the sequence and playerinput
    public List<SimonColor> sequence = new List<SimonColor>();
    public List<SimonColor> playerInput = new List<SimonColor>();
    

    // Color Buttons
    [SerializeField] public Button redButton;
    [SerializeField] public Button greenButton;
    [SerializeField] public Button blueButton;
    [SerializeField] public Button yellowButton;

    // Get playerstats and current pedestrian
    private PlayerStats playerStats;
    private PedestrianNPC currentPedestrian;

    // To ensure the sequence is shown right 
    private int sequenceIndex = 0;
    private float highlightDuration = 1.0f;
    private float elapsedTime = 0.0f;


    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        StartGame();
    }

    void Update()
    {
        // Check if pedestrians stopped talking
        if (!currentPedestrian.isTalking)
        {
            playerInput.Clear();
            UnityEngine.Debug.Log("Too slow!");
            Destroy(gameObject);
        }

        // Check if the player has exited the pedestrian collider
        if (currentPedestrian != null && !currentPedestrian.triggerEntered)
        {
            playerInput.Clear();
            UnityEngine.Debug.Log("Player exited the pedestrian collider!");
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        float delayBeforeStart = 0.5f; // Adjust the delay time as needed
        StartCoroutine(StartGameWithDelay(delayBeforeStart));
        currentPedestrian = GetCurrentPedestrian();
    }

    IEnumerator StartGameWithDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Generate and show the sequence after the delay
        GenerateSequence();
        StartCoroutine(ShowSequence());

        // Enable input after showing the sequence
        EnableInput();
    }

    void GenerateSequence()
    {
        sequence.Clear();

        for (int i = 0; i < 5; i++) // length of the sequence
        {
            SimonColor randomColor = (SimonColor)UnityEngine.Random.Range(0, Enum.GetValues(typeof(SimonColor)).Length);
            sequence.Add(randomColor);
        }
    }

    void DisableInput()
    {
        // Disable button clicks during the sequence display
        redButton.onClick.RemoveAllListeners();
        greenButton.onClick.RemoveAllListeners();
        blueButton.onClick.RemoveAllListeners();
        yellowButton.onClick.RemoveAllListeners();
    }

    IEnumerator ShowSequence()
    {
        // Disable input from player when sequence the sequence is shown
        DisableInput();

        while (sequenceIndex < sequence.Count)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= highlightDuration)
            {
                elapsedTime = 0.0f;
                HighlightButton(sequence[sequenceIndex]);
                sequenceIndex++;
                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
    }

    void HighlightButton(SimonColor color)
    {
        Image buttonImage = null;
        Color originalColor = Color.white;

        switch (color)
        {
            case SimonColor.Red:
                buttonImage = redButton.GetComponent<Image>();
                originalColor = buttonImage.color;
                buttonImage.color = Color.red;
                break;
            case SimonColor.Green:
                buttonImage = greenButton.GetComponent<Image>();
                originalColor = buttonImage.color;
                buttonImage.color = Color.green;
                break;
            case SimonColor.Blue:
                buttonImage = blueButton.GetComponent<Image>();
                originalColor = buttonImage.color;
                buttonImage.color = Color.blue;
                break;
            case SimonColor.Yellow:
                buttonImage = yellowButton.GetComponent<Image>();
                originalColor = buttonImage.color;
                buttonImage.color = Color.yellow;
                break;
        }

        StartCoroutine(ResetButtonColor(buttonImage, originalColor, 0.5f));

    }


    IEnumerator ResetButtonColor(Image buttonImage, Color originalColor, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Check if the button color is still the highlighted color
        if (buttonImage.color != originalColor)
        {
            // Reset the button color to the original color
            buttonImage.color = originalColor;
        }
    }


    void EnableInput()
    {
        // Remove existing listeners before adding new ones
        redButton.onClick.RemoveAllListeners();
        greenButton.onClick.RemoveAllListeners();
        blueButton.onClick.RemoveAllListeners();
        yellowButton.onClick.RemoveAllListeners();

        // Enable button clicks for player input
        redButton.onClick.AddListener(() => OnButtonClick(SimonColor.Red));
        greenButton.onClick.AddListener(() => OnButtonClick(SimonColor.Green));
        blueButton.onClick.AddListener(() => OnButtonClick(SimonColor.Blue));
        yellowButton.onClick.AddListener(() => OnButtonClick(SimonColor.Yellow));
    }

    void OnButtonClick(SimonColor color)
    {
        // Add the clicked color to the player's input
        playerInput.Add(color);
        HighlightButton(color);

        // Check if the player's input matches the sequence
        if (!CheckInput())
        {
            // Wrong input --> stop the game
            playerInput.Clear();
            Destroy(gameObject);
        }
        else if (playerInput.Count == sequence.Count)
        {
            // Correct input --> get value of current pedestrian and add to player
            playerInput.Clear();
            playerStats.AddValue(currentPedestrian.pickpocketableValue);
            currentPedestrian.hasBeenPickpocketed = true;
            currentPedestrian.SetMaxCycles();
            Destroy(gameObject);
        }
    }

    PedestrianNPC GetCurrentPedestrian()
    {
        // Return the pedestrian currently being pickpocketed
        foreach (PedestrianNPC pedestrian in PedestrianNPC.talkingNPCs)
        {
            if (pedestrian.triggerEntered)
            {
                return pedestrian;
            }
        }

        return null; // Return null if no pedestrian is being pickpocketed
    }

    bool CheckInput()
    {
        List<SimonColor> playerInputCopy = new List<SimonColor>(playerInput);

        // Check if the player's input matches the current sequence
        for (int i = 0; i < playerInputCopy.Count; i++)
        {
            if (playerInputCopy[i] != sequence[i])
            {
                return false;
            }
        }
        return true;
    }
}
