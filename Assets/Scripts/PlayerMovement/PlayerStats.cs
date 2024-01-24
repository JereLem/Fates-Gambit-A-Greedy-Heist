using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public int timesCaught; // Current times player has been caught
    public int maximumTimesCaught = 1; // Maximum times player can be caught

    public int pickpocketedValue;  // Inital value of pickpocketed items

    // Start is called before the first frame update
    void Start()
    {
        timesCaught = 0;
        pickpocketedValue = 0;
    }
}
