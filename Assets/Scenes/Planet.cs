using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Planet : MonoBehaviour
{

    public double mass;
    public double D;
    public double r;
    public double v;
    public double angle;

    public double scaleD = 1e-8;
    public double scaler = 2e-10;
    public double scaleTime = 864000;  // one day
    public double dt = 60;  // each minute update the location

    public double[] pos = new double[3]{0, 0, 0};
    public double[] vel = new double[3]{0, 0, 0};

    public Transform tf;
    public List<GameObject> others = new List<GameObject>();
    
    public List<double> trajectory = new List<double>();
    public double[] last = new double[3]{0, 0, 0};
    public LineRenderer line;

    public double last_time;
    public double cur_time;

    // add current loction to trajectory
    void add_trajectory(){
        trajectory.Add(pos[0]);
        trajectory.Add(pos[1]);
        trajectory.Add(pos[2]);
        last[0] = pos[0];
        last[1] = pos[1];
        last[2] = pos[2];
    }


    // Start is called before the first frame update
    void Start()
    {
        // Init Planet Info
        PlanetInfo pltinfo = GetComponent<PlanetInfo>();
        mass = pltinfo.mass;
        D = pltinfo.D;
        r = pltinfo.r;
        v = pltinfo.v;
        angle = pltinfo.angle;

        // size of sphere
        tf = GetComponent<Transform>();
        float d = (float)(D * scaleD);
        tf.localScale = new Vector3(d, d, d); 

        // position and vel of sphere
        pos[0] = r;
        tf.position = new Vector3((float)(pos[0] * scaler), (float)(pos[1] * scaler), (float)(pos[2] * scaler));

        double pi = Math.PI;
        vel[1] = Math.Sin(pi * angle / 180) * v;
        vel[2] = Math.Cos(pi * angle / 180) * v;
        
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.white;

        GameObject[] gs = GameObject.FindGameObjectsWithTag("gravity");
        foreach(GameObject g in gs){
            if(g.name == "Sun"){
                others.Add(g);
            }
        }        
        
        // init line drawing setting
        add_trajectory();
        line = gameObject.GetComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.positionCount = 0;
        line.startWidth = 0.01f;
        line.endWidth = (float)(scaleD * D * 1.1);
        line.startColor = Color.red;
        line.endColor = Color.red;
        line.useWorldSpace = true;

        last_time = Time.time;
        Debug.Log("[" + this.name + "]: d=" + d + ", pos=" + tf.position);
    }


    // calculate accelate speed
    double[] accelate(double[] p1, double[] p2, double m2){
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

    // once move during dt, cur_dt may less than dt if it is the last dis
    void once_move(double cur_dt){
        double[] a = new double[3]{0, 0, 0};
        for(int i = 0; i < others.Count; i++){
            GameObject obj = others[i];
            Star st = obj.GetComponent<Star>();
            double[] cur_a = accelate(pos, st.pos, st.mass);
            a[0] += cur_a[0];
            a[1] += cur_a[1];
            a[2] += cur_a[2];
        }

        // updata speed
        vel[0] = vel[0] + a[0] * cur_dt;
        vel[1] = vel[1] + a[1] * cur_dt;
        vel[2] = vel[2] + a[2] * cur_dt;
    
        // update position
        double item = 0.5f * cur_dt * cur_dt;
        pos[0] = pos[0] + vel[0] * cur_dt + a[0] * item;
        pos[1] = pos[1] + vel[1] * cur_dt + a[1] * item;
        pos[2] = pos[2] + vel[2] * cur_dt + a[2] * item;
    }


    // Update is called once per frame
    void Update()
    {

        // truely duration time
        cur_time = Time.time;
        double deltaTime = (cur_time - last_time) * scaleTime;
        last_time = cur_time;
        int move_time = (int)(Math.Floor(deltaTime / dt));
        double last_dt = deltaTime - move_time * dt;
        //Debug.Log("Duration Time=" + deltaTime + " scale="+ scaleTime + " move-time:" + move_time + " last_dt=" + last_dt);
        for(int i = 0; i < move_time; i++){
            once_move(dt);
        }
        if(last_dt > 0) once_move(last_dt);
        
        tf.position = new Vector3((float)(pos[0] * scaler), (float)(pos[1] * scaler), (float)(pos[2] * scaler));
        //cur_location = tf.position;

        // if cur location is enough far from last location, record loction
        double dist = Math.Sqrt(Math.Pow(pos[0] - last[0], 2.0) + 
                                Math.Pow(pos[1] - last[1], 2.0) + 
                                Math.Pow(pos[2] - last[2], 2.0));
        if(dist >= D) add_trajectory();
        
        // draw the trajectory
        int point_count = trajectory.Count / 3;
        line.positionCount = point_count;
        for (int i = 0; i < point_count; i++){
            float tx = (float)(trajectory[i*3] * scaler);
            float ty = (float)(trajectory[i*3+1] * scaler);
            float tz = (float)(trajectory[i*3+2] * scaler);
            line.SetPosition(i, new Vector3(tx, ty, tz));
            //Debug.Log("Draw: " + tx + "," + ty + "," + tz);
        }
        //Debug.Log("Time=" + Time.time);
    }
}
