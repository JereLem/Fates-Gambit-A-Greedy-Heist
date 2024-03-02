using UnityEngine;
using UnityEngine.UI;

public class BirdMovement : MonoBehaviour
{
    public float birdDistance;
    public float minRange;
    public float maxRange;

    private float birdSpeed = 3f;
    private float imageWidth;
    private Image birdImage;

    private void Awake()
    {
        birdImage = GetComponent<Image>();
        imageWidth = birdImage.sprite.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos;
        float newY = transform.position.y + Random.Range(minRange, maxRange);

        if (transform.position.x + imageWidth / 2 < 0)
        {
            if (transform.position.y > 0 || transform.position.y < 0)
                            newY = Screen.height / 2 + Random.Range(-30, 30);
                
                        newPos = new Vector2(Screen.width + imageWidth, newY);
        }
        else
            newPos = new Vector2(transform.position.x - birdSpeed / birdDistance, newY);

        transform.position = newPos;
    }
}
