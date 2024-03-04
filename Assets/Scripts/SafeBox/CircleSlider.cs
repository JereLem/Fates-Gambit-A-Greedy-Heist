using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSlider : MonoBehaviour
{
    [SerializeField] Transform handle;
    [SerializeField] Image fill;
    [SerializeField] Text valTxt;

    [Header("Components for Game Success")]
    [SerializeField] float targetAngle; 
    [SerializeField] AudioClip successSound; 
    [SerializeField] AudioSource audioSource;
    [SerializeField] bool isMissionCompleted = false; 

    Vector3 mousePos;

    private void Awake()
    {
        targetAngle = Random.Range(0f, 360f);
        fill.fillAmount = 0;
    }


    public void onHandleDrag()
    {
        mousePos = Input.mousePosition;
        Vector2 dir = mousePos - handle.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle < 0) ? 360 + angle : angle;

        // Set angle to clockwise direction
        angle = 360 - angle;

        fill.fillAmount = angle / 360f;
        valTxt.text = Mathf.RoundToInt(fill.fillAmount * 100).ToString();

        if (Mathf.Abs(angle - targetAngle) < 5f)
        {
            if (!isMissionCompleted)
            {
                MissionCompleted();
            }
        }
        else
        {
            isMissionCompleted = false; 
        }

        //Debug.Log("drag");
    }

    void MissionCompleted()
    {
        isMissionCompleted = true;
        Debug.Log("Mission Completed!");
        audioSource.PlayOneShot(successSound); // ÂûÄ¬ ¼Ò¸® Àç»ý
    }
}
