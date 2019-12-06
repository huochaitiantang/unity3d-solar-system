using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySetting : MonoBehaviour
{

    // global variable, scale settings for display
    public double scaleD = 1e-8;    // scale times for diameter
    public double scaleR = 2e-10;   // scale times for distance
    public double scaleT = 864000;  // for accelate revolution, set 1 s = 10 day
    public double dt = 60;          // 60 s update the position

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
