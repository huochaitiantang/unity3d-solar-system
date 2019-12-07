using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BodyChase : MonoBehaviour
{

    public double field = 5;
    public GameObject chaseObject;
    public GameObject referObject;
    Transform tF;
    BodyMotion chaseBM;
    Transform chaseTF;
    Transform referTF;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        tF = GetComponent<Transform>();
        chaseTF = chaseObject.GetComponent<Transform>();
        chaseBM = chaseObject.GetComponent<BodyMotion>();
        referTF = referObject.GetComponent<Transform>();
        camera = GetComponent<Camera>();  
        camera.fieldOfView = (float)(2 * Math.Atan(0.1 * field) * 180 / Math.PI); // for offset = 5D = 10R
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = chaseTF.position - referTF.position;
        double offset = chaseBM.bP.D * chaseBM.dS.scaleD * 5;
        double orbitR = (double)(pos.magnitude);
        float ratio = (float)(1.0 + offset / orbitR);

        Vector3 target = pos * ratio + referTF.position;
        target.y += (float)(offset / 8);
        tF.position = target;
        //tF.LookAt(referTF);
        tF.LookAt(chaseTF);



    }
}
