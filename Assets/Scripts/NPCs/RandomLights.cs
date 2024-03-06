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
    private PlayerMovement playerMovement;
    private Coroutine catchCoroutine;
    private const KeyCode pickpocketKey = KeyCode.E;


    [Header("Catch Delay")]
    public float catchDelay = 0.5f;
    public bool isCatching = false;

    //UI
    [SerializeField] public PlayerUI playerUI;
    [SerializeField] public GameObject playerInfo;
    public TMP_Text playerInfoText;

    // Ligth2D
    private UnityEngine.Rendering.Universal.Light2D light2DComponent;

    
    // Start is called before the first frame update
    void Start()
    {
        playerUI = GameObject.FindGameObjectWithTag("UI")?.GetComponent<PlayerUI>();
        playerInfo = playerUI.transform.Find("InfoPlayer")?.gameObject;
        lastToggleTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        lightObject.GetComponent<SpriteRenderer>().sprite = lightOffSprite;
        light2DComponent = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        playerInfoText = playerInfo.GetComponent<TMP_Text>();
        StartCoroutine(ToggleLights());
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickpocketKey) && playerStats.enableSafeBox)
        {
            playerStats.isPickpocketing = true;
        }
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

            // Set 2D light
            light2DComponent.intensity = lightsOn ? 0.5f : 0f;
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

    // If player contact with the lightOn window, TryToCatchPlayer occurs.
    void OnTriggerEnter2D(Collider2D other)
    {
        playerStats.isNearBalcony = true;

        // If the window lightOn, SafeBox minigame disable, and you have to move from the places. 
        if (other.CompareTag("Player") && lightObject.GetComponent<SpriteRenderer>().sprite == lightOnSprite)
        {
            if (GameObject.FindGameObjectWithTag("SafeBox")!=null)
            {
                GameManager.SetMiniGameActive(false);
                Destroy(GameObject.FindGameObjectWithTag("SafeBox"));
                Debug.Log("Destroy and trying to catch player");
                playerStats.isPickpocketing = false;
                playerStats.PickpocketingComplete();
            }
            catchCoroutine = StartCoroutine(TryToCatchPlayer());
        }
        else if (lightObject.GetComponent<SpriteRenderer>().sprite == lightOffSprite)
        {
            playerStats.enableSafeBox = true;
        }
    }

    // When the player exists from the window, isNearBalcony = false so you cannot play safeBox minigame. 
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCatching = false;
            Debug.Log("Outside of balcony");
            playerStats.isNearBalcony = false;
            playerStats.enableSafeBox = false;

            if (GameObject.FindGameObjectWithTag("SafeBox") != null)
            {
                GameManager.SetMiniGameActive(false);
                Destroy(GameObject.FindGameObjectWithTag("SafeBox"));
                playerStats.isPickpocketing = false;
                playerStats.PickpocketingComplete();
            }

            // Check if the coroutine is running before trying to stop it
            if (catchCoroutine != null)
            {
                Debug.Log("Player escaped!");
                StopCoroutine(catchCoroutine); // Stop ongoing catch coroutine
            }
        }
    }
}
