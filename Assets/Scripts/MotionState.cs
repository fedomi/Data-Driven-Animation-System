using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionState
{
    private float phase;
    private Vector2 p;  // 2D Character position
    private Vector2 q;  // 2D character facing direction
    private List<Vector3> j;    // Joint positions
    private List<Vector3> h;    // Joint facing directions
    private List<Vector3> tp;   // Trajectory positions
    private List<Vector3> td;   // Trajectory facing directions
    private Transform hips;

    private int N_JOINTS = 19;  // Number of joints
    private int N_TPOINTS = 20; // Number of trajectory points

    private bool leftFootContact = false;
    private bool rightFootContact = false;

    public MotionState(string[] data) {
        j = new List<Vector3>();
        tp = new List<Vector3>();

        
        int C_INDEX = 0;    // Index in which character info starts
        

        if (float.Parse(data[0]) == 0.0f)
        {
            C_INDEX = 1;
        }
        int T_INDEX = C_INDEX + 3 * N_JOINTS;
        Debug.Log("Building motion state with " + data.Length + "numbers");
        /*
        int J_INDEX = C_INDEX + 4;
        int T_INDEX = 3 * N_JOINTS + J_INDEX; // Index of data[] vector in which trajectory points start

        p = new Vector2(float.Parse(data[C_INDEX]), float.Parse(data[C_INDEX + 1]));
        q = new Vector2(float.Parse(data[C_INDEX + 2]), float.Parse(data[C_INDEX + 3]));

        for (int i = 0; i < N_JOINTS; i++) {
            j.Add(new Vector3(float.Parse(data[J_INDEX + 3*i]), float.Parse(data[J_INDEX + 3*i + 1]), float.Parse(data[J_INDEX + 3*i + 2])));
        }

        for (int i = 0; i < N_TPOINTS; i++)
        {
            tp.Add(new Vector3(float.Parse(data[T_INDEX + 3*i]), float.Parse(data[T_INDEX + 3*i + 1]), float.Parse(data[T_INDEX + 3*i + 2])));
            
        }
        */

        for (int i = 0; i < N_JOINTS; i++)
        {
            j.Add(new Vector3(float.Parse(data[C_INDEX + 3 * i]), float.Parse(data[C_INDEX + 3 * i + 1]), float.Parse(data[C_INDEX + 3 * i + 2])));
        }

        for (int i = 0; i < N_TPOINTS; i++)
        {
            tp.Add(new Vector3(float.Parse(data[T_INDEX + 3 * i]), float.Parse(data[T_INDEX + 3 * i + 1]), float.Parse(data[T_INDEX + 3 * i + 2])));

        }

    }

    public MotionState(Body b)
    {

        //j = new List<Vector3>();
        h = new List<Vector3>();
        tp = new List<Vector3>();

        hips = b.GetHips();
        p = new Vector2(hips.position.x, hips.position.z);
        q = new Vector2(hips.localEulerAngles.x, hips.localEulerAngles.z).normalized;

        j = ExtractLocalPositions(b);
        //List<Transform> joints = b.GetJoints();
        if (b.GetLeftFoot().position.y < 0.1)
        {
            //Debug.Log("LEFT FOOT STEP");
            leftFootContact = true;
        }
        else if (b.GetRightFoot().position.y < 0.1)
        {
            //Debug.Log("RIGHT FOOT STEP");
            rightFootContact = true;
        }
        //Debug.Log(b.GetLeftFoot().position.y.ToString("F4"));
        /*
        foreach (Transform t in joints)
        {
            j.Add(t.position);
            //h.Add(t.eulerAngles);
        }*/
    }

    public Vector2 GetP() { return p; }
    public Vector2 GetQ() { return q; }
    public List<Vector3> GetJ() { return j; }
    public List<Vector3> GetH() { return h; }
    public List<Vector3> GetTP() { return tp; }

    public float GetPhase() { return phase; }
    public void SetPhase(float newPhase) { phase = newPhase; }

    public bool GetLeftFootContact() { return leftFootContact; }
    public bool GetRightFootContact() { return rightFootContact; }


    /*
     * Receives a list of trajectory points stored as world positions
     * Calculates local positions related to this motion state hip position**/
    public void AddTrajectories(List<Vector3> t_positions) {
        Vector3 hip_flat = new Vector3(p.x, 0, p.y);

        foreach (Vector3 t in t_positions) {
            tp.Add(t - hip_flat);
        }


    }

    public void PrintMotionState()
    {
        Debug.Log("--------------------");
        Debug.Log("p:" + p.ToString("F8"));
        Debug.Log("q:" + q.ToString("F8"));
        Debug.Log("j:" + Vector3ListToString(j));
        //Debug.Log("h:" + Vector3ListToString(h));
        Debug.Log("--------------------");
    }

    public string ExportString() {

        //PrintMotionState();
        string s = "0.0" +  /*p.x + "," + p.y +
            "," + q.x + "," + q.y +*/
            ExportVector3ListAsString(j) +
            ExportVector3ListAsString(tp);/* + 
            "," + phase;*/ /*+
            ExportVector3ListAsString(h) +
            ExportVector3ListAsString(tp);*/
        Debug.Log(s.Split(',').Length);
        return s;

    }

    // Turns List<Vector3> into a string in readable format
    static public string Vector3ListToString(List<Vector3> l)
    {
        return String.Join("",
             new List<Vector3>(l)
             .ConvertAll(i => i.ToString("F8"))
             .ToArray());
    }

    static public string ExportVector3ListAsString(List<Vector3> l)
    {
        return String.Join("",
             new List<Vector3>(l)
             .ConvertAll(i => "," + i.x + "," + i.y + "," + i.z)
             .ToArray());
    }

    public List<Vector3> ExtractLocalPositions(Body b) {
        List<Vector3> res = new List<Vector3>();
        Vector3 local;

        //res.Add(b.GetArmature().position); // armature

        //local = b.GetHips().position;
        //res.Add(local); //hips

        local = b.GetLeftUpLeg().position - b.GetHips().position;
        res.Add(local);  // LeftUpLeg

        local = b.GetLeftLeg().position - b.GetLeftUpLeg().position;
        res.Add(local);  // Left Leg

        local = b.GetLeftFoot().position - b.GetLeftLeg().position;
        res.Add(local);  // Left foot


        local = b.GetRightUpLeg().position - b.GetHips().position;
        res.Add(local);  // LeftUpLeg

        local = b.GetRightLeg().position - b.GetRightUpLeg().position;
        res.Add(local);  // Left Leg

        local = b.GetRightFoot().position - b.GetRightLeg().position;
        res.Add(local);  // Left foot

        local = b.GetSpine().position - b.GetHips().position;
        res.Add(local);

        local = b.GetSpine1().position - b.GetSpine().position;
        res.Add(local);

        local = b.GetSpine2().position - b.GetSpine1().position;
        res.Add(local);

        local = b.GetLeftShoulder().position - b.GetSpine2().position;
        res.Add(local);

        local = b.GetLeftArm().position - b.GetLeftShoulder().position;
        res.Add(local);

        local = b.GetLeftForeArm().position - b.GetLeftArm().position;
        res.Add(local);

        local = b.GetLeftHand().position - b.GetLeftForeArm().position;
        res.Add(local);

        local = b.GetNeck().position - b.GetSpine2().position;
        res.Add(local);

        local = b.GetHead().position - b.GetNeck().position;
        res.Add(local);

        local = b.GetRightShoulder().position - b.GetSpine2().position;
        res.Add(local);

        local = b.GetRightArm().position - b.GetRightShoulder().position;
        res.Add(local);

        local = b.GetRightForeArm().position - b.GetRightArm().position;
        res.Add(local);

        local = b.GetRightHand().position - b.GetRightForeArm().position;
        res.Add(local);


        return res;
    }
}
