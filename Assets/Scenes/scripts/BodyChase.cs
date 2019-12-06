using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyChase : MonoBehaviour
{

    public GameObject chaseObject;
    Transform tF;
    BodyMotion chaseBM;
    Transform chaseTF;

    // Start is called before the first frame update
    void Start()
    {
        tF = GetComponent<Transform>();
        chaseTF = chaseObject.GetComponent<Transform>();
        chaseBM = chaseObject.GetComponent<BodyMotion>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = chaseTF.position;
        double offset = chaseBM.bP.D * chaseBM.dS.scaleD * 10;
        double orbitR = (double)(pos.magnitude);
        float ratio = (float)(1.0 + offset / orbitR);

        Vector3 target = pos * ratio;
        target.y += (float)(offset / 10);
        tF.position = target;
        tF.LookAt(chaseTF);
    }
}
