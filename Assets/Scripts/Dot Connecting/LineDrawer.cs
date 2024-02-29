using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Net;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public List<Dot> dots = new List<Dot>();


    public RectTransform pointA;
    public RectTransform pointB;

    public Vector2 screenPointA;
    public Vector2 screenPointB;

    private void Start()
    {
        // Start dotType�� ��ǥ���� �����ͼ� �ֱ� 
        foreach (Dot dot in dots)
        {
            if (dot.dotType == DotType.Start)
            {
                pointA = dot.GetComponent<RectTransform>();
                Debug.Log("find start point");
                break; // �������� ã�����Ƿ� ���� ����
            }
        }
        screenPointA = RectTransformUtility.WorldToScreenPoint(null, pointA.position);
        lineRenderer.SetPosition(0, screenPointA);
    }

    public void SetPoint(Vector3 worldPos)
    {
        screenPointB = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        lineRenderer.positionCount++; 
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, screenPointB); 


        Debug.Log("screen A B is " + screenPointA + screenPointB);
    }


}
