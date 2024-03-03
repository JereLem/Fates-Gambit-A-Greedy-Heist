using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomLights : MonoBehaviour
{
    [Header("Gameobject and Sprites")]
    public GameObject lightObject;
    public Sprite lightOnSprite;   
    public Sprite lightOffSprite;

    [Header("Intervals")]
    public float toggleInterval = 2f;  // Interval in seconds
    public float lastToggleTime;

    // Player
    private Transform player;
    private PlayerStats playerStats;
    private Coroutine catchCoroutine;

    [Header("Catch Delay")]
    public float catchDelay = 0.5f;
    public bool isCatching = false;

    //UI
    [SerializeField] public PlayerUI playerUI;
    [SerializeField] public GameObject playerInfo;
    public TMP_Text playerInfoText;
    
    // Start is called before the first frame update
    void Start()
    {
        playerUI = GameObject.FindGameObjectWithTag("UI")?.GetComponent<PlayerUI>();
        playerInfo = playerUI.transform.Find("InfoPlayer")?.gameObject;
        lastToggleTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        lightObject.GetComponent<SpriteRenderer>().sprite = lightOffSprite;
        playerInfoText = playerInfo.GetComponent<TMP_Text>();
        StartCoroutine(ToggleLights());
    }


    // Coroutine to toggle lights at random intervals
    IEnumerator ToggleLights()
    {
        while (true)
        {
            yield return new WaitForSeconds(toggleInterval);

            // Toggle lights randomly
            bool lightsOn = Random.value > 0.5f;

            // Set the sprite based on the lights state
            lightObject.GetComponent<SpriteRenderer>().sprite = lightsOn ? lightOnSprite : lightOffSprite;
        }
    }

    IEnumerator TryToCatchPlayer()
    {
        isCatching = true;

        // Wait for the catchDelay duration (Player has basically a final chance to get away)
        yield return new WaitForSeconds(catchDelay);
        playerInfoText.text = "Hey I caught you! Get off of my balcony!";
        playerUI.StartCoroutine(playerUI.DisplayPlayerInfoText());

        // After the delay, perform the catching actions
        playerStats.timesCaught++;

        isCatching = false;
    }

    // If the police officer collides with the player, try to catch the player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && lightObject.GetComponent<SpriteRenderer>().sprite == lightOnSprite)
        {

            catchCoroutine = StartCoroutine(TryToCatchPlayer());
        }
    }


    // When the player exits the police collide, player basically escapes
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCatching = false;
            // Check if the coroutine is running before trying to stop it
            if (catchCoroutine != null)
            {
                Debug.Log("Player escaped!");
                StopCoroutine(catchCoroutine); // Stop ongoing catch coroutine
            }
        }
    }
}
