using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineImage : MonoBehaviour
{
    private RectTransform imageRectTransform;
    public float lineWidth = 1.0f;
    public Vector3 pointA;
    public Vector3 pointB;

    void Start()
    {
        imageRectTransform = GetComponent<RectTransform>();
    }

    public void SetPoint(Vector3 point)
    {
        pointB = pointA;
        pointA = point;

        // 스크린 좌표로 설정
        pointA.z = Camera.main.nearClipPlane;
        pointB.z = Camera.main.nearClipPlane;

        Debug.Log("pointB, pointA is " + pointB + ", " + pointA);
        UpdateLine();
    }

    void UpdateLine()
    {
        Vector3 screenPointA = pointA;
        Vector3 screenPointB = pointB;

        Vector3 differenceVector = screenPointB - screenPointA;
        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        imageRectTransform.pivot = new Vector2(0, 0.5f);
        imageRectTransform.position = screenPointA;

        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

}

