using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{


    // Player variables
    public int timesCaught;
    public int maximumTimesCaught = 1;
    public int pickpocketedValue;

    // Get levelParameters
    public LevelParameters levelParameters;

    //Flag to see if player is pickpocketing
    public bool isPickpocketing;
    private PickPocketing currentPickpocketTarget;

    // Start is called before the first frame update
    void Start()
    {
        timesCaught = 0;
        pickpocketedValue = 0;
        isPickpocketing = false;
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
            GameEvents._current.OnGameOver(GameOverReason.PlayerCaught);
        }

        if (pickpocketedValue >= levelParameters.targetValue)
        {
            GameEvents._current.OnLevelChanged(levelParameters.levelNumber + 1);
        }

        if (isPickpocketing && currentPickpocketTarget != null)
        {
            GameEvents._current.OnPickPocketing(true);
            currentPickpocketTarget.StartPickpocketing();
        }
        else if (!isPickpocketing && currentPickpocketTarget != null)
        {
            currentPickpocketTarget.StopPickpocketing();
        }
    }

    public void SetPickpocketTarget(PickPocketing target)
    {
        currentPickpocketTarget = target;
    }
}
