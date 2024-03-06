using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotGameTrigger : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private const KeyCode pickpocketKey = KeyCode.R;


    private void Awake()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickpocketKey) && playerStats.isNearCircuit)
        {
            playerStats.isPickpocketing = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerStats.isNearCircuit = true;
            Debug.Log("Ready to play Circuit game");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerStats.isNearCircuit = false;
            Debug.Log("Outside of Circuit game area");

            if (GameObject.FindGameObjectWithTag("Circuit") != null)
            {
                GameManager.SetMiniGameActive(false);
                Destroy(GameObject.FindGameObjectWithTag("Circuit"));
                Debug.Log("circuit game stopped because out of boundary");
                playerStats.isPickpocketing = false;
                playerStats.PickpocketingComplete();
            }
        }
    }
}
