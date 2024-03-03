using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventSystem;
public class PlayerUI : MonoBehaviour
{
    // Reference to scripts
    public LevelParameters levelParameters;
    public PlayerStats playerStats;
    public TickSystem tickSystem;
    private int remainingTicks;

    [SerializeField] public GameObject playerInfo;
    private TMP_Text playerInfoText;
    void Start()
    {
        // Get data from EventSystem, PlayerStats and TickSystem
        levelParameters = GameObject.FindGameObjectWithTag("EventSystem")?.GetComponent<LevelParameters>();
        tickSystem = GameObject.FindGameObjectWithTag("EventSystem")?.GetComponent<TickSystem>();
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        playerInfoText = playerInfo.GetComponent<TMP_Text>();
        remainingTicks = levelParameters.timerDuration;

        UpdateUI();
    }

    void Update()
    {
        // Update the remaining ticks
        remainingTicks = levelParameters.timerDuration - tickSystem.GetCurrentTick();
        UpdateUI();
    }

    void UpdateUI()
    {
        Transform panel = GameObject.Find("TopPanel").transform;

        // Find every text element
        TMP_Text number = panel.Find("levelNumber").GetComponent<TMP_Text>();
        TMP_Text name = panel.Find("levelName").GetComponent<TMP_Text>();
        TMP_Text targetValue = panel.Find("targetValue").GetComponent<TMP_Text>();
        TMP_Text timeLimit = panel.Find("timeLimit").GetComponent<TMP_Text>();

        // Set every text to the UI
        number.text = levelParameters.levelNumber.ToString();
        name.text = levelParameters.levelName;
        targetValue.text = playerStats.pickpocketedValue.ToString() + "/" + levelParameters.targetValue.ToString() + "$";


        // Time calculations
        int ticksPerSecond = 60; // Assuming tick speed is 60 ticks per second
        int totalMilliseconds = remainingTicks * (1000 / ticksPerSecond); // Convert ticks to milliseconds

        int remainingSeconds = totalMilliseconds / 1000;
        int remainingMinutes = remainingSeconds / 60;

        remainingSeconds %= 60;
        int remainingMilliseconds = totalMilliseconds % 1000;

        // Format as mm:ss:fff (minutes:seconds:milliseconds)
        string formattedTicks = string.Format("{0:00}:{1:00}:{2:000}", remainingMinutes, remainingSeconds, remainingMilliseconds);
        timeLimit.text = formattedTicks;

    }

    public IEnumerator DisplayPlayerInfoText()
    {
        playerInfo.gameObject.SetActive(true);

        // Display for 3 seconds
        yield return new WaitForSeconds(3f);

        // Deactivate the playerInfo text
        playerInfo.gameObject.SetActive(false);
    }
}

