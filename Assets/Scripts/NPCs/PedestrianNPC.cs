using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


public class PedestrianNPC : NPCMovement
{
    [Header("Pedestrian Variables")]
    public Sprite[] sprites;
    SpriteRenderer spriteRendererPed;
    [SerializeField] public GameObject pedestrianTrigger;
    Color pickpocketedColor;
    public float talkDuration = 5f;
    public bool isTalking = false;
    public int pickpocketableValue;

    // Chances
    public int pedestrianTypeChance;
    public int policeAlertChance;
    public float conversationChanceThreshold = 0.25f;

    // NPCs currently talking and max ammount that can talk
    public int currentTalkingNPCs = 0;
    public int maxTalkingNPCs = 6;

    // Player
    public Transform player;
    public PlayerStats playerStats;
    

    // List to store all talking pedestrians
    public static List<PedestrianNPC> talkingNPCs = new List<PedestrianNPC>();

    [Header("Pickpocketing Flags")]
    private const KeyCode pickpocketKey = KeyCode.E;
    public float pickpocketingDistanceThreshold = 2f;
    public bool hasBeenPickpocketed = false;

    // Extra flag to check the player can start pickpocketing the pedestrian
    public bool triggerEntered = false;

    [Header("UI & Icons")]
    public Transform panel;
    public Color originalColor;
    public Color grayedOutColor;
    public Image highlightPickpocket;
    [SerializeField] public GameObject highlightObject; 
    [SerializeField] public GameObject talkingIcon;
    public GameObject TalkingIcons;
    public GameObject clone;


    
    // Animator
    public Animator animator;
    public Animator playerAnimator;

    new void Start()
    {

        highlightPickpocket = GameObject.Find("HighlightPickpocket").GetComponent<Image>();
        grayedOutColor = Color.gray;
        originalColor = Color.white;
        highlightPickpocket.color = grayedOutColor;
        
        // Set male or female sprite
        spriteRendererPed = GetComponent<SpriteRenderer>();
        spriteRendererPed.sprite = sprites[Random.Range(0,sprites.Length)];

        GetComponent<Animator>().runtimeAnimatorController = animator.runtimeAnimatorController;

        //Pedestrian trigger
        pedestrianTrigger = transform.GetChild(0).gameObject;

        // Get player transform and stats
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        playerAnimator = player.GetComponent<Animator>();
        
        // 90% chance the pedestrian is default type 10% rich 
        pedestrianTypeChance = Random.Range(1, 11);

        if (pedestrianTypeChance <= 1)
        {
            animator.SetBool("isRich",true);
            pickpocketableValue = Random.Range(20, 25);
        }
        else
        {
            pickpocketableValue = Random.Range(10, 15);
        }

        // Randomize layer of sprites, making sure it's different from the player's level
        SetRandomSortingOrder();

        // Folder for talking icons
        TalkingIcons = GameObject.FindGameObjectWithTag("TalkingIcons");

        base.Start();

    }


    void SetRandomSortingOrder()
    {
        // Set a random sorting order
        int randomSortingOrder = Random.Range(2, 8);

        // Check if it's the same as the player's level, regenerate if needed
        while (randomSortingOrder == player.GetComponent<SpriteRenderer>().sortingOrder)
        {
            randomSortingOrder = Random.Range(2, 8);
        }

        // Set the sorting order for the pedestrian sprite
        spriteRendererPed.sortingOrder = randomSortingOrder;
    }

    void Update()
    {
        Move();
        FlipSprite();
        // Pickpocketing is activated by pressing E, and player has to be near 2 talking pedestrians
        if (Input.GetKeyDown(pickpocketKey) && triggerEntered)
        {
            StartPickpocketing();
            playerAnimator.SetBool("isPickpocketing",true);

        }

        else if(hasBeenPickpocketed)
        {
            spriteRendererPed.color = grayedOutColor;
            SetMaxCycles();
        }
    }

    public void StartPickpocketing()
    {
        if (!hasBeenPickpocketed)
        {
            playerStats.isPickpocketing = true;
        }
    }

    public void StopPickpocketing()
    {
        playerStats.isPickpocketing = false;
        playerAnimator.SetBool("isPickpocketing",false);
    }

    // Pedestrians start to talk, and will talk a set duration
    public IEnumerator StartTalking()
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

}

