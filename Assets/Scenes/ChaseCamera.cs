using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{

    public GameObject planet_obj;
    public Planet planet;
    public Transform planet_tf;
    public Transform tf;
    
    // Start is called before the first frame update
    void Start()
    {
        string planet_name = this.name.Remove(0, 6);
        //Debug.Log("Camera-Planet-Name:" + planet_name);
        planet_obj = GameObject.Find(planet_name);
        planet = planet_obj.GetComponent<Planet>();
        planet_tf = planet_obj.GetComponent<Transform>();
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 planet_position = planet_tf.position;
        double offset = planet.D * planet.scaleD * 10;
        //double offset = 1.5;
        double orbit_r = (double)planet_position.magnitude;
        float ratio = (float)(1.0 + offset / orbit_r);

        Vector3 camera_position = planet_position * ratio;
        camera_position.y += (float)(offset / 10);
        tf.position = camera_position;
        tf.LookAt(planet_tf);
    }
}
