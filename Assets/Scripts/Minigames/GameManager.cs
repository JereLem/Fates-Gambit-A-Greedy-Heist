using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int randomMinigame;

    [Header("Minigames")]
    [SerializeField] private GameObject simonSaysPrefab;
    [SerializeField] private GameObject arrowGamePrefab;

    [Header("Bonus Multiplier")]
    [SerializeField] private float timeBonusMultiplier;

    private PlayerStats playerStats;
    public static bool isMiniGameActive = false;
    public int level;

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
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        level = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<LevelParameters>().levelNumber;

        // All minigames 30 seconds
        timeBonusMultiplier = 30f;
    }

    // Function to start a random minigame
    public void StartRandomMinigame()
    {
        int randomMinigame = UnityEngine.Random.Range(0, 3);
        if (!isMiniGameActive)
        {
            AudioManager.instance.PlayMinigameMusic(level == 1 ? "Lv1Minigame" : "Lv2Minigame");
            switch (randomMinigame)
            {
                case 0:
                    StartMiniGame(simonSaysPrefab);
                    break;
                case 1:
                    StartMiniGame(arrowGamePrefab);
                    break;
                case 2:
                    StartMiniGame(simonSaysPrefab);
                    break;
            }
        }
        else
        {
            AudioManager.instance.StopMinigameMusic();
        }
    }

    // Function to start the minigames
    private void StartMiniGame(GameObject minigamePrefab)
    {
        if (minigamePrefab != null)
        {
            GameObject gameObject = Instantiate(minigamePrefab);
        }

    }

    // Calculate time bonus
    public void CalculateTimeBonus(float timeTaken)
    {
        // Calculate bonus based on the time taken and round to the nearest integer
        int bonus = Mathf.RoundToInt(Mathf.Max(0, timeBonusMultiplier - timeTaken));
        playerStats.AddValue(bonus);
    }

    public static bool IsMiniGameActive()
    {
        return isMiniGameActive;
    }

    public static void SetMiniGameActive(bool isActive)
    {
        isMiniGameActive = isActive;
    }
}
