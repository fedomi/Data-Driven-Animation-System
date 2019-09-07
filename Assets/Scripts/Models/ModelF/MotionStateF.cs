using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionStateF
{
    private float phase;
    private Vector2 p;  // 2D Character position
    private Vector2 q;  // 2D character facing direction
    private Vector2 rv; // 2D Root translational velocity
    private float deltaQ; // Change of facing direction resp. to previous motion state
    private List<Vector3> j;    // Joint positions
    private List<Vector3> h;    // Joint facing directions
    private List<Quaternion> r; // Joint rotations
    private List<Vector3> jv;   // Joint velocities
    private List<Vector2> tp;   // Trajectory positions
    private List<float> td;   // Trajectory facing directions
    private Transform hips;

    private int N_JOINTS = 19;  // Number of joints
    private int N_TPOINTS = 20; // Number of trajectory points

    private bool leftFootContact = false;
    private bool rightFootContact = false;
    private bool standing = false;

    public Vector3 leftFootPosition;
    public Vector3 rightFootPosition;

    public MotionStateF() { }

    
    public MotionStateF(string[] data) {
        j = new List<Vector3>();
        tp = new List<Vector2>();
        td = new List<float>();
        r = new List<Quaternion>();
        
        int C_INDEX = 0;    // Index in which character info starts
        

        if (float.Parse(data[0]) == 0.0f)
        {
            C_INDEX = 1;
        }
        int R_INDEX = C_INDEX + 3 * N_JOINTS;
        int T_INDEX = R_INDEX + 4 * N_JOINTS;
        int TD_INDEX = T_INDEX + 2 * N_TPOINTS;


        for (int i = 0; i < N_JOINTS; i++)
        {
            j.Add(new Vector3(float.Parse(data[C_INDEX + 3 * i]), float.Parse(data[C_INDEX + 3 * i + 1]), float.Parse(data[C_INDEX + 3 * i + 2])));
        }

        for (int i = 0; i < N_JOINTS; i++)
        {
            r.Add(new Quaternion(float.Parse(data[R_INDEX + 4 * i]), float.Parse(data[R_INDEX + 4 * i + 1]), float.Parse(data[R_INDEX + 4 * i + 2]), float.Parse(data[R_INDEX + 4 * i + 3])));
        }

        for (int i = 0; i < N_TPOINTS; i++)
        {
            tp.Add(new Vector2(float.Parse(data[T_INDEX + 2 * i]), float.Parse(data[T_INDEX + 2 * i + 1])));

        }

        for (int i = 0; i < N_TPOINTS; i++) {
            td.Add(float.Parse(data[TD_INDEX + i]));
        }
        rv = new Vector2(float.Parse(data[data.Length - 3]), float.Parse(data[data.Length - 2]));
        phase = float.Parse(data[data.Length - 1]);
    }

    public MotionStateF(Body b)
    {

        //j = new List<Vector3>();
        h = new List<Vector3>();
        tp = new List<Vector2>();
        td = new List<float>();

        hips = b.GetHips();
        p = new Vector2(hips.position.x, hips.position.z);
        //q = new Vector2(hips.localEulerAngles.x, hips.localEulerAngles.z).normalized;
        q = new Vector2(hips.forward.x, hips.forward.z);
        float angle = Vector3.SignedAngle(new Vector3(1, 0, 0), new Vector3(q.x,0,q.y), Vector3.up);
        Debug.DrawLine(hips.position, hips.position + new Vector3(q.x, 0, q.y), Color.green);
        Debug.DrawLine(hips.position, hips.position + new Vector3(1, 0, 0), Color.red);

        //Debug.Log("Hips forward: " + q.x + " - " + q.y);
        //Debug.Log("Angle: " + angle);

        j = ExtractLocalPositions(b, -angle);
        r = ExtractLocalRotations(b);
        //List<Transform> joints = b.GetJoints();

        leftFootPosition = b.GetLeftFoot().position;
        float leftFootHeight = leftFootPosition.y;

        rightFootPosition = b.GetRightFoot().position;
        float rightFootHeight = rightFootPosition.y;
        float FOOT_HEIGHT_CONTACT = 0.08f;
        //Debug.Log("Distance between feet: " + Vector3.Distance(leftFootPosition, rightFootPosition));
        if (leftFootHeight < FOOT_HEIGHT_CONTACT && rightFootHeight >= FOOT_HEIGHT_CONTACT)
        {
            //Debug.Log("LEFT FOOT STEP");
            leftFootContact = true;
        }
        else if (rightFootHeight < FOOT_HEIGHT_CONTACT && leftFootHeight >= FOOT_HEIGHT_CONTACT)
        {
            //Debug.Log("RIGHT FOOT CONTACT");
            rightFootContact = true;
        }
        else if (rightFootHeight < FOOT_HEIGHT_CONTACT && leftFootHeight < FOOT_HEIGHT_CONTACT && Vector3.Distance(leftFootPosition, rightFootPosition) < 0.25f)
        {
            Debug.Log("Standing motion.");
            leftFootContact = true;
            rightFootContact = true;
        }
        else if (rightFootHeight < FOOT_HEIGHT_CONTACT && leftFootHeight < FOOT_HEIGHT_CONTACT) {
            leftFootContact = true;
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
    public List<Vector2> GetTP() { return tp; }
    public List<float> GetTD() { return td; }

    public Vector2 GetRV() { return rv; }
    public void SetRV(Vector2 root_velocity) { rv = root_velocity; }

    public void SetP(Vector2 v) { p = v; }
    public void SetQ(Vector2 v) { q = v; }

    public float GetPhase() { return phase; }
    public void SetPhase(float newPhase) { phase = newPhase; }

    public bool GetLeftFootContact() { return leftFootContact; }
    public bool GetRightFootContact() { return rightFootContact; }
    public bool IsStanding() { return standing; }
    public void SetStanding(bool s) { standing = s; }


    public void SetTP(List<Vector2> trajectoryPoints) { tp = trajectoryPoints; }

    public void SetTD(List<float> trajectoryDirections) { td = trajectoryDirections; }

    public void SetRotations(List<Quaternion> nr) { r = nr; }

    public void AddPose(string[] data)
    {
        j = new List<Vector3>();
        r = new List<Quaternion>();


        int C_INDEX = 0;    // Index in which character info starts
        int R_INDEX = C_INDEX + 3 * N_JOINTS;

        /*
        if (float.Parse(data[0]) == 0.0f)
        {
            C_INDEX = 1;
        }
        int T_INDEX = C_INDEX + 3 * N_JOINTS;*/

        //Debug.Log("Adding pose info with " + data.Length + " numbers and from index " + C_INDEX);
        for (int i = 0; i < N_JOINTS; i++)
        {
            j.Add(new Vector3(float.Parse(data[C_INDEX + 3 * i]), float.Parse(data[C_INDEX + 3 * i + 1]), float.Parse(data[C_INDEX + 3 * i + 2])));
        }

        for (int i = 0; i < N_JOINTS; i++)
        {
            r.Add(new Quaternion(float.Parse(data[R_INDEX + 4 * i]), float.Parse(data[R_INDEX + 4 * i + 1]), float.Parse(data[R_INDEX + 4 * i + 2]), float.Parse(data[R_INDEX + 4 * i + 3])));
        }

        int l = data.Length;
        float angle = float.Parse(data[l-3]);
        float dist = float.Parse(data[l - 2]);
        float dAngle = float.Parse(data[l - 1]);
        rv = new Vector2(angle,dist);
        deltaQ = dAngle;
    }


    /*
     * Receives a list of trajectory points stored as pairs (angle,distance)**/
    public void AddTrajectories(List<Vector2> t_positions, List<float> t_directions) {

        if (t_positions.Count == t_directions.Count)
        {
            for (int i = 0; i < t_positions.Count; i++)
            {
                tp.Add(t_positions[i]);
                td.Add(t_directions[i]);
            }
        }
        else Debug.LogError("Trajectory vectors length not matching!");
                

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

    /* This is the method that is used to export motion state info as a vector of numbers */
    public string ExportString() {

        //PrintMotionState();
        string s = "0.0" +  /*p.x + "," + p.y +
            "," + q.x + "," + q.y +*/
            ExportVector3ListAsString(j) +
            ExportQuaternionListAsString(r) +
            ExportVector2ListAsString(tp) + 
            ExportFloatListAsString(td) +
            "," +
            rv.x + "," + rv.y + "," + deltaQ + "," + 
            ((standing) ? 1 : 0) + "," +
            phase; /*+
            ExportVector3ListAsString(h) +
            ExportVector3ListAsString(tp);*/
        //Debug.Log(s.Split(',').Length);
        return s;

    }

    public void SetDeltaQ(float dq) { deltaQ = dq; }
    public float GetDeltaQ() { return deltaQ; }


    private string ExportGaitLabels() {
        if (standing) return "1";
        else return "0";
    }

    Vector3 RotatePointAroundPivot(Vector3 pivot, Vector3 point, float a)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0, a, 0)) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }



    #region Feature extraction methods

    public List<Quaternion> ExtractLocalRotations(Body b) {
        List<Quaternion> res = new List<Quaternion>();

        Vector3 local;

        local = b.GetLeftUpLeg().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));  // LeftUpLeg

        local = b.GetLeftLeg().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));  // Left Leg

        local = b.GetLeftFoot().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));  // Left foot

        local = b.GetRightUpLeg().localPosition;
        res.Add(ExponentialMap.Map(local));  // LeftUpLeg


        local = b.GetRightLeg().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));  // Left Leg

        local = b.GetRightFoot().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));  // Left foot

        local = b.GetSpine().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetSpine1().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetSpine2().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));


        local = b.GetLeftShoulder().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetLeftArm().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));


        local = b.GetLeftForeArm().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetLeftHand().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetNeck().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetHead().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetRightShoulder().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetRightArm().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetRightForeArm().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));

        local = b.GetRightHand().localRotation.eulerAngles;
        res.Add(ExponentialMap.Map(local));


        return res;
    }

    public List<Vector3> ExtractLocalPositions(Body b) {
        List<Vector3> res = new List<Vector3>();
        Vector3 local;

        //res.Add(b.GetArmature().position); // armature

        //local = b.GetHips().position;
        //res.Add(local); //hips

        local = b.GetLeftUpLeg().localPosition;
        //local = b.GetLeftUpLeg().position - b.GetHips().position;
        res.Add(local);  // LeftUpLeg

        local = b.GetLeftLeg().localPosition;
        //local = b.GetLeftLeg().position - b.GetLeftUpLeg().position;
        res.Add(local);  // Left Leg

        local = b.GetLeftFoot().localPosition;
        //local = b.GetLeftFoot().position - b.GetLeftLeg().position;
        res.Add(local);  // Left foot

        local = b.GetRightUpLeg().localPosition;
        //local = b.GetRightUpLeg().position - b.GetHips().position;
        res.Add(local);  // LeftUpLeg


        local = b.GetRightLeg().localPosition;
        //local = b.GetRightLeg().position - b.GetRightUpLeg().position;
        res.Add(local);  // Left Leg

        local = b.GetRightFoot().localPosition;
        //local = b.GetRightFoot().position - b.GetRightLeg().position;
        res.Add(local);  // Left foot

        local = b.GetSpine().localPosition;
        //local = b.GetSpine().position - b.GetHips().position;
        res.Add(local);

        local = b.GetSpine1().localPosition;
        //local = b.GetSpine1().position - b.GetSpine().position;
        res.Add(local);

        local = b.GetSpine2().localPosition;
        //local = b.GetSpine2().position - b.GetSpine1().position;
        res.Add(local);


        local = b.GetLeftShoulder().localPosition;
        //local = b.GetLeftShoulder().position - b.GetSpine2().position;
        res.Add(local);

        local = b.GetLeftArm().localPosition;
        //local = b.GetLeftArm().position - b.GetLeftShoulder().position;
        res.Add(local);


        local = b.GetLeftForeArm().localPosition;
        //local = b.GetLeftForeArm().position - b.GetLeftArm().position;
        res.Add(local);

        local = b.GetLeftHand().localPosition;
        //local = b.GetLeftHand().position - b.GetLeftForeArm().position;
        res.Add(local);

        local = b.GetNeck().localPosition;
        //local = b.GetNeck().position - b.GetSpine2().position;
        res.Add(local);

        local = b.GetHead().localPosition;
        //local = b.GetHead().position - b.GetNeck().position;
        res.Add(local);

        local = b.GetRightShoulder().localPosition;
        //local = b.GetRightShoulder().position - b.GetSpine2().position;
        res.Add(local);

        local = b.GetRightArm().localPosition;
        //local = b.GetRightArm().position - b.GetRightShoulder().position;
        res.Add(local);

        local = b.GetRightForeArm().localPosition;
        //local = b.GetRightForeArm().position - b.GetRightArm().position;
        res.Add(local);

        local = b.GetRightHand().localPosition;
        //local = b.GetRightHand().position - b.GetRightForeArm().position;
        res.Add(local);


        return res;
    }
    
    public List<Vector3> ExtractLocalPositions(Body b, float angle)
    {
        List<Vector3> res = new List<Vector3>();
        Vector3 rotated;
        Vector3 local;
        Vector3 root = b.GetHips().position;
        //res.Add(b.GetArmature().position); // armature

        //local = b.GetHips().position;
        //res.Add(local); //hips


        //local = b.GetLeftUpLeg().localPosition;
        rotated = RotatePointAroundPivot(root, b.GetLeftUpLeg().position, angle);
        local = rotated - root;
        res.Add(local);  // LeftUpLeg

        //local = b.GetLeftLeg().localPosition;
        //local = b.GetLeftLeg().position - b.GetLeftUpLeg().position;
        rotated = RotatePointAroundPivot(root, b.GetLeftLeg().position, angle);
        local = rotated - root;
        res.Add(local);  // Left Leg

        //local = b.GetLeftFoot().localPosition;
        //local = b.GetLeftFoot().position - b.GetLeftLeg().position;
        rotated = RotatePointAroundPivot(root, b.GetLeftFoot().position, angle);
        local = rotated - root;
        res.Add(local);  // Left foot

        //local = b.GetRightUpLeg().localPosition;
        //local = b.GetRightUpLeg().position - b.GetHips().position;
        rotated = RotatePointAroundPivot(root, b.GetRightUpLeg().position, angle);
        local = rotated - root;
        res.Add(local);  // LeftUpLeg


        //local = b.GetRightLeg().localPosition;
        //local = b.GetRightLeg().position - b.GetRightUpLeg().position;
        rotated = RotatePointAroundPivot(root, b.GetRightLeg().position, angle);
        local = rotated - root;
        res.Add(local);  // Left Leg

        //local = b.GetRightFoot().localPosition;
        //local = b.GetRightFoot().position - b.GetRightLeg().position;
        rotated = RotatePointAroundPivot(root, b.GetRightFoot().position, angle);
        local = rotated - root;
        res.Add(local);  // Left foot

        //local = b.GetSpine().localPosition;
        //local = b.GetSpine().position - b.GetHips().position;
        rotated = RotatePointAroundPivot(root, b.GetSpine().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetSpine1().localPosition;
        //local = b.GetSpine1().position - b.GetSpine().position;
        rotated = RotatePointAroundPivot(root, b.GetSpine1().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetSpine2().localPosition;
        //local = b.GetSpine2().position - b.GetSpine1().position;
        rotated = RotatePointAroundPivot(root, b.GetSpine2().position, angle);
        local = rotated - root;
        res.Add(local);


        //local = b.GetLeftShoulder().localPosition;
        //local = b.GetLeftShoulder().position - b.GetSpine2().position;
        rotated = RotatePointAroundPivot(root, b.GetLeftShoulder().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetLeftArm().localPosition;
        //local = b.GetLeftArm().position - b.GetLeftShoulder().position;
        rotated = RotatePointAroundPivot(root, b.GetLeftArm().position, angle);
        local = rotated - root;
        res.Add(local);


        //local = b.GetLeftForeArm().localPosition;
        //local = b.GetLeftForeArm().position - b.GetLeftArm().position;
        rotated = RotatePointAroundPivot(root, b.GetLeftForeArm().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetLeftHand().localPosition;
        //local = b.GetLeftHand().position - b.GetLeftForeArm().position;
        rotated = RotatePointAroundPivot(root, b.GetLeftHand().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetNeck().localPosition;
        //local = b.GetNeck().position - b.GetSpine2().position;
        rotated = RotatePointAroundPivot(root, b.GetNeck().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetHead().localPosition;
        //local = b.GetHead().position - b.GetNeck().position;
        rotated = RotatePointAroundPivot(root, b.GetHead().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetRightShoulder().localPosition;
        //local = b.GetRightShoulder().position - b.GetSpine2().position;
        rotated = RotatePointAroundPivot(root, b.GetRightShoulder().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetRightArm().localPosition;
        //local = b.GetRightArm().position - b.GetRightShoulder().position;
        rotated = RotatePointAroundPivot(root, b.GetRightArm().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetRightForeArm().localPosition;
        //local = b.GetRightForeArm().position - b.GetRightArm().position;
        rotated = RotatePointAroundPivot(root, b.GetRightForeArm().position, angle);
        local = rotated - root;
        res.Add(local);

        //local = b.GetRightHand().localPosition;
        //local = b.GetRightHand().position - b.GetRightForeArm().position;
        rotated = RotatePointAroundPivot(root, b.GetRightHand().position, angle);
        local = rotated - root;
        res.Add(local);


        return res;
    }
    #endregion



    #region String export methods
    // Turns List<Vector3> into a string in readable format
    static public string Vector3ListToString(List<Vector3> l)
    {
        return String.Join("",
             new List<Vector3>(l)
             .ConvertAll(i => i.ToString("F8"))
             .ToArray());
    }

    static public string Vector2ListToString(List<Vector2> l)
    {
        return String.Join("",
             new List<Vector2>(l)
             .ConvertAll(i => i.ToString("F8"))
             .ToArray());
    }

    static public string ExportFloatListAsString(List<float> f) {
        return String.Join("",
             new List<float>(f)
             .ConvertAll(i => "," + i.ToString("F12"))
             .ToArray());
    }

    static public string ExportQuaternionListAsString(List<Quaternion> l)
    {
        return String.Join("",
             new List<Quaternion>(l)
             .ConvertAll(i => "," + i.x.ToString("F12") + "," + i.y.ToString("F12") + "," + i.z.ToString("F12") + "," + i.w.ToString("F12"))
             .ToArray());
    }

    static public string ExportVector3ListAsString(List<Vector3> l)
    {
        return String.Join("",
             new List<Vector3>(l)
             .ConvertAll(i => "," + i.x.ToString("F12") + "," + i.y.ToString("F12") + "," + i.z.ToString("F12"))
             .ToArray());
    }

    static public string ExportVector2ListAsString(List<Vector2> l)
    {
        return String.Join("",
             new List<Vector2>(l)
             .ConvertAll(i => "," + i.x.ToString("F12") + "," + i.y.ToString("F12"))
             .ToArray());
    }
    #endregion


}
