using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Net;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public DotManager dotManager;

    public List<Dot> dots = new List<Dot>();

    public RectTransform pointA;
    public RectTransform pointB;

    public Vector2 screenPointA;
    public Vector2 screenPointB;

    private void Start()
    {
        dotManager = transform.parent.GetComponent<DotManager>();
        // find the Start type dot's coordinates
        //foreach (Dot dot in dots)
        //{
        //    if (dot.dotType == DotType.Start)
        //    {
        //        pointA = dot.GetComponent<RectTransform>();
        //        Debug.Log("find start point");
        //        Debug.Log("pointA: "+ pointA.position);
                
        //        //break; 
        //    }
        //}
        //screenPointA = RectTransformUtility.WorldToScreenPoint(null, pointA.position);

        //lineRenderer.SetPosition(0, screenPointA);
    }

    public void DrawPoint(Vector3 worldPos, bool isSet)
    {
        if (isSet)
        {
            screenPointB = RectTransformUtility.WorldToScreenPoint(null, worldPos);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, screenPointB);
        }
    }
    
    public bool ErasePoint()
    {
        if(lineRenderer.positionCount > 0)
        {
            lineRenderer.positionCount--;
            //Debug.Log("dot delete");
            return true;
        }
        else
        {
            //Debug.Log("there is no dot to delete");
            return false;
        }
    }

}
