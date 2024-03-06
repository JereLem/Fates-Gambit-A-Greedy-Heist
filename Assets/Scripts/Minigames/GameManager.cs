using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int randomMinigame;

    [Header("Minigames")]
    [SerializeField] private GameObject simonSaysPrefab;
    [SerializeField] private GameObject arrowGamePrefab;
    [SerializeField] private GameObject dotConnectingPrefab;
    [SerializeField] private GameObject safeBoxPrefab;

    [Header("Bonus Multiplier")]
    [SerializeField] private float timeBonusMultiplier;
    [SerializeField] public PlayerUI playerUI;
    [SerializeField] public GameObject playerInfo;

    private PlayerStats playerStats;
    private PlayerMovement playerMovement;
    public static bool isMiniGameActive = false;
    //public static bool 
    public int level;
    private TMP_Text playerInfoText;

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
        playerUI = GameObject.Find("UI")?.GetComponent<PlayerUI>();
        playerInfo = playerUI.transform.Find("InfoPlayer")?.gameObject;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        level = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<LevelParameters>().levelNumber;
        playerInfoText = playerInfo.GetComponent<TMP_Text>();

        // If solved faster than 10s
        timeBonusMultiplier = 10f;
    }

    // Function to start a random minigame
    public void StartRandomMinigame()
    {
        int randomMinigame = UnityEngine.Random.Range(0, 2);
        playerMovement.enableHookshot = false;

        // Play the SafeBox game if you fulfill the condition of the game.
        if(playerStats.isNearBalcony && playerStats.enableSafeBox && !isMiniGameActive)
        {
            //AudioManager.instance.PlayMusic; 
            StartMiniGame(safeBoxPrefab);
        }
        //else if other minigame occurs. 
        else if (!isMiniGameActive)
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
            }
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
        int bonus = 0;
        string bonusText = "";


        // Calculate bonus based on the time taken and round to the nearest integer
        bonus = Mathf.RoundToInt(Mathf.Max(0, timeBonusMultiplier - timeTaken));
        bonusText = $"You are fast! You found some extra change {bonus}$";

        if (bonus > 0)
        {
            playerStats.AddValue(bonus);
            GameStats.Instance.pickpocketedValue += bonus;
            playerInfoText.text = bonusText;
            playerUI.StartCoroutine(playerUI.DisplayPlayerInfoText());
        }
    }


    public static bool IsMiniGameActive()
    {
        return isMiniGameActive;
    }

    public static void SetMiniGameActive(bool isActive)
    {
        isMiniGameActive = isActive;
    }

    public void ExecuteDotGameSuccessEffects()
    {
        PoliceNPC[] policeNPCs = GameObject.FindObjectsOfType<PoliceNPC>();
        foreach (PoliceNPC policeNPC in policeNPCs)
        {
            policeNPC.SendMessage("DisableChaseForSeconds");
        }

        GameObject background = GameObject.Find("Background");
        background.SendMessage("ChangeBackgroundColor");
    }
}

