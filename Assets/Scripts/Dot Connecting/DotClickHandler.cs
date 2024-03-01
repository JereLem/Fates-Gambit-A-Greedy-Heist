using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DotClickHandler : MonoBehaviour, IPointerClickHandler
{
    public LineDrawer lineDrawer;

    public void Awake()
    {
        lineDrawer = transform.parent.parent.GetComponentInChildren<LineDrawer>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        lineDrawer.SetPoint(Camera.main.ScreenToWorldPoint(eventData.position));
    }
}
