using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Dot : MonoBehaviour
{
    public bool isConnected = false;
    public bool isActivated = true;

    public Image image;
    public DotType dotType;

    private void Awake()
    {
        image = GetComponent<Image>();
        Debug.Log("dot position is " + GetComponent<RectTransform>().position);

    }

    private void Start()
    {
        if (dotType == DotType.Start)
        {
            image.color = Color.yellow;
        }
        if(dotType == DotType.End)
        {
            image.color = Color.green;
        }

        if (dotType == DotType.NonActive)
        {
            image.color = Color.gray;
        }
    }

    public void SetConnectedState()
    {
        isConnected = !isConnected;
        if (dotType == DotType.Active)
        {
            image.color = isConnected ? Color.red : Color.blue;
        }
        Debug.Log("set state: " + isConnected);
    }

}
