using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudMovement : MonoBehaviour
{
    public float cloudDistance;

    private float cloudSpeed = 1.0f;
    private float imageWidth;
    private Image cloudImage;

    private void Awake()
    {
        cloudImage = GetComponent<Image>();
        imageWidth = cloudImage.sprite.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos;

        if (transform.position.x - imageWidth/2 > Screen.width)
            newPos = new Vector2(0 - imageWidth, transform.position.y);
        else
            newPos = new Vector2(transform.position.x + cloudSpeed / cloudDistance, transform.position.y);

        transform.position = newPos;
    }
}
