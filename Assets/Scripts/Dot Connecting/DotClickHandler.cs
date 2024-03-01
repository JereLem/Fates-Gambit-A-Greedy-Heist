using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DotClickHandler : MonoBehaviour, IPointerClickHandler
{
    public LineDrawer lineDrawer;
    public DotManager dotManager;

    public void Awake()
    {
        lineDrawer = transform.parent.parent.GetComponentInChildren<LineDrawer>();
        dotManager = transform.parent.GetComponent<DotManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dotManager.isFirstClick) {
            Dot dot = GetComponent<Dot>();
            if (dot.dotType == DotType.Start){
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    bool dotSet = dotManager.SetPoint(this.GetComponent<Dot>());
                    lineDrawer.DrawPoint(Camera.main.ScreenToWorldPoint(eventData.position), dotSet);
                    if (dotSet)
                    {
                        this.GetComponent<Dot>().SetConnectedState();
                        dotManager.isFirstClick = false;
                    }
                }
            }
        }
        else 
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Dot dot = GetComponent<Dot>();
                if (dot.dotType == DotType.NonActive)
                    return;

                bool dotSet = dotManager.SetPoint(this.GetComponent<Dot>());
                lineDrawer.DrawPoint(Camera.main.ScreenToWorldPoint(eventData.position), dotSet);
                if (dotSet)
                {
                    this.GetComponent<Dot>().SetConnectedState();
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                bool dotDeleted = dotManager.RemovePoint(this.GetComponent<Dot>());
                lineDrawer.ErasePoint();
                if (dotDeleted)
                {
                    this.GetComponent<Dot>().SetConnectedState();
                    if(lineDrawer.lineRenderer.positionCount == 0)
                    {
                        dotManager.isFirstClick = true;
                    }
                }
            }
        }
    }
}
