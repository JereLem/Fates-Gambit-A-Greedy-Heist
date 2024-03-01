using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{   

    [Header("Start and Endpoint")]
    [SerializeField] protected Vector2 startPoint;
    [SerializeField] protected Vector2 endPoint;
    public bool movingToEndPoint = true;
    public SpriteRenderer spriteRenderer;

    [Header("Cycles")]
    [SerializeField] public int completedCycles;
    [SerializeField] public int maxCycles = 0;

    [Header("Speeds")]
    [SerializeField] public float originalMoveSpeed;
    [SerializeField] public float moveSpeed;

    [Header("Min & Max Values")]
    const float minMoveSpeed = 0.5f;
    const float maxMoveSpeed = 1.0f;
    const int minCyclesAmmount = 1;
    const int maxCyclesAmmount = 4;

    private bool initialFacingRight = true;


    // Start is called before the first frame update
    protected void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeStartPosition();
        InitializeMovementParameters();
        initialFacingRight = spriteRenderer.flipX;
    }

    protected void InitializeStartPosition()
    {
        bool startFromLeft = Random.Range(0, 2) == 0;
        transform.position = startFromLeft ? startPoint : endPoint;
        completedCycles = 0;
    }

    protected void InitializeMovementParameters()
    {
        maxCycles = Random.Range(minCyclesAmmount, maxCyclesAmmount);
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        originalMoveSpeed = moveSpeed;
    }


    // Movement of the NPC
    protected void Move()
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

    public virtual void FlipSprite()
    {
        // Check the spawn point of the NPC
        if (movingToEndPoint)
        {
            spriteRenderer.flipX = !initialFacingRight;
        }
        else
        {
            spriteRenderer.flipX = initialFacingRight;
        }
    }

    // Method to stop movement
    public void StopMovement()
    {
        // Set moveSpeed to zero to stop movement
        moveSpeed = 0f;
    }

    public float GetSpeed()
    {
        // Set moveSpeed to zero to stop movement
        return moveSpeed;
    }

    // Method to resume movement
    public void ResumeMovement()
    {
        // Reset moveSpeed to the original value
        moveSpeed = originalMoveSpeed;
    }

    // Method to set max movement, used for removing the pedestrian after they have been pickpocketed
    public void SetMaxCycles()
    {
        completedCycles = maxCycles;
    }

    // Method to check if the NPC has gone all cycles, calls the NPCSpawner
    protected void CheckCycleCompletion()
    {
        if (completedCycles >= maxCycles)
        {
            NPCSpawner spawner = FindObjectOfType<NPCSpawner>();
            spawner?.NPCCompletedCycle(gameObject);
        }
    }
}
