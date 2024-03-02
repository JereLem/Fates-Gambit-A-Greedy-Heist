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

    private int f = 0;
    private int birdFrameYChange = 60;

    private void Awake()
    {
        birdImage = GetComponent<Image>();
        imageWidth = birdImage.sprite.rect.width;
    }

    void Update()
    {
        Vector2 newPos;
        float newY = transform.position.y + Random.Range(minRange, maxRange);

        if (f % birdFrameYChange != 0)
            newY = transform.position.y;
        
        if (transform.position.x + imageWidth / 2 < 0)
        {
            if (transform.position.y > 0 || transform.position.y < 0)
                            newY = Screen.height / 2 + Random.Range(-100, 100);
                
                        newPos = new Vector2(Screen.width + imageWidth, newY);
        }
        else
            newPos = new Vector2(transform.position.x - birdSpeed / birdDistance, newY);

        transform.position = newPos;
        f = (f + 1) % birdFrameYChange;
    }
}
