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
        // ���� ��ġ�� ��ǥ ��ġ ���� �Ÿ�
        float distance = Vector2.Distance(player.position, targetPosition);
        // �̵��� �ɸ��� �ð� ��� (�Ÿ��� ���� ������ �� ����)
        float duration = distance / moveSpeed;

        // ���� �ð�
        float startTime = Time.time;

        // �������� ����Ͽ� �ε巴�� �̵�
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            player.position = Vector3.Lerp(player.position, targetPosition, t);
            yield return null;
        }

        // ��ǥ ��ġ�� �����ϸ� �̵� ����
        player.position = targetPosition;
        Debug.Log("Quick");
    }

}
