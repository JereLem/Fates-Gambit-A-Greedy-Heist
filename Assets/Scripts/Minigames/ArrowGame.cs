using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArrowGame : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite arrowUp;
    public Sprite arrowDown;
    public Sprite arrowUpWithBox;
    public Sprite arrowDownWithBox;
    public Sprite x;

    [Header("Duration and rounds")]
    public float roundDuration = 1.5f;
    public int currentRound = 0;
    public int maxRounds = 10;

    [Header("Slots")]
    public List<GameObject> slots = new List<GameObject>();
    public List<Sprite> sequence = new List<Sprite>();
    public bool isGameRunning = true;
    public int userInput;
    // Reference to your UI Text element
    public TMP_Text roundIndicator;

    // Get playerstats and current pedestrian
    private PlayerStats playerStats;
    private PedestrianNPC currentPedestrian;
    private bool roundCompleted;
    private float gameTime;

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        currentPedestrian = GetCurrentPedestrian();
        StartCoroutine(RunGame());
        isGameRunning = true;
    }

    private void Update()
    {
        // Check for user input outside the coroutine
        if (isGameRunning)
        {
            CheckUserInput();
        }

        // Check if pedestrians stopped talking
        if (!currentPedestrian.isTalking)
        {
            UnityEngine.Debug.Log("Too slow!");
            Destroy(gameObject);
        }

        // Check if the player has exited the pedestrian collider
        if (currentPedestrian != null && !currentPedestrian.triggerEntered)
        {
            UnityEngine.Debug.Log("Player exited the pedestrian collider!");
            Destroy(gameObject);
        }

        // If player gets caught stop game
        if(playerStats.hasBeenCaught)
        {
            Destroy(gameObject);
        }

        gameTime += Time.deltaTime;
    }



private IEnumerator RunGame()
{
    gameTime = 0f;
    for (currentRound = 0; currentRound < maxRounds && isGameRunning;)
    {
        sequence = GenerateSequence();
        DisplaySequence(sequence);

        // Update the round indicator text
        roundIndicator.text = "Round: " + (currentRound + 1);


        roundCompleted = false;
        float timer = 0f;

        while (timer < roundDuration && !roundCompleted)
        {
            timer += Time.deltaTime;
            yield return null;

            // Check for user input during the round
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Ensure that the user input is checked
                if (HandleArrowInput())
                {
                    // User input was correct, proceed to the next round
                    currentRound++;
                    roundCompleted = true;
                    userInput = 0;
                }
                else
                {
                    // User input was incorrect, stop the game
                    Destroy(gameObject);
                }
            }
            else{
                if (HandleArrowInput())
                {
                    // Ensure that the user input is checked
                    if ( userInput == 0 && timer >= roundDuration)
                    {
                        // User input was correct, proceed to the next round
                        currentRound++;
                        roundCompleted = true;
                        userInput = 0;
                    }
                }
            }
        }

        // Check if the round was not completed within the time limit
        if (!roundCompleted)
        {
            // The timer exceeded the round duration, stop the game
            Destroy(gameObject);
        }

    }
    // Check if the player has successfully completed all rounds
    if (currentRound >= maxRounds)
    {
            playerStats.AddValue(currentPedestrian.pickpocketableValue);
            currentPedestrian.hasBeenPickpocketed = true;
            currentPedestrian.SetMaxCycles();
        
            // Call GameManager method to calculate and apply the time bonus
            GameManager.Instance.CalculateTimeBonus(gameTime);

            // Play laugh SFX
            AudioManager.instance.PlaySFX("pickpocket_success");
    }

    // Ensure the game is deleted
    Destroy(gameObject);
    }      

    private List<Sprite> GenerateSequence()
    {
        List<Sprite> sequence = new List<Sprite>(5);

        // Generate middle arrow (either up or down)
        int middleArrowIndex = Random.Range(0, 1);

        // Generate other arrows (with same sprite)
        int randomSpriteIndex = Random.Range(0, 5);
        Sprite randomSprite;

        // Set middleArrowIndex to randomSpriteIndex only if randomSpriteIndex is in the range [0, 2]
        if (randomSpriteIndex >= 0 && randomSpriteIndex <= 1)
        {
            middleArrowIndex = randomSpriteIndex;
            randomSprite = GetRandomSprite(randomSpriteIndex);
        }
        else
        {
            randomSprite = GetRandomSprite(randomSpriteIndex);
        }

        sequence.Add(null); // Index 0
        sequence.Add(null); // Index 1
        sequence.Insert(2, GetRandomSprite(middleArrowIndex)); // Index 2 (middle arrow)
        sequence.Add(null); // Index 3
        sequence.Add(null); // Index 4

        // Fill other slots with the random sprite
        for (int i = 0; i < sequence.Count; i++)
        {
            if (sequence[i] == null)
            {
                sequence[i] = randomSprite;
            }
        }

        return sequence;
    }


    private void DisplaySequence(List<Sprite> sequence)
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            if (i < slots.Count)
            {
                Image image = slots[i].GetComponent<Image>();
                image.sprite = sequence[i];
            }
        }
    }

    private Sprite GetRandomSprite(int index)
    {
        switch (index)
        {
            case 0:
                return arrowUp;
            case 1:
                return arrowDown;
            case 2:
                return arrowUpWithBox;
            case 3:
                return arrowDownWithBox;
            case 4:
                return x;
            default:
                return null;
        }
    }


    private void CheckUserInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            userInput = 1;
            HandleArrowInput();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            userInput = -1;
            HandleArrowInput();
        }
        else
        {
            userInput = 0;
            HandleArrowInput();
        }
    }

    private bool HandleArrowInput()
    {
        int firstArrowIndex = 0;
        int middleArrowIndex = 2;

        if (userInput == 1 && sequence[middleArrowIndex] == arrowUp && (sequence[middleArrowIndex] == sequence[firstArrowIndex]))
        {
            Debug.Log("Arrow up middle");
            return true;
        }
        else if (userInput == -1 && sequence[middleArrowIndex] == arrowDown && (sequence[middleArrowIndex] == sequence[firstArrowIndex]))
        {
            Debug.Log("Arrow down middle");
            return true;
        }
        else if (userInput == 1 && sequence[firstArrowIndex] == arrowUpWithBox && sequence[middleArrowIndex] != arrowUpWithBox)
        {
            Debug.Log("Arrow up (with box)");
            return true;
        }
        else if (userInput == -1 && sequence[firstArrowIndex] == arrowDownWithBox && sequence[middleArrowIndex] != arrowDownWithBox)
        {
            Debug.Log("Arrow down (with box)");
            return true;
        }
        else if (userInput == 0 && sequence[firstArrowIndex] == x)
        {
            Debug.Log("X");
            return true;
        }
        
        // If none of the conditions are met, the input is incorrect
        return false;
    }


    private bool IsGameRunning()
    {
        return isGameRunning;
    }

    private void StopGame()
    {
        isGameRunning = false;
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

}
