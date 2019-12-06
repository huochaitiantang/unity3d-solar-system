using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BodyMotion : MonoBehaviour
{
    public double[] pos = new double[3]{0, 0, 0}; // position of body
    public double[] vel = new double[3]{0, 0, 0}; // velocity of body
    double t0;
    double t1;

    public DisplaySetting dS;
    public BodyProperty bP;
    Transform tF;
    BodyMotion cBM = null;  // central body motion
    public List<GameObject> others = new List<GameObject>();
    
    public bool trajEnable = false;     // show trajectory?
    public int maxTrajCount = 2000;     // max trajectory count
    public int lastTrajIndex;              // last trajectory index
    public List<double[]> traj = new List<double[]>(); // trajectory array
    LineRenderer line;


    // set position of body
    void SetPosition(){
        double x = pos[0] * dS.scaleR;
        double y = pos[1] * dS.scaleR;
        double z = pos[2] * dS.scaleR;
        if(cBM != null){
            x += cBM.pos[0] * dS.scaleR;
            y += cBM.pos[1] * dS.scaleR;
            z += cBM.pos[2] * dS.scaleR;
        }
        tF.position = new Vector3((float)x, (float)y, (float)z);
    }


    // calculate the acceleration from m2
    double[] Acceleration(double[] p1, double[] p2, double m2){
        double dx = p2[0] - p1[0];
        double dy = p2[1] - p1[1];
        double dz = p2[2] - p1[2];
        double dis_3 = Math.Pow(dx * dx + dy * dy + dz * dz, 1.5);

        double[] a = new double[3];
        double Gm2 = 6.67e-11 * m2;
        a[0] = Gm2 * dx / dis_3;
        a[1] = Gm2 * dy / dis_3;
        a[2] = Gm2 * dz / dis_3;
        return a;
    }


    // once motion during dt, dt may less than dS.dt if
    void MoveOnce(double dt){
        double[] gravityPos = new double[3]{0, 0, 0};
        double[] a = Acceleration(pos, gravityPos, cBM.bP.M);
        // other gravities
        /*
        for(int i = 0; i < others.Count; i++){
            BodyMotion bM = others[i].GetComponent<BodyMotion>();
            gravityPos[0] = bM.pos[0] - cBM.pos[0];
            gravityPos[1] = bM.pos[1] - cBM.pos[1];
            gravityPos[2] = bM.pos[2] - cBM.pos[2];
            double[] cur_a = Acceleration(pos, gravityPos, bM.bP.M);
            a[0] += cur_a[0];
            a[1] += cur_a[1];
            a[2] += cur_a[2];
        }
        */
        // updata velocity
        vel[0] = vel[0] + a[0] * dt;
        vel[1] = vel[1] + a[1] * dt;
        vel[2] = vel[2] + a[2] * dt;
    
        // update position, only consider central body, add the position of central body
        double item = 0.5f * dt * dt;
        pos[0] = pos[0] + vel[0] * dt + a[0] * item;
        pos[1] = pos[1] + vel[1] * dt + a[1] * item;
        pos[2] = pos[2] + vel[2] * dt + a[2] * item;
    }


    // add current position to trajectory array
    void AddTrajectory(){
        if(traj.Count < maxTrajCount){
            traj.Add(new double[3]{pos[0], pos[1], pos[2]});
            lastTrajIndex = traj.Count - 1;
            //Debug.Log("add x=" + traj[lastTrajIndex][0] + " y=" + traj[lastTrajIndex][1] + " z=" + traj[lastTrajIndex][2]);
        }
        // if the array is full, save by a loop method
        else{
            lastTrajIndex = (lastTrajIndex + 1) % maxTrajCount;
            traj[lastTrajIndex][0] = pos[0];
            traj[lastTrajIndex][1] = pos[1];
            traj[lastTrajIndex][2] = pos[2];
        }
    }


    // draw current trajectory array
    void DrawTrajectory(){
        int j = 0;
        //Debug.Log("Draw Traj " + this.name + " count="+traj.Count);
        line.positionCount = traj.Count;
        for(int i = lastTrajIndex; i >= 0; i--, j++){
            float x = (float)((cBM.pos[0] + traj[i][0]) * dS.scaleR);
            float y = (float)((cBM.pos[1] + traj[i][1]) * dS.scaleR);
            float z = (float)((cBM.pos[2] + traj[i][2]) * dS.scaleR);
            line.SetPosition(j, new Vector3(x, y, z));
        }
        // process loop part
        if(j < traj.Count){
            for(int i = traj.Count - 1; i > lastTrajIndex; i--, j++){
                float x = (float)((cBM.pos[0] + traj[i][0]) * dS.scaleR);
                float y = (float)((cBM.pos[1] + traj[i][1]) * dS.scaleR);
                float z = (float)((cBM.pos[2] + traj[i][2]) * dS.scaleR);
                line.SetPosition(j, new Vector3(x, y, z));
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // get useful classes
        tF = GetComponent<Transform>();
        dS = GameObject.Find("Setting").GetComponent<DisplaySetting>();
        bP = GetComponent<BodyProperty>();
        BodyOrbit bO = GetComponent<BodyOrbit>();
        if(bO.centralBody != null) 
            cBM = bO.centralBody.GetComponent<BodyMotion>();

        // set display size of body
        float d = (float)(bP.D * dS.scaleD);
        tF.localScale = new Vector3(d, d, d); 

        // set the initial position of body, 
        // initial position is relative to the central body
        pos[0] = bO.R;
        /*
        if(cBM != null){
            pos[0] += cBM.pos[0];
            pos[1] += cBM.pos[1];
            pos[2] += cBM.pos[2];
        }
        */
        SetPosition();
        Debug.Log(this.name + " size=" + tF.localScale + " pos=" + tF.position);

        // set the initial velocity of revolution
        // initial velocity if relative to the central body
        double pi = Math.PI;
        vel[1] = Math.Sin(pi * bO.angle / 180) * bO.V;
        vel[2] = Math.Cos(pi * bO.angle / 180) * bO.V;
        /*
        if(cBM != null){
            vel[0] += cBM.vel[0];
            vel[1] += cBM.vel[1];
            vel[2] += cBM.vel[2];
        }
        */
        t0 = Time.time;

        // init trajectory and line drawing setting
        if(trajEnable){
            AddTrajectory();
            line = gameObject.GetComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.positionCount = 0;
            line.startWidth = (float)(dS.scaleD * bP.D * 1.05);
            line.endWidth = 0.01f;
            line.startColor = Color.red;
            line.endColor = Color.red;
            line.useWorldSpace = true;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(cBM != null){
            // true duration time
            t1 = Time.time;
            double deltaTime = (t1 - t0) * dS.scaleT;
            t0 = t1;
            int moveCount = (int)(Math.Floor(deltaTime / dS.dt));
            double tailDt = deltaTime - moveCount * dS.dt;

            // move the position based on the gravity of central body
            for(int i = 0; i < moveCount; i++) MoveOnce(dS.dt);
            if(tailDt > 0) MoveOnce(tailDt);
            SetPosition();
            
            // add new trajectory if satisfy condition and draw current trajectory array
            if(trajEnable){
                double dist = Math.Sqrt(Math.Pow(pos[0] - traj[lastTrajIndex][0], 2.0) + 
                                        Math.Pow(pos[1] - traj[lastTrajIndex][1], 2.0) + 
                                        Math.Pow(pos[2] - traj[lastTrajIndex][2], 2.0));
                if(dist >= bP.D) AddTrajectory();
                DrawTrajectory();
            }
        }

    }
}
