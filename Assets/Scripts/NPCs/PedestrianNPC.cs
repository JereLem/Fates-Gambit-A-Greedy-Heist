using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using EventSystem;
using UnityEngine.UI;


public class PedestrianNPC : NPCMovement
{
    [Header("Pedestrian Variables")]
    public Sprite[] sprites;
    SpriteRenderer spriteRendererPed;
    Color pickpocketedColor;
    public float talkDuration = 5f;
    public bool isTalking = false;
    public int pickpocketableValue;

    // Chances
    private int pedestrianTypeChance;
    private int policeAlertChance;
    private float conversationChanceThreshold = 0.25f;

    // NPCs currently talking and max ammount that can talk
    private int currentTalkingNPCs = 0;
    private int maxTalkingNPCs = 6;

    // Player
    private Transform player;
    private PlayerStats playerStats;
    

    // List to store all talking pedestrians
    public static List<PedestrianNPC> talkingNPCs = new List<PedestrianNPC>();

    [Header("Pickpocketing Flags")]
    private const KeyCode pickpocketKey = KeyCode.E;
    private const float pickpocketingDistanceThreshold = 2f;
    public bool hasBeenPickpocketed = false;

    // Extra flag to check the player can start pickpocketing the pedestrian
    public bool triggerEntered;

    [Header("UI & Icons")]
    private Transform panel;
    private Color originalColor;
    private Color grayedOutColor;
    public Image highlightPickpocket;

    [SerializeField] public GameObject talkingIcon;
    public GameObject TalkingIcons;
    private GameObject clone;

    
    // Animator
    private Animator animator;


    new void Start()
    {
        // Set male or female sprite
        spriteRendererPed = GetComponent<SpriteRenderer>();
        spriteRendererPed.sprite = sprites[Random.Range(0,sprites.Length)];

        // Get player transform and stats
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        animator = player.GetComponent<Animator>();
        
        // 90% chance the pedestrian is default type 10% rich 
        pedestrianTypeChance = Random.Range(1, 11);
        pickpocketableValue = (pedestrianTypeChance <= 9) ? Random.Range(10, 15) : Random.Range(20, 25);

        // Randomize layer of sprites, making sure it's different from the player's level
        SetRandomSortingOrder();

        // UI stuff
        panel = GameObject.Find("LowerPanel").transform;
        highlightPickpocket = panel.Find("HighlightPickpocket").GetComponent<Image>();
        originalColor = highlightPickpocket.color;
        grayedOutColor = Color.gray;

        // Folder for talking icons
        TalkingIcons = GameObject.FindGameObjectWithTag("TalkingIcons");

        base.Start();

    }


    void SetRandomSortingOrder()
    {
        // Set a random sorting order
        int randomSortingOrder = Random.Range(1, 4);

        // Check if it's the same as the player's level, regenerate if needed
        while (randomSortingOrder == player.GetComponent<SpriteRenderer>().sortingOrder)
        {
            randomSortingOrder = Random.Range(1, 4);
        }

        // Set the sorting order for the pedestrian sprite
        spriteRendererPed.sortingOrder = randomSortingOrder;
    }

    void Update()
    {
        Move();
        FlipSprite();
        // Pickpocketing is activated by pressing E, and player has to be near 2 talking pedestrians
        if (Input.GetKeyDown(pickpocketKey) && triggerEntered == true)
        {
            StartPickpocketing();
            animator.SetBool("isPickpocketing",true);
        }
        else if(hasBeenPickpocketed)
        {
            spriteRendererPed.color = grayedOutColor;
            SetMaxCycles();
        }

        highlightPickpocket.color = triggerEntered ? originalColor : grayedOutColor;
    }

    private void StartPickpocketing()
    {
        if (!hasBeenPickpocketed)
        {
            playerStats.isPickpocketing = true;
        }
    }

    private void StopPickpocketing()
    {
        playerStats.isPickpocketing = false;
        animator.SetBool("isPickpocketing",false);
    }

    // Pedestrians start to talk, and will talk a set duration
    IEnumerator StartTalking()
    {
        if (!isTalking)
        {
            isTalking = true;

            animator.SetBool("isTalking", true);

            yield return new WaitForSeconds(talkDuration);

            animator.SetBool("isTalking", false);

            // Delete
            Destroy(clone);

            // Resume movement after NPCs have stop talking
            ResumeMovement();

            // Deactivate pickpocketing if pedestrians stop talking
            StopPickpocketing();

            currentTalkingNPCs--;

            if (talkingNPCs.Contains(this))
            {
                talkingNPCs.Remove(this);
            }

            isTalking = false;
        }
    }

    // If two pedestrians collide they might start talking 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PedestrianNPC") && !isTalking && !hasBeenPickpocketed)
        {
            PedestrianNPC otherNPC = collision.gameObject.GetComponent<PedestrianNPC>();

            if (!otherNPC.isTalking && !talkingNPCs.Contains(this) && !talkingNPCs.Contains(otherNPC)
                && currentTalkingNPCs < maxTalkingNPCs - 1 && otherNPC.currentTalkingNPCs < maxTalkingNPCs - 1 && movingToEndPoint != otherNPC.movingToEndPoint)
            {
                // Introduce a random chance for the conversation to start
                float randomChance = Random.Range(0.0f, 1.0f);

                if (randomChance < conversationChanceThreshold)
                {
                    StartCoroutine(StartTalking());

                    // Stop NPCMovement when they start to talk
                    StopMovement();

                    otherNPC.StartCoroutine(otherNPC.StartTalking());
                    otherNPC.StopMovement();

                    // Set talking icon by finding the midpoint between the two pedestrians
                    Vector3 midpoint = (transform.position + otherNPC.transform.position) / 2f;

                    // Spawn the talkingIcon at the midpoint
                    clone = (GameObject) Instantiate(talkingIcon, new Vector3(midpoint.x, midpoint.y + 1.5f, midpoint.z), Quaternion.identity);
                    clone.transform.parent = TalkingIcons.transform;

                    currentTalkingNPCs++;
                    otherNPC.currentTalkingNPCs++;

                    talkingNPCs.Add(this);
                    talkingNPCs.Add(otherNPC);
                }
            }
        }

        // Check collision with player, to figure out if we can start pickpocketing
        if (collision.gameObject.CompareTag("Player") && isTalking)
        {
            // Check if the player is close enough to start pickpocketing
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);  

            if (distanceToPlayer < pickpocketingDistanceThreshold)
            {
                triggerEntered = true;    
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggerEntered = false;
            playerStats.isPickpocketing = false;
        }
    }
}

