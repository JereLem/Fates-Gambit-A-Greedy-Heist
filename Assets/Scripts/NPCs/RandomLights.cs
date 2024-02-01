using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLights : MonoBehaviour
{
    // Variables
    public GameObject lightObject;
    public Sprite lightOnSprite;   
    public Sprite lightOffSprite;

    private float toggleInterval = 2f;  // Interval in seconds
    private float lastToggleTime;
    
    // Start is called before the first frame update
    void Start()
    {
        lastToggleTime = Time.time;
        StartCoroutine(ToggleLights());
    }

    // Coroutine to toggle lights at random intervals
    IEnumerator ToggleLights()
    {
        while (true)
        {
            yield return new WaitForSeconds(toggleInterval);

            // Toggle lights randomly
            bool lightsOn = Random.value > 0.5f;

            // Set the sprite based on the lights state
            lightObject.GetComponent<SpriteRenderer>().sprite = lightsOn ? lightOnSprite : lightOffSprite;
        }
    }
}
