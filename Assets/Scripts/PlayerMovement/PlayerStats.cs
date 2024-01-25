using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public int timesCaught; // Current times player has been caught
    public int maximumTimesCaught = 1; // Maximum times player can be caught
    public int pickpocketedValue;  // Inital value of pickpocketed items

    public LevelParameters levelParameters; // Get parameters for current level

    // Start is called before the first frame update
    void Start()
    {
        timesCaught = 0;
        pickpocketedValue = 0;
        levelParameters = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<LevelParameters>();
    }
    public void AddValue(int amount){
        SetValue(pickpocketedValue + amount);        
    }
    public void RemoveValue(int amount){
        SetValue(pickpocketedValue - amount);
    }

    public void SetValue(int amount)
    {
        pickpocketedValue = amount;
    }


    void Update()
    {
        if (timesCaught > maximumTimesCaught)
        {
            HandlePlayerCaught();
        }

        if (pickpocketedValue >= levelParameters.targetValue)
        {
            GameEvents._current.OnLevelChanged(levelParameters.levelNumber + 1);
        }
    }

    void OnValueChange(int oldValue, int newValue)
    {
        GameStats.pickpocketedValue = newValue;
    }

    protected void HandlePlayerCaught()
    {
        GameEvents._current.OnGameOver(GameOverReason.PlayerCaught);
    }
}
