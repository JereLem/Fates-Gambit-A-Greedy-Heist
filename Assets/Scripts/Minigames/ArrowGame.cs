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

    [Header("Duration")]
    public float roundDuration = 3f;

    [Header("Sequence")]
    public List<Sprite> sequence = new List<Sprite>();
    public List<GameObject> slots = new List<GameObject>();

    private int currentRound = 0;
    private bool isGameRunning = true;
    private const int NumberOfRounds = 5;


    private void Start()
    {
        StartCoroutine(RunGame());
    }

    private IEnumerator RunGame()
    {
        while (currentRound < 5 && isGameRunning)
        {
            Sprite[] generatedSequence = GenerateSequence();
            DisplaySequence(generatedSequence);

            float timer = 0f;
            while (timer < roundDuration)
            {
                timer += Time.deltaTime;
                yield return null;

                // Check for user input during the round
                CheckUserInput();

                // If the game state changes (e.g., due to incorrect input), break out of the round loop
                if (!isGameRunning)
                    break;
            }

            currentRound++;
        }

        // Game is either completed or stopped, add any necessary logic here
        Debug.Log("Game Over");
    }

    private Sprite[] GenerateSequence()
    {
        sequence.Clear();

        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, 5);
            Sprite randomSprite = GetRandomSprite(randomIndex);
            sequence.Add(randomSprite);
        }

        return sequence.ToArray();
    }

    private void DisplaySequence(Sprite[] sequence)
    {
        for (int i = 0; i < sequence.Length; i++)
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

        private void Update()
    {
        CheckUserInput();
    }

    private void CheckUserInput()
    {
        // Check for user input only if the game is running
        if (IsGameRunning())
        {
            int middleIndex = sequence.Count / 2; // Get the index of the middle arrow

            // Check if there is a square around the middle arrow
            if (IsSquareAround(middleIndex))
            {
                // Check the direction of the arrows around the middle arrow
                CheckDirectionAround(middleIndex);
            }
            else
            {
                // Check the direction of the middle arrow
                CheckDirection(middleIndex);
            }
        }
    }

    

    private bool IsGameRunning()
    {
        return isGameRunning;
    }

    private void StopGame()
    {
        isGameRunning = false;
    }

    private bool IsSquareAround(int index)
    {
        // Check if there is a square around the arrow at the specified index
        if (index > 0 && index < sequence.Count - 1)
        {
            return sequence[index - 1] == arrowUpWithBox || sequence[index - 1] == arrowDownWithBox
                || sequence[index + 1] == arrowUpWithBox || sequence[index + 1] == arrowDownWithBox;
        }
        return false;
    }

    private void CheckDirection(int index)
    {
        // Check the direction of the middle arrow
        if (Input.GetKeyDown(KeyCode.UpArrow) && sequence[index] == arrowUp)
        {
            // Correct input for arrowUp
            Debug.Log("Correct input for arrowUp");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && sequence[index] == arrowDown)
        {
            // Correct input for arrowDown
            Debug.Log("Correct input for arrowDown");
        }
        else
        {
            // Incorrect input
            Debug.Log("Incorrect input");
        }
    }

    private void CheckDirectionAround(int index)
    {
        // Check the direction of the arrows around the middle arrow
        if (Input.GetKeyDown(KeyCode.UpArrow) && (sequence[index - 1] == arrowUpWithBox || sequence[index - 1] == arrowDownWithBox))
        {
            // Correct input for arrowUpWithBox or arrowDownWithBox on the left
            Debug.Log("Correct input for arrowUpWithBox");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && (sequence[index + 1] == arrowUpWithBox || sequence[index + 1] == arrowDownWithBox))
        {
            // Correct input for arrowUpWithBox or arrowDownWithBox on the right
            Debug.Log("Correct input for arrowDownWithBox");
        }
        else
        {
            // Incorrect input
            Debug.Log("Incorrect input");
        }
    }
}
