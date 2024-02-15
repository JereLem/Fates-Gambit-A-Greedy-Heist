using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAction : MonoBehaviour
{
    // 1. RayCast
    public Transform player;
    Camera cam;
    RaycastHit2D hit;
    public LayerMask GrapplingObj;

    // 2. LineRenderer
    LineRenderer lr;
    bool OnGrappling = false;
    Vector3 spot;
    public float moveSpeed= 1f;

    // 3. Spring Joint
    //SpringJoint2D sj;

    private void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RopeShoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndShoot();
        }

        if (OnGrappling && Input.GetMouseButton(1))
        {
            QuickMove();
        }

        DrawRope();
    }

    void RopeShoot()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Control the distance between camera
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(mousePosition);

        // Detect the collision between player's position and mouse's using Raycast
        hit = Physics2D.Raycast(player.position, mouseWorldPosition - (Vector2)player.position, 100f, GrapplingObj);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                Debug.Log("Wall!");
                OnGrappling = true;
                lr.positionCount = 2;
                lr.SetPosition(0, this.transform.position);
                lr.SetPosition(1, hit.point);
            }
        }
    }

    void EndShoot()
    {
        OnGrappling = false;
        lr.positionCount = 0;
    }

    void DrawRope()
    {
        if (OnGrappling)
        {
            lr.SetPosition(0, this.transform.position);
        }
    }

    void QuickMove()
    {
        if (hit.collider != null)
        {
            StartCoroutine(MovePlayerSmoothly(hit.point));
        }
    }

    IEnumerator MovePlayerSmoothly(Vector2 targetPosition)
    {
        // 현재 위치와 목표 위치 간의 거리
        float distance = Vector2.Distance(player.position, targetPosition);
        // 이동에 걸리는 시간 계산 (거리에 따라 조절할 수 있음)
        float duration = distance / moveSpeed;

        // 시작 시간
        float startTime = Time.time;

        // 보간법을 사용하여 부드럽게 이동
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            player.position = Vector3.Lerp(player.position, targetPosition, t);
            yield return null;
        }

        // 목표 위치에 도달하면 이동 종료
        player.position = targetPosition;
        Debug.Log("Quick");
    }

}
