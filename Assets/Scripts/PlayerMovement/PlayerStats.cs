using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{

    [Header("Player Variables")]
    public int timesCaught;
    public int maximumTimesCaught = 1;
    public int pickpocketedValue;

    // Animations
    public Animator animator;

    [Header("Level Parameters")]
    public LevelParameters levelParameters;

    private int finalLevel = 2;

    [Header("Player Flags")]
    public bool isPickpocketing;
    private bool isPickpocketingInProgress = false;
    public bool hasBeenCaught = false;
    public bool isNearBalcony = false;
    public bool enableSafeBox = false;

    [Header("UI")]
    private Color originalColor;
    private Color grayedOutColor;
    public Image highlightCaught;
    private Transform panel;

    // Start is called before the first frame update
    void Awake()
    {
        timesCaught = 0;
        pickpocketedValue = 0;
        isPickpocketing = false;
        levelParameters = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<LevelParameters>();
        panel = GameObject.Find("LowerPanel").transform;
        highlightCaught = panel.Find("HighlightCaught").GetComponent<Image>();

    }

    // Methods to add/remove value
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

        // Player caught too many times --> Game ends
        if (timesCaught > maximumTimesCaught)
        {
            GameEvents._current.OnGameOver(GameOverReason.PlayerCaught);
        }

        // Player reached target value --> Change level or won game
        if (pickpocketedValue >= levelParameters.targetValue)
        {
            if (levelParameters.levelNumber < finalLevel)
            {
                GameEvents._current.OnLevelChanged(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                GameEvents._current.OnGameOver(GameOverReason.WonTheGame);
            }
        }
        
       // Handle pickpocketing 
       if (isPickpocketing && !isPickpocketingInProgress)
        {
            isPickpocketingInProgress = true;
            animator.SetBool("isPickpocketing", true);
            OnPickPocketing();
        }
        if(!isPickpocketing)
        {
            PickpocketingComplete();
        }
        if(timesCaught > 0)
        {
            highlightCaught.color = Color.red;
        }
    }

    // Call the game event
    public void OnPickPocketing()
    {
        GameEvents._current.OnPickPocketing(true);
        hasBeenCaught = false;
    }

    // Method to reset the flag after pickpocketing is complete.
    public void PickpocketingComplete()
    {
        isPickpocketingInProgress = false;
        animator.SetBool("isPickpocketing", false);

    }
}
