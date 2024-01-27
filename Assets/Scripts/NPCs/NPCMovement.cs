using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{   
    // Start & End Points
    [SerializeField] public Vector2 startPoint;
    [SerializeField] public Vector2 endPoint;

    // Current & Max Cycles
    [SerializeField] public int completedCycles;
    [SerializeField] public int maxCycles = 0;

    // Default moving speed & flag to reset the direction
    public float originalMoveSpeed;
    public float moveSpeed;
    private bool movingToEndPoint = true;

    // Start is called before the first frame update
    void Start()
    {
        bool startFromLeft = Random.Range(0, 2) == 0;
        transform.position = startFromLeft ? startPoint : endPoint;
        completedCycles = 0;
        maxCycles = Random.Range(1, 4);
        moveSpeed = Random.Range(1f, 3f);
        originalMoveSpeed = moveSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    // Movement of the NPC
    void Move()
    {
        // Add some randomness that it is more realistic. Some walk faster, some slower
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

    // Method to stop movement
    public void StopMovement()
    {
        // Set moveSpeed to zero to stop movement
        moveSpeed = 0f;
    }

    // Method to resume movement
    public void ResumeMovement()
    {
        // Reset moveSpeed to the original value
        moveSpeed = originalMoveSpeed;
    }


    // Method to check if the NPC has gone all cycles, calls the NPCSpawner
    void CheckCycleCompletion()
    {
        if (completedCycles >= maxCycles)
        {
            NPCSpawner spawner = FindObjectOfType<NPCSpawner>();
            spawner?.NPCCompletedCycle(gameObject);
        }
    }
}
