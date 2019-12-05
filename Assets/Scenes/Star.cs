using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public double mass = 1.9891e30; // kg
    public double D = 1.392e9; // m
    public double scaleD = 1e-8;
    public double[] pos = new double[3]{0, 0, 0};    
    public Transform tf;


    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        float d = (float)(D * scaleD);
        tf.localScale = new Vector3(d, d, d); 
        tf.position = new Vector3((float)(pos[0]), (float)(pos[1]), (float)(pos[2]));

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.yellow;

        Debug.Log("[" + this.name + "]: d=" + d + ", pos=" + tf.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
