using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using TMPro;

public class SimonSays : MonoBehaviour
{

    [Header("Sequence & Player Input")]
    public List<SimonColor> sequence = new List<SimonColor>();
    public List<SimonColor> playerInput = new List<SimonColor>();
    

    [Header("Color Buttons")]
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
    private float gameTime;
    public int level;
    [SerializeField] public PlayerUI playerUI;
    [SerializeField] public GameObject playerInfo;
    public TMP_Text playerInfoText;



    void Start()
    {
        playerUI = GameObject.FindGameObjectWithTag("UI")?.GetComponent<PlayerUI>();
        playerInfo = playerUI.transform.Find("InfoPlayer")?.gameObject;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        level = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<LevelParameters>().levelNumber;
        playerInfoText = playerInfo.GetComponent<TMP_Text>();
        playerInfoText.text = "Dammit, the pedestrians got away! You were too slow!";
        StartGame();
    }

    void Update()
    {
        // Check if pedestrians stopped talking
        if (!currentPedestrian.isTalking)
        {
            playerInput.Clear();
            playerInfoText.text = "Dammit, the pedestrians got away! You were too slow!";
            playerUI.StartCoroutine(playerUI.DisplayPlayerInfoText());
            DestroyMiniGame();
        }

        // Check if the player has exited the pedestrian collider
        if (currentPedestrian != null && !currentPedestrian.triggerEntered)
        {
            playerInput.Clear();
            UnityEngine.Debug.Log("Player exited the pedestrian collider!");
            DestroyMiniGame();
        }

        // If player gets caught stop game
        if(playerStats.hasBeenCaught)
        {
            DestroyMiniGame();
        }

        gameTime += Time.deltaTime;
    }

    public void StartGame()
    {
        // Check with GameManager if a mini-game is already active
        if (!GameManager.IsMiniGameActive())
        {
            GameManager.SetMiniGameActive(true);
            DisableInput();
            float delayBeforeStart = 0.25f;
            StartCoroutine(StartGameWithDelay(delayBeforeStart));
            currentPedestrian = GetCurrentPedestrian();
        }
        
    }


    IEnumerator StartGameWithDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Generate and show the sequence after the delay
        GenerateSequence();
        StartCoroutine(ShowSequence());
        gameTime = 0f;
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


    IEnumerator ShowSequence()
    {
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
        EnableInput();
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

    void DisableInput()
    {
        // Disable button clicks during the sequence display
        redButton.interactable = false;
        greenButton.interactable = false;
        blueButton.interactable = false;
        yellowButton.interactable = false;
    }

    void EnableInput()
    {
        redButton.interactable = true;
        greenButton.interactable = true;
        blueButton.interactable = true;
        yellowButton.interactable = true;

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
            DestroyMiniGame();
            AudioManager.instance.PlaySFX(level == 1 ? "Lv1MinigameLoss" : "Lv2MinigameLoss");
            playerInfoText.text = "Hah, nice try... You're not pickpocketing me!";
            playerUI.StartCoroutine(playerUI.DisplayPlayerInfoText());
        }
        else if (playerInput.Count == sequence.Count)
        {
            // Correct input --> get value of current pedestrian and add to player
            playerInput.Clear();
            playerStats.AddValue(currentPedestrian.pickpocketableValue);
            currentPedestrian.hasBeenPickpocketed = true;
            currentPedestrian.SetMaxCycles();

            // Add Values to gamestats also
            GameStats.Instance.pickpocketedValue = currentPedestrian.pickpocketableValue;
            GameStats.Instance.pedestriansPickpocketed += 1;
        
            // Call GameManager method to calculate and apply the time bonus
            GameManager.Instance.CalculateTimeBonus(gameTime);

            AudioManager.instance.PlaySFX(level == 1 ? "Lv1MinigameWin" : "Lv2MinigameWin");
            // Play laugh SFX
            AudioManager.instance.PlaySFX("pickpocket_success");

            DestroyMiniGame();
        }
    }

    void DestroyMiniGame()
    {
        // Inform GameManager that the mini-game is no longer active
        GameManager.SetMiniGameActive(false);

        // Destroy the mini-game object
        Destroy(gameObject);
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
