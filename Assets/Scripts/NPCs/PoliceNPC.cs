using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : NPCMovement
{

    // Police officer variables
    [Header("Police Officer Variables")]
    [SerializeField] public int policeRank;
    [SerializeField] public float detectDistance = 1.0f;
    [SerializeField] private float runSpeed = 1.5f;



    [Header("Police Officer Times & Delays")]
    [SerializeField] public float timeChasing = 5.0f;
    [SerializeField] public float timeOnAlert = 5.0f;
    [SerializeField] public float catchDelay = 2.0f;
    [SerializeField] private const float initialCatchDelay = 2.0f;
    [SerializeField] private const float minCatchDelayMultiplier = 0.2f;


    [Header("Police Officer Flags")]
    [SerializeField] public bool isCatching = false;
    [SerializeField] private bool isChasing = false;
    [SerializeField] private bool isAlertActive;


    [Header("Other Icons")]
    [SerializeField] public GameObject eyeIcon;
    [SerializeField] public GameObject circleIcon;



    // Player
    private Transform player;
    private PlayerStats playerStats;
    private Coroutine catchCoroutine;
    private SpriteRenderer circleSprite;


    new void Start()
    {
        // Alert state for the police officer
        isAlertActive = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();

        // Set the eye icon above the player, and get the detectSprite
        eyeIcon.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        circleSprite = circleIcon.GetComponent<SpriteRenderer>();

        // Randomness for each police officer
        policeRank = Random.Range(1, 3);
        detectDistance = detectDistance + policeRank;
        catchDelay = initialCatchDelay - (minCatchDelayMultiplier * policeRank);
        base.Start();

    }

    void Update()
    {
        FlipSprite();
        circleSprite.transform.localScale = new Vector3(detectDistance * 2, 0.5f, 1.0f);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within the detect distance and player pickpocketing or police are on alert
        if (distanceToPlayer <= detectDistance && !isChasing && (playerStats.isPickpocketing || isAlertActive))
        {
            StartCoroutine(ChasePlayer());
        }

        // If police officer is trying to catch the player stop movement.
        if (isCatching)
        {
            StopMovement();
        }

        if (!isChasing) // Only move if not chasing
        {
            base.Move(); // Call the base Move method for regular NPC movement
        }

        // Else continue moving.
        else
        {
            ResumeMovement();
        }
    }

    // Police officer goes on to alert mode
    public void OfficerOnAlert()
    {
        isAlertActive = true;
        ToggleIcons();
        StartCoroutine(onAlert());
    }

    // Stay on alert for set duration, and then stop alert
    IEnumerator onAlert()
    {
        yield return new WaitForSeconds(timeOnAlert);
        OfficerOffAlert();
    }
    
    public void OfficerOffAlert()
    {  
        isAlertActive = false;
        ToggleIcons();
    }

    // Toggle the eyeIcon above the police officer when they are on alert
    void ToggleIcons()
    {
        eyeIcon.SetActive(isAlertActive);
        circleIcon.SetActive(isAlertActive);
    }


    IEnumerator TryToCatchPlayer()
    {
        // Check if the player has already been caught by another police officer
        if (!playerStats.hasBeenCaught)
        {
            isCatching = true;

            // Wait for the catchDelay duration (Player has basically a final chance to get away)
            yield return new WaitForSeconds(catchDelay);

            // After the delay, perform the catching actions
            playerStats.timesCaught++;
            playerStats.hasBeenCaught = true;

            isCatching = false;
        }
    }

    IEnumerator ChasePlayer()
    {
        // Set the flag to indicate that the coroutine is running
        isChasing = true;
        
        // Toggle alert state and eye icon only if transitioning from alert to chasing
        OfficerOnAlert();

        float elapsedTime = 0f;

        // Chase the player until the player is caught or distance exceeds detectDistance
        while ((Vector3.Distance(transform.position, player.position) <= detectDistance) && elapsedTime < timeChasing)
        {
            // Calculate the target position on the x-axis
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);

            // Move towards the target position on the x-axis
            float step = runSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
            
            // Update the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // After catching or losing the player, toggle off the alert state and eye icon
        OfficerOffAlert();

        // Reset the flag when the coroutine completes
        isChasing = false;
    }

    // If the police officer collides with the player, try to catch the player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && (playerStats.isPickpocketing || isAlertActive))
        {
            Debug.Log("Trying to catch the player!");
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
                playerStats.isPickpocketing = false;
            }
        }
    }
}

