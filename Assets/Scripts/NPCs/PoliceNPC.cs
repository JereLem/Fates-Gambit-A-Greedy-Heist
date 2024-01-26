using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : MonoBehaviour
{

    // Police officer variables
    public float detectDistance = 10.0f;
    public float catchDelay = 2.0f;
    public bool isCatching = false;
    public int policeRank;
    private bool isAlertActive;


    // Player
    private Transform player;
    private PlayerStats playerStats;

    // Eye icon
    [SerializeField] public GameObject eyeIcon;
    private bool isEyeIconActive = false;

    // NPC movement
    private NPCMovement npcMovement;


    void Start()
    {
        // Alert state for the police officer
        //isAlertActive = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        // Set the eye icon above the player
        eyeIcon.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        // Randomness for each police officer
        policeRank = Random.Range(1, 3);
        detectDistance = 0.5f * policeRank;
        catchDelay = 2.0f - (0.2f * policeRank);

        // Get movement script
        npcMovement = GetComponent<NPCMovement>();

        // Register the police officer with GameEvents
        //GameEvents._current.RegisterPoliceOfficer(this);
    }


    // If the police officer collides with the player, try to catch the player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCatching)
        {
            Debug.Log("Player Pickpocketing and catch");
            StartCoroutine(CatchPlayerWithDelay());
        }
    }


    // Police officer goes on to allert mode, --> can catch the player even if not pickpocketing
    public void OfficerOnAlert()
    {
        ToggleEyeIcon();
    }


    // When the player exits the police collide, player basically escapes
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset any catching state
            isCatching = false;
            StopAllCoroutines(); // Stop ongoing catch coroutine
        }
    }

    // Togglethe eyeIcon above the police officer when they are on alert
    void ToggleEyeIcon()
    {
        if (eyeIcon != null)
        {
           isEyeIconActive = !isEyeIconActive;
            eyeIcon.SetActive(isEyeIconActive);
        }
    }

    IEnumerator CatchPlayerWithDelay()
    {
        isCatching = true;

        // Wait for the catchDelay duration (Player has basically a final chance to get away)
        yield return new WaitForSeconds(catchDelay);

        // After the delay, perform the catching actions
        Debug.Log("Catching player!");
        playerStats.timesCaught++;

        // Reset the catching flag
        isCatching = false;
    }
}

