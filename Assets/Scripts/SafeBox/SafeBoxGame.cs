using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeBoxGame : MonoBehaviour
{
    public HUD hud;
    public bool isClear;


    private void Awake()
    {
        hud = GetComponentInChildren<HUD>();
    }

    private void Update()
    {
        if(hud.leftTime> 0)
        {
            if(isClear)
            {
                float gameTime = hud.totalTime - hud.leftTime;
                GameManager.Instance.CalculateTimeBonus(gameTime);

                // Inform GameManager that the mini-game is no longer active
                GameManager.SetMiniGameActive(false);

                // Destroy the mini-game object
                Destroy(gameObject);
            }
        }
    }
}
