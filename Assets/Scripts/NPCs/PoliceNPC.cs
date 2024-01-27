using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : MonoBehaviour
{

    // Police officer variables
    public float detectDistance = 3.0f;
    public float timeChasing = 5.0f;
    public float catchDelay = 2.0f;
    public bool isCatching = false;
    public int policeRank;
    private bool isAlertActive;


    // Player
    private Transform player;
    private PlayerStats playerStats;

    // Eye icon
    [SerializeField] public GameObject eyeIcon;

    // NPC movement
    private NPCMovement npcMovement;
    private bool isChasing = false;
    private Coroutine catchCoroutine;


    void Start()
    {
        // Alert state for the police officer
        isAlertActive = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        // Set the eye icon above the player
        eyeIcon.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        // Randomness for each police officer
        policeRank = Random.Range(1, 3);
        detectDistance = detectDistance + policeRank;
        catchDelay = 1.5f - (0.2f * policeRank);

        // Get movement script
        npcMovement = GetComponent<NPCMovement>();

    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within the detect distance, and player pickpocketing or police are on alert
        if (distanceToPlayer <= detectDistance && !isChasing && (playerStats.isPickpocketing || isAlertActive))
        {
            StartCoroutine(ChasePlayer());
        }

        // If police officer is trying to catch the player stop movement.
        if (isCatching)
        {
            npcMovement.StopMovement();
        }

        // Else continue moving.
        else
        {
            npcMovement.ResumeMovement();
        }
    }

    // Police officer goes on to allert mode, --> can catch the player even if not pickpocketing
    public void OfficerOnAlert()
    {
        ToggleEyeIcon();
        isAlertActive = true;
    }

    
    public void OfficerOffAlert()
    {
        ToggleEyeIcon();
        isAlertActive = false;
    }

    // Togglethe eyeIcon above the police officer when they are on alert
    void ToggleEyeIcon()
    {
        if (eyeIcon != null)
        {
           isAlertActive = !isAlertActive;
           eyeIcon.SetActive(isAlertActive);
        }
    }

    IEnumerator TryToCatchPlayer()
    {
        isCatching = true;

        // Wait for the catchDelay duration (Player has basically a final chance to get away)
        yield return new WaitForSeconds(catchDelay);

        // After the delay, perform the catching actions
        Debug.Log("Player caught!");
        playerStats.timesCaught++;

        isCatching = false;
    }

    IEnumerator ChasePlayer()
    {
        // Set the flag to indicate that the coroutine is running
        isChasing = true;
        
        // Toggle alert state and eye icon
        OfficerOnAlert();

        float elapsedTime = 0f;

        // Chase the player until the player is caught or distance exceeds detectDistance
        while ((Vector3.Distance(transform.position, player.position) <= detectDistance) && elapsedTime < timeChasing)
        {
            // Calculate the target position on the x-axis
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);

            // Move towards the target position on the x-axis
            float step = 3f * Time.deltaTime;
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
            }
        }
    }
}

