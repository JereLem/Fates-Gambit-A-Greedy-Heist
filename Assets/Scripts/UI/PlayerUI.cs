using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    // Start is called before the first frame update

    public LevelParameters levelParameters;
    public PlayerStats playerStats;
    void Start()
    {
        Transform panel = GameObject.Find("TopPanel").transform;

        // Get data from EventSystem and PlayerStats
        levelParameters = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<LevelParameters>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        
        // Find every text element
        TMP_Text number = panel.Find("levelNumber").GetComponent<TMP_Text>();
        TMP_Text name = panel.Find("levelName").GetComponent<TMP_Text>();
        TMP_Text targetValue = panel.Find("targetValue").GetComponent<TMP_Text>();
        TMP_Text timeLimit = panel.Find("timeLimit").GetComponent<TMP_Text>();

        // Set every text to the UI
        number.text = levelParameters.levelNumber.ToString();
        name.text = levelParameters.levelName;
        targetValue.text = playerStats.pickpocketedValue.ToString() + "/" + levelParameters.targetValue.ToString();
        timeLimit.text = levelParameters.timerDuration.ToString();

    }
}
