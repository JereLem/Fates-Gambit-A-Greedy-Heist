using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeBoxGame : MonoBehaviour
{
    public HUD hud;
    public bool isClear;
    public CircleSlider[] knobs;
    

    private PlayerStats playerStats;
    private int pickpocketableValue;
    [SerializeField] int pickpocketableValue_Min;
    [SerializeField] int pickpocketableValue_Max;

    private void Awake()
    {
        hud = GetComponentInChildren<HUD>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        pickpocketableValue = Random.Range(pickpocketableValue_Min, pickpocketableValue_Max);
    }

    private void Update()
    {
        if(hud.leftTime> 0)
        {
            if(isClear)
            {
                Debug.Log("SafeBox clear!");
                float gameTime = hud.totalTime - hud.leftTime;

                playerStats.AddValue(pickpocketableValue);

                // Add Values to gamestats also
                GameStats.Instance.pickpocketedValue += pickpocketableValue;
                GameStats.Instance.pedestriansPickpocketed += 1;

                GameManager.Instance.CalculateTimeBonus(gameTime);

                // Inform GameManager that the mini-game is no longer active
                GameManager.SetMiniGameActive(false);

                // Destroy the mini-game object
                Destroy(gameObject);

                playerStats.isPickpocketing = false;
            }
        }

        if(knobs[0].IsMissionCompleted() && knobs[1].IsMissionCompleted())
        {
            isClear = true;
        }
    }
}
