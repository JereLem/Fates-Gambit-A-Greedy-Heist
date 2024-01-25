using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float catchDistance = 0.5f;
    public float catchDelay = 2.0f;
    private bool isCatching = false;
    public Vector2 startPoint;
    public Vector2 endPoint;

    private bool movingToEndPoint = true;
    private Transform player;

    public int completedCycles = 0;
    public int maxCycles = 0;

    private PlayerStats playerStats;

    private int policeRank;

    void Start()
    {
        bool startFromLeft = Random.Range(0, 2) == 0;
        transform.position = startFromLeft ? startPoint : endPoint;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        maxCycles = Random.Range(1, 4);
        moveSpeed = Random.Range(1f, 3f);
    }

    void Update()
    {
        Move();
    }


    void Move()
    {
        if(!isCatching)
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCatching)
        {
            Debug.Log("Player in catching distance! Starting catch...");
            StartCoroutine(CatchPlayerWithDelay());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player escaped!");
            // Reset any catching state
            isCatching = false;
            StopAllCoroutines(); // Stop ongoing catch coroutine
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

