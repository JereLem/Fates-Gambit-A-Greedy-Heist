using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianTrigger : MonoBehaviour
{
    // Reference to the parent PedestrianNPC
    public PedestrianNPC parentNPC;
    private PlayerStats playerStats;

    private void Start()
    {
        // Get the parent NPC script
        parentNPC = transform.parent.GetComponent<PedestrianNPC>();
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PedestrianTrigger"))
        {
            PedestrianTrigger otherTrigger = collision.gameObject.GetComponent<PedestrianTrigger>();

            if (!otherTrigger.parentNPC.isTalking && !parentNPC.isTalking &&
                !PedestrianNPC.talkingNPCs.Contains(parentNPC) && !PedestrianNPC.talkingNPCs.Contains(otherTrigger.parentNPC)
                && parentNPC.currentTalkingNPCs < parentNPC.maxTalkingNPCs - 1 && otherTrigger.parentNPC.currentTalkingNPCs < otherTrigger.parentNPC.maxTalkingNPCs - 1
                && parentNPC.movingToEndPoint != otherTrigger.parentNPC.movingToEndPoint && !parentNPC.hasBeenPickpocketed && !otherTrigger.parentNPC.hasBeenPickpocketed)
            {
                // Introduce a random chance for the conversation to start
                float randomChance = Random.Range(0.0f, 1.0f);

                if (randomChance < parentNPC.conversationChanceThreshold)
                {
                    parentNPC.StartCoroutine(parentNPC.StartTalking());
                    parentNPC.StopMovement();

                    otherTrigger.parentNPC.StartCoroutine(otherTrigger.parentNPC.StartTalking());
                    otherTrigger.parentNPC.StopMovement();

                    // Set talking icon by finding the midpoint between the two pedestrians
                    Vector3 midpoint = (parentNPC.transform.position + otherTrigger.parentNPC.transform.position) / 2f;

                    // Spawn the talkingIcon at the midpoint
                    parentNPC.clone = Instantiate(parentNPC.talkingIcon, new Vector3(midpoint.x, midpoint.y + 1.5f, midpoint.z), Quaternion.identity);
                    parentNPC.clone.transform.parent = parentNPC.TalkingIcons.transform;

                    parentNPC.currentTalkingNPCs++;
                    otherTrigger.parentNPC.currentTalkingNPCs++;

                    PedestrianNPC.talkingNPCs.Add(parentNPC);
                    PedestrianNPC.talkingNPCs.Add(otherTrigger.parentNPC);
                }
            }
        }

        // Check collision with player, to figure out if we can start pickpocketing
        if (collision.gameObject.CompareTag("Player") && parentNPC.isTalking && !parentNPC.hasBeenPickpocketed)
        {
            // Check if the player is close enough to start pickpocketing
            float distanceToPlayer = Vector2.Distance(parentNPC.transform.position, parentNPC.player.position);

            if (distanceToPlayer < parentNPC.pickpocketingDistanceThreshold)
            {
                parentNPC.triggerEntered = true;
                parentNPC.highlightPickpocket.color = parentNPC.originalColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && parentNPC.isTalking && !parentNPC.hasBeenPickpocketed)
        {
            parentNPC.triggerEntered = false;
            playerStats.isPickpocketing = false;
            parentNPC.highlightPickpocket.color = parentNPC.grayedOutColor;
            
            // Inform GameManager that the mini-game is no longer active
            GameManager.SetMiniGameActive(false);
            AudioManager.instance.StopMinigameMusic();
        }
    }
}
