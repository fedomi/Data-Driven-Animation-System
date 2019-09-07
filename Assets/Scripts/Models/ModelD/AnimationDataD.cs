using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimationDataD : MonoBehaviour
{
    #region Attributes

    public bool export = true;
    private Body b;
    private MotionStateD m;
    private List<MotionStateD> msl;
    public float currentFrame = 0;
    private float clipLength = 29000;
    private float frameToPlay;
    private Animator animator;
    private string clipName = "Locomotion";

    public List<Vector3> tp;   // Trajectory positions
    public List<Vector3> td;   // Trajectory directions

    private Vector3 previous_position;
    private int TRAJECTORY_WINDOW = 960;

    private string DataFile = "MotionData_018.txt";
    private string TrajectoriesFile = "TrajectoryTestData.txt";


    // Play animation with visuals
    public bool play = false;
    public bool applyPose = false;
    public bool drawTrajectoryLines = true;
    
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        b = new Body(transform);
        b.PrintState();
        msl = new List<MotionStateD>();
        Debug.Log("Collecting animation data...");
        previous_position = b.GetFlatPosition();
        InitializeTrajectoryQueue();
        
        if (play) ImportData();
    }

    private void FixedUpdate()
    {
        //DrawTrajectory(GenerateTrajectory());

        if (currentFrame < clipLength)
        {
            if (!applyPose)
            {
                frameToPlay = currentFrame / clipLength;
                animator.Play(clipName, 0, frameToPlay); // Frame is played

                currentFrame++;
                m = new MotionStateD(b);

                //Debug.DrawLine(b.GetHips().position, b.GetHips().position + new Vector3(m.GetQ().x, 0, m.GetQ().y), Color.green);
                //Debug.Log("Position: " + m.GetP().x + "," + m.GetP().y);
            }
            else
            {
                currentFrame++;
                //m = new MotionStateD(b);

                b.ApplyPose(msl[(int)currentFrame].GetJ());
            }

            if (!play)
            {
                UpdateTrajectoryQueue();


                //Debug.Log(msl.Count + " motion states.");
                //m.PrintMotionStateD();
                msl.Add(m);
                AddTrajectoryInfo();

            }
            else
            {
                Debug.Log("Drawing frame " + currentFrame);
                DrawDebugLines(msl[(int)currentFrame]);
            }

        }
        else
        {
            if (export)
            {
                export = false;
                SetPhaseValues();
                ExportData();
                
            }

        }

    }



    private void InitializeTrajectoryQueue() {
        tp = new List<Vector3>();
        td = new List<Vector3>();
        /*for (int i = 0; i < TRAJECTORY_WINDOW; i++) {
            //tp.Add(new Vector3(0,0,0));
            tp.Add(b.GetFlatPosition());
            td.Add(b.GetHips().forward);
        }*/
    }


    // Adds the trajectory info to one of the past frames
    private void AddTrajectoryInfo() {
        //int half = TRAJECTORY_WINDOW / 2;
        int half = 600;
        


        if (currentFrame >= 2*half)
        {
            Vector3 pivotDirection = td[(int)currentFrame - half];
            Vector3 pivotPosition = tp[(int)currentFrame - half];
            List<Vector2> mstp = new List<Vector2>();   // List in which trajectory points will be stored for this motion state, stored as (angle, distance)

            for (int i = -10; i < 10; i++)
            {
                Vector3 point = tp[(int)currentFrame - half + i * 60];
                //Vector2 point2d = new Vector2(point.x, point.z);
                //Vector2 direction = point2d - pivotPosition;

                //float angle = Vector2.SignedAngle(pivotDirection, direction);
                //float distance = direction.magnitude;
                Vector2 parametricPoint = PointToAngleAndDistance(point, pivotPosition, pivotDirection);
                mstp.Add(parametricPoint);

            }

            //msl[(int)currentFrame - half].AddTrajectories(new List<Vector3>(tp));
            msl[(int)currentFrame - half].AddTrajectories(mstp);

        }
    }

    // Dequeues oldest frame trajectory info on list and adds the current one
    private void UpdateTrajectoryQueue()
    {
        //tp.Dequeue();
        td.Add(new Vector3(m.GetQ().x, 0, m.GetQ().y));
        tp.Add(b.GetFlatPosition());
        previous_position = b.GetFlatPosition();
        
        if (tp.Count > 500) {
            for (int i = 1; i < 500; i++) {
                Debug.DrawLine(tp[tp.Count - i], tp[tp.Count - i - 1], Color.black);
                if (i % 50 == 0) Debug.DrawLine(tp[tp.Count - i], tp[tp.Count - i] + td[tp.Count - i], Color.red);
            }
        }

    }

    private void DrawDebugLines(MotionStateD m) {

        if (drawTrajectoryLines) {
            //Debug.DrawLine(transform.position, transform.position + transform.forward * 5, Color.green);

            Vector3 hipPos = new Vector3(m.GetP().x, 0, m.GetP().y);

            Vector3 fwd = new Vector3(m.GetQ().x, 0, m.GetQ().y);
            List<Vector3> tp = new List<Vector3>();
            List<Vector2> points = m.GetTP();
            /*
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(30,1));
            points.Add(new Vector2(60, 2));
            points.Add(new Vector2(90, 3));
            points.Add(new Vector2(120, 4));*/

            Vector3 hip_flat = new Vector3(hipPos.x, 0.2f, hipPos.z);
            //Debug.Log(MotionStateD.Vector2ListToString(tps));
            for (int i = 0; i < points.Count; i++)
            {
                tp.Add(AngleAndDistanceToPoint(points[i], hipPos, fwd));

                if (i > 0) Debug.DrawLine(tp[i], tp[i - 1], new Color(0, 0, 45 * i));
            }


        }


    }

    private Vector3 AngleAndDistanceToPoint(Vector2 p_info, Vector3 pivot, Vector3 fwd)
    {
        Vector3 point = pivot + fwd * p_info.y;

        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0, p_info.x, 0)) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    private Vector2 PointToAngleAndDistance(Vector3 point, Vector3 pivot, Vector3 forward) {
        Vector3 direction = point - pivot;
        float distance = direction.magnitude;
        
        float angle = Vector3.SignedAngle(forward, direction, Vector3.up);
        //Debug.Log("Angle between " + forward.ToString("F8") + " and " + direction.ToString("F8") + ": " + angle);


        return new Vector2(angle, distance);

    }

    Vector3 RotatePointAroundPivot(Vector3 pivot, Vector3 point, float a)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0, a, 0)) * dir; // rotate it
        point = dir + transform.position; // calculate rotated point
        return point; // return it
    }

    Vector2 GetPointInSpace(Vector2 pivot, Vector2 forward, float angle, float magnitude) {
        Vector3 dir = Quaternion.Euler(new Vector3(0, angle, 0)) * forward.normalized * magnitude;
        Vector2 dir2d = new Vector2(dir.x, dir.z);
        return dir2d + pivot;
       
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle, float magnitude)
    {
        Vector3 dir = (point - pivot).normalized; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0,angle,0)) * dir * (magnitude); // rotate it
        point = dir + pivot; // calculate rotated point
        return new Vector3(point.x, 0.2f, point.z); // return it
    }

    private void DrawTrajectory(List<Vector3> t) {
        for (int i = 0; i < t.Count - 1; i++)
        {
            //Vector3 start = hip_flat + tps[i];
            Vector3 start = new Vector3(t[i].x, 0.2f, t[i].z);

            Vector3 end = new Vector3(t[i + 1].x, 0.2f, t[i + 1].z);
            //Vector3 end = hip_flat + tps[i + 1];
            Debug.DrawLine(start, end, Color.red);
        }

    }

    private List<Vector3> GenerateTrajectory() {
        List<Vector3> t = new List<Vector3>();

        for (int i = 0; i < 50; i++) {
            t.Add(new Vector3(i*0.1f, 0, 0));
        }

        return t;
    }

    /*
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
        Gizmos.DrawSphere(position, 0.1f);
    }*/

    private void ExportData() {

        Debug.Log("Exporting " + msl.Count + " motion states.");
        // Export motion data
        using (StreamWriter file =
            new StreamWriter(@"Assets\AnimationFiles\" + DataFile, true))
        {
            foreach (MotionStateD m in msl)
            {
                //string line = m.GetP() + "," + m.GetQ() + "," + MotionStateD.Vector3ListToString(m.GetJ()) + "," + MotionStateD.Vector3ListToString(m.GetH());
                string line = m.ExportString();
                file.WriteLine(line);
                
            }
            Debug.Log("Motion data export completed.");
            file.Close();    
        }
        play = true;
        currentFrame = 10;
        /*
        // Export trajectory data (for testing the network)
        using (StreamWriter file =
            new StreamWriter(@"Assets\AnimationFiles\" + TrajectoriesFile, true))
        {
            foreach (MotionStateD m in msl)
            {
                string line = MotionStateD.ExportVector2ListAsString(m.GetTP());
                if(line.Length > 0)
                    file.WriteLine(line);
                
            }
            Debug.Log("Trajectory data export completed.");
            file.Close();
            play = true;
            currentFrame = 10;
        }*/
    }

    private void ImportData() {
        // Read file using StreamReader. Reads file line by line  
        using (StreamReader file = new StreamReader(@"Assets\AnimationFiles\" + DataFile))
        {
            int counter = 0;
            string ln;

            while ((ln = file.ReadLine()) != null)
            {
                string[] line = ln.Split(',');
                if(line.Length >= 98)
                {
                    MotionStateD ms = new MotionStateD(line);

                    msl.Add(ms);

                    //Debug.Log("Parsing frame " + counter);
                    counter++;
                }
                
            }
            file.Close();
            Console.WriteLine("File has lines.");
        }

    }

    void OnDrawGizmos()
    {
        if(m != null)
        {
            if (m.GetLeftFootContact())
            {
                // Draw a yellow sphere at the transform's position
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(b.GetLeftFoot().position, 0.1f);
            }

            if (m.GetRightFootContact())
            {
                // Draw a yellow sphere at the transform's position
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(b.GetRightFoot().position, 0.1f);
            }
        }
        

    }


    private void SetPhaseValues() {
        float currentPhase = 0;
        bool rightContactLastFrame = false;
        bool leftContactLastFrame = false;
        int framesSinceLastUpdate = 0;

        Debug.Log("Calculating Phase values...");

        for(int i = 0; i < msl.Count; i++)
        {
            if (msl[i].GetRightFootContact())
            {
                if (!rightContactLastFrame)
                {
                    UpdateLastFramesPhase(i, framesSinceLastUpdate, 2 * Mathf.PI);
                    rightContactLastFrame = true;
                    framesSinceLastUpdate = 0;
                }
                

            }
            else {
                rightContactLastFrame = false;
            }

            if (msl[i].GetLeftFootContact()) {
                if (!leftContactLastFrame)
                {
                    UpdateLastFramesPhase(i, framesSinceLastUpdate, Mathf.PI);
                    leftContactLastFrame = true;
                    framesSinceLastUpdate = 0;
                }
                
            }
            else
            {
                leftContactLastFrame = false;
            }


            framesSinceLastUpdate++;

        }

    }

    private void UpdateLastFramesPhase(int currentFrame, int n, float phase) {
        float step = Mathf.PI / n;

        for (int i = 0; i < n; i++) {
            msl[currentFrame - i].SetPhase(phase - i*step);
        }

    }
}




