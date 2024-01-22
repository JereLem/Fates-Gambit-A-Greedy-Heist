using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float catchDistance = 1f;
    public Vector2 startPoint;
    public Vector2 endPoint;

    private bool movingToEndPoint = true;
    private Transform player;

    public int completedCycles = 0;
    public int maxCycles = 0;

    void Start()
    {
        bool startFromLeft = Random.Range(0, 2) == 0;
        transform.position = startFromLeft ? startPoint : endPoint;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        maxCycles = Random.Range(1, 4);
        moveSpeed = Random.Range(1f, 3f);
    }

    void Update()
    {
        Move();

        // Check if the player is within catchDistance
        if (Vector2.Distance(transform.position, player.position) < catchDistance)
        {
            Debug.Log("Player in catching distance!");
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
}

