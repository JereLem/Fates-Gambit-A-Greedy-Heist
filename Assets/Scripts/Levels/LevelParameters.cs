using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelParameters : MonoBehaviour
{
    [Header("Level parameters")]
    [SerializeField] public int levelNumber;
    [SerializeField] public string levelName;
    [SerializeField] public float timerDuration;
    [SerializeField] public int targetValue;


}
