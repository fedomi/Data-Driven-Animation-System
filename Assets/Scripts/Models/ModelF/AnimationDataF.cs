using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimationDataF : MonoBehaviour
{
    #region Attributes

    public bool export = true;
    private Body b;
    private MotionStateF m;
    private List<MotionStateF> msl;
    public float currentFrame = 0;
    public float clipLength;
    private float frameToPlay;
    private Animator animator;
    private string clipName = "Locomotion";

    public List<Vector3> tp;   // Trajectory positions
    public List<Vector3> td;   // Trajectory directions

    private Vector3 previous_position;
    private int TRAJECTORY_WINDOW = 960;

    private string DataFile = "MotionData_Test2108.txt";
    private string TrajectoriesFile = "TrajectoryTestData.txt";

    private Vector3 prevHipPosition;
    private Vector3 prevForwardDirection;

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
        prevHipPosition = b.GetFlatPosition();
        prevForwardDirection = new Vector3(b.GetHips().forward.x, 0, b.GetHips().forward.z);
        msl = new List<MotionStateF>();
        Debug.Log("Collecting animation data...");
        previous_position = b.GetFlatPosition();

        InitializeTrajectoryQueue();
        
        if (play) ImportData();
    }

    private void Update()
    {
        //DrawTrajectory(GenerateTrajectory());

        if (currentFrame < clipLength - 3)
        {
            if (!applyPose)
            {
                frameToPlay = currentFrame / clipLength;
                animator.Play(clipName, 0, frameToPlay); // Frame is played

                currentFrame += 2;
                m = new MotionStateF(b);

                //Debug.DrawLine(b.GetHips().position, b.GetHips().position + new Vector3(m.GetQ().x, 0, m.GetQ().y), Color.green);
                //Debug.Log("Position: " + m.GetP().x + "," + m.GetP().y);
            }
            else
            {
                currentFrame += 2;
                //m = new MotionStateF(b);

                b.ApplyPose(msl[(int)currentFrame / 2].GetJ());
            }

            if (!play)
            {
                UpdateTrajectoryQueue();


                //Debug.Log(msl.Count + " motion states.");
                //m.PrintMotionStateF();
                
                // Root velocity
                Vector2 r_velocity = PointToAngleAndDistance(b.GetFlatPosition(), prevHipPosition, prevForwardDirection);
                m.SetRV(r_velocity);

                // Root facing direction change
                Vector3 flatFwd = new Vector3(b.GetHips().forward.x, 0, b.GetHips().forward.z);
                float deltaAngle = Vector3.SignedAngle(prevForwardDirection, flatFwd, Vector3.up);
                m.SetDeltaQ(deltaAngle);

                Debug.Log("Calculating root velocity with positions " + b.GetFlatPosition().ToString("F4") + " and " + prevHipPosition.ToString("F4") + ", and forward direction " + prevForwardDirection.ToString("F4")  );
                Debug.Log("Root velocity: " + r_velocity.x + " - " + r_velocity.y);
                Vector3 drawPosition = new Vector3(prevHipPosition.x, b.GetHips().position.y, prevHipPosition.z);
                Debug.DrawRay(drawPosition, prevForwardDirection, Color.magenta);

                //Debug.Break();
                //Debug.DrawLine(prevHipPosition, b.GetFlatPosition(), Color.magenta);
                prevHipPosition = b.GetFlatPosition();
                prevForwardDirection = new Vector3(b.GetHips().forward.x, 0, b.GetHips().forward.z);

                //if (r_velocity.x > 90) Debug.Break();

                msl.Add(m);
                AddTrajectoryInfo();

            }
            else
            {
                Debug.Log("Drawing frame " + currentFrame);
                DrawDebugLines(msl[(int)currentFrame / 2]);
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
        int half = 120;

        int index = (int)currentFrame / 2;

        if (index >= 2*half)
        {
            Vector3 pivotDirection = td[index - half];
            Vector3 pivotPosition = tp[index - half];
            List<Vector2> mstp = new List<Vector2>();   // List in which trajectory points will be stored for this motion state, stored as (angle, distance)
            List<float> mstd = new List<float>();

            for (int i = -6; i < 6; i++)
            {
                // Trajectory points
                Vector3 point = tp[index - half + i * 20];
                Vector2 parametricPoint = PointToAngleAndDistance(point, pivotPosition, pivotDirection);
                mstp.Add(parametricPoint);

                // Trajectory directions
                Vector3 facingDirectionOnFrame = td[index - half + i * 20];
                Vector3 facingDirectionCurrent = new Vector3(m.GetQ().x, 0, m.GetQ().y);

                float angle = Vector3.SignedAngle(facingDirectionCurrent, facingDirectionOnFrame, Vector3.up);
                mstd.Add(angle);
            }

            //msl[(int)currentFrame - half].AddTrajectories(new List<Vector3>(tp));
            msl[index - half].AddTrajectories(mstp, mstd);
            //Debug.Log(MotionStateF.ExportFloatListAsString(mstd));
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

    private void DrawDebugLines(MotionStateF m) {

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
            //Debug.Log(MotionStateF.Vector2ListToString(tps));
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

    // Given a point in the space and a pivot with its forward direction, returns the point in an (angle,distance) representation relative to the pivot
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
            foreach (MotionStateF m in msl)
            {
                //string line = m.GetP() + "," + m.GetQ() + "," + MotionStateF.Vector3ListToString(m.GetJ()) + "," + MotionStateF.Vector3ListToString(m.GetH());
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
            foreach (MotionStateF m in msl)
            {
                string line = MotionStateF.ExportVector2ListAsString(m.GetTP());
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
                    MotionStateF ms = new MotionStateF(line);

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
        //Debug.Break();
        int standing_frames = 0;
        for(int i = 0; i < msl.Count; i++)
        {
            Debug.Log(i + " - " + msl[i].GetRightFootContact() + " - " + msl[i].GetLeftFootContact() + " - " +
                Vector3.Distance(msl[i].leftFootPosition, msl[i].rightFootPosition) + "- " +
                msl[i].GetRV().y);


            if (msl[i].GetRightFootContact() && msl[i].GetLeftFootContact() &&
                Vector3.Distance(msl[i].leftFootPosition, msl[i].rightFootPosition) < 0.3f &&
                msl[i].GetRV().y < 0.01) {
                msl[i].SetStanding(true);
                standing_frames++;
            } 

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
        Debug.Log("Standing Frames: " + standing_frames);

    }

    private void UpdateLastFramesPhase(int currentFrame, int n, float phase) {
        float step = Mathf.PI / n;

        
        for (int i = 0; i < n; i++) {
            msl[currentFrame - i].SetPhase(phase - i*step);
        }

    }
}




