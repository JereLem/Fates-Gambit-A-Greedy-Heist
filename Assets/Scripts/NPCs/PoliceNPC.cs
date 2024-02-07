using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : NPCMovement
{

    // Police officer variables
    public float detectDistance = 3.0f;
    public float timeChasing = 5.0f;
    public float timeOnAlert = 5.0f;
    public float catchDelay = 2.0f;
    public bool isCatching = false;
    private bool isChasing = false;
    public int policeRank;
    private bool isAlertActive;
    [SerializeField] private const float initialCatchDelay = 0.5f;
    [SerializeField] private const float minCatchDelayMultiplier = 0.2f;
    [SerializeField] private float runSpeed = 3.0f;


    // Player
    private Transform player;
    private PlayerStats playerStats;

    // Eye icon
    [SerializeField] public GameObject eyeIcon;

    // NPC movement
    private Coroutine catchCoroutine;


    new void Start()
    {
        // Alert state for the police officer
        isAlertActive = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();

        // Set the eye icon above the player
        eyeIcon.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        // Randomness for each police officer
        policeRank = Random.Range(1, 2);
        detectDistance = detectDistance + policeRank;
        catchDelay = initialCatchDelay - (minCatchDelayMultiplier * policeRank);
        base.Start();

    }

    void Update()
    {
        Move();
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
        ToggleEyeIcon();
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
        ToggleEyeIcon();
    }

    // Toggle the eyeIcon above the police officer when they are on alert
    void ToggleEyeIcon()
    {
        eyeIcon.SetActive(isAlertActive);
    }

    IEnumerator TryToCatchPlayer()
    {
        isCatching = true;

        // Wait for the catchDelay duration (Player has basically a final chance to get away)
        yield return new WaitForSeconds(catchDelay);

        // After the delay, perform the catching actions
        playerStats.timesCaught++;

        isCatching = false;
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

