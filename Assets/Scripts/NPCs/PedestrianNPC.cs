using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianNPC : MonoBehaviour
{
    // Pedestrian variables
    public float talkDuration = 5f;
    private bool isTalking = false;
    public int pickpocketableValue;

    // Chances
    private int pedestrianTypeChance;
    private int policeAlertChance;

    // NPCs currently talking and max ammount that can talk
    private int currentTalkingNPCs = 0;
    private int maxTalkingNPCs = 6;

    // Player
    private Transform player;
    private PlayerStats playerStats;

    // List to store all pedestrians
    private static List<PedestrianNPC> talkingNPCs = new List<PedestrianNPC>();

    // NPC movement
    private NPCMovement npcMovement;
    

    void Start()
    {
        // Get player transform and stats
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        
        // Adjust pickpocketableValue
        // 90% chance the pedestrian is default type 10% rich 
        pedestrianTypeChance = Random.Range(1, 11);
        pickpocketableValue = (pedestrianTypeChance <= 9) ? Random.Range(10, 15) : Random.Range(20, 25);

        // Get movement script
        npcMovement = GetComponent<NPCMovement>();

    }


    // If two pedestrians collide they might start talking 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PedestrianNPC") && !isTalking)
        {
            PedestrianNPC otherNPC = collision.gameObject.GetComponent<PedestrianNPC>();

            if (!otherNPC.isTalking && !talkingNPCs.Contains(this) && !talkingNPCs.Contains(otherNPC)
                && currentTalkingNPCs < maxTalkingNPCs - 1 && otherNPC.currentTalkingNPCs < maxTalkingNPCs - 1)
            {
                // Introduce a random chance for the conversation to start
                float randomChance = Random.Range(0.0f, 1.0f);
                float conversationChanceThreshold = 0.25f;

                if (randomChance < conversationChanceThreshold)
                {
                    StartCoroutine(StartTalking());

                    // Stop NPCMovement when they start to talk
                    npcMovement.StopMovement();


                    otherNPC.StartCoroutine(otherNPC.StartTalking());
                    otherNPC.npcMovement.StopMovement();

                    currentTalkingNPCs++;
                    otherNPC.currentTalkingNPCs++;

                    talkingNPCs.Add(this);
                    talkingNPCs.Add(otherNPC);
                }
            }
        }
    }

    // Pedestrians start to talk, and will talk a set duration
    IEnumerator StartTalking()
    {
        if (!isTalking)
        {
            isTalking = true;
            yield return new WaitForSeconds(talkDuration);

            // Resume movement after NPCs have stop talking
            npcMovement.ResumeMovement();

            currentTalkingNPCs--;

            if (talkingNPCs.Contains(this))
            {
                talkingNPCs.Remove(this);
            }
            isTalking = false;
        }
    }
}
