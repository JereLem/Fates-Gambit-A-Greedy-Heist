using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianNPC : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float talkDuration = 5f;

    private bool movingToEndPoint = true;
    private Transform player;
    private bool isTalking = false;

    private int maxTalkingNPCs = 6;
    private int currentTalkingNPCs = 0;
    private static List<PedestrianNPC> talkingNPCs = new List<PedestrianNPC>();

    public int completedCycles = 0;
    public int maxCycles = 0;

    void Start()
    {
        bool startFromLeft = Random.Range(0, 2) == 0;
        transform.position = startFromLeft ? startPoint : endPoint;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        maxCycles = Random.Range(1, 4);
        moveSpeed = Random.Range(1f, 3f);
    }

    void Update()
    {
        if (!isTalking)
        {
            Move();
        }
    }

    void Move()
    {
        // Add some randomneess that its more realistic that some walk faster, some slower
        float step = moveSpeed * Time.deltaTime;
        Vector2 targetPoint = movingToEndPoint ? endPoint : startPoint;

        transform.position = Vector2.MoveTowards(transform.position, targetPoint, step);

        if (Vector2.Distance((Vector2)transform.position, targetPoint) < 0.01f)
        {
            movingToEndPoint = !movingToEndPoint;

            if (!movingToEndPoint)
            {
                completedCycles++;
                CheckCycleCompletion();
            }
        }
    }

    void CheckCycleCompletion()
    {
        if (completedCycles >= maxCycles)
        {
            NPCSpawner spawner = FindObjectOfType<NPCSpawner>();
            spawner?.NPCCompletedCycle(gameObject);
            completedCycles = 0;
        }
    }

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
                    otherNPC.StartCoroutine(otherNPC.StartTalking());

                    currentTalkingNPCs++;
                    otherNPC.currentTalkingNPCs++;

                    talkingNPCs.Add(this);
                    talkingNPCs.Add(otherNPC);
                }
            }
        }
    }

    IEnumerator StartTalking()
    {
        if (!isTalking)
        {
            isTalking = true;
            Debug.Log("NPC's talking!");
            yield return new WaitForSeconds(talkDuration);

            currentTalkingNPCs--;

            if (talkingNPCs.Contains(this))
            {
                talkingNPCs.Remove(this);
            }

            isTalking = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isTalking)
        {
            Debug.Log("Start pickpocketing?");
        }
    }
}
