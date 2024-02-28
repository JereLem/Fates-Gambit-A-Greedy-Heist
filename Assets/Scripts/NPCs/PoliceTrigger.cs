using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceTrigger : MonoBehaviour
{
    public PoliceNPC parentNPC;
    // Start is called before the first frame update
    void Start()
    {
        // Get the parent NPC script
        parentNPC = transform.parent.GetComponent<PoliceNPC>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && (parentNPC.playerStats.isPickpocketing || parentNPC.isAlertActive))
        {
            Debug.Log("Trying to catch the player!");
            parentNPC.catchCoroutine = parentNPC.StartCoroutine(parentNPC.TryToCatchPlayer());
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            parentNPC.isCatching = false;
            // Check if the coroutine is running before trying to stop it
            if (parentNPC.catchCoroutine != null)
            {
                Debug.Log("Player escaped!");
                parentNPC.StopCoroutine(parentNPC.catchCoroutine); // Stop ongoing catch coroutine
                parentNPC.playerStats.isPickpocketing = false;
            }
        }

    }

}
