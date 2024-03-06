using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class SafeBoxGame : MonoBehaviour
{
    public HUD hud;
    public bool isClear;
    public CircleSlider[] knobs;
    public int level;

    private void Awake()
    {
        hud = GetComponentInChildren<HUD>();
    }
    private void Start()
    {
        level = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<LevelParameters>().levelNumber;

    }

    private void Update()
    {
        if(hud.leftTime> 0)
        {
            if(isClear)
            {
                Debug.Log("SafeBox clear!");
                float gameTime = hud.totalTime - hud.leftTime;
                GameManager.Instance.CalculateTimeBonus(gameTime);

                AudioManager.instance.PlaySFX(level == 1 ? "Lv1MinigameWin" : "Lv2MinigameWin");

                // Play laugh SFX
                AudioManager.instance.PlaySFX("pickpocket_success");

                // Inform GameManager that the mini-game is no longer active
                GameManager.SetMiniGameActive(false);

                // Destroy the mini-game object
                Destroy(gameObject);
            }
        }

        if(knobs[0].IsMissionCompleted() && knobs[1].IsMissionCompleted())
        {
            isClear = true;
        }
    }
}
