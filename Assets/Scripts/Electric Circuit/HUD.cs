using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Text myText;
    Slider mySlider;

    public float totalTime = 10f;
    public float leftTime = 10f;
    public float normalizedTime;
    int min;
    int sec;

    private void Awake()
    {
        mySlider = GetComponent<Slider>();
        myText = GetComponentInChildren<Text>();
    }

    private void LateUpdate()
    {
        leftTime -= Time.deltaTime;
        normalizedTime = Mathf.Clamp01(leftTime / totalTime);
        min = Mathf.FloorToInt(leftTime / 60);
        sec = Mathf.FloorToInt(leftTime % 60);
        myText.text = string.Format("{0:D2} : {1:D2}", min, sec);
        mySlider.value = normalizedTime;

        if (leftTime <= 0)
        {
            DestroyMiniGame();
        }
    }
    void DestroyMiniGame()
    {
        // Inform GameManager that the mini-game is no longer active
        GameManager.SetMiniGameActive(false);

        // Destroy the mini-game object
        Destroy(transform.parent.gameObject);
    }
}
