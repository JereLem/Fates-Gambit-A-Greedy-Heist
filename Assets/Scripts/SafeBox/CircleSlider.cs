using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSlider : MonoBehaviour
{
    [SerializeField] Transform handle;
    [SerializeField] Image knob;
    [SerializeField] Text valTxt;

    [Header("Components for Game Success")]
    [SerializeField] float targetAngle;
    [SerializeField] float toleranceAngle;
    [SerializeField] bool isMissionCompleted = false; 

    Vector3 mousePos;

    private void Awake()
    {
        targetAngle = Random.Range(0f, 360f);
    }


    public void onHandleDrag()
    {
        mousePos = Input.mousePosition;
        Vector2 dir = mousePos - handle.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle < 0) ? 360 + angle : angle;

        // Set angle to clockwise direction
        angle = (angle < 0) ? 360 + angle : angle;

        knob.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Check if spining sound is already playing
        //if (!AudioManager.instance.IsSFXPlaying("knob_spin"))
        //{
        //    Debug.Log("knob_spin");
        //    // Play spinning sound only when the mission is not completed and no spinning sound is playing
        //    AudioManager.instance.PlaySFX("knob_spin");
        //}

        if (Mathf.Abs(angle - targetAngle) < toleranceAngle)
        {
            if (!isMissionCompleted)
            {
                SetMissionCompleted();
            }
        }
        else
        {
            isMissionCompleted = false; 
        }
    }

    public void SetMissionCompleted()
    {
        isMissionCompleted = true;
        Debug.Log("Mission Completed!");
        AudioManager.instance.PlaySFX("knob_unlock");
    }

    public bool IsMissionCompleted()
    {
        return isMissionCompleted;
    }
}
