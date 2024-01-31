using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickPocketing : MonoBehaviour
{

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    public void StartPickpocketing()
    {
        playerStats.isPickpocketing = true;
    }

    public void StopPickpocketing()
    {
        playerStats.isPickpocketing = false;
    }
}
