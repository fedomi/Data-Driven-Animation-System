using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimationData : MonoBehaviour
{
    #region Attributes

    public bool export = true;
    private Body b;
    private MotionState m;
    private List<MotionState> msl;
    public float currentFrame = 0;
    private float clipLength = 29500;
    private float frameToPlay;
    private Animator animator;
    private string clipName = "Locomotion";

    private List<Vector3> tp;   // Trajectory positions
    private Vector3 previous_position;
    private int TRAJECTORY_WINDOW = 960;

    private string DataFile = "MotionData_009.txt";
    private string TrajectoriesFile = "TrajectoryTestData.txt";


    // Play animation with visuals
    public bool play = false;
    public bool applyPose = false;

    
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        b = new Body(transform);
        b.PrintState();
        msl = new List<MotionState>();
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
                m = new MotionState(b);
            }
            else
            {
                currentFrame++;
                m = new MotionState(b);

                b.ApplyPose(msl[(int)currentFrame].GetJ());
            }

            if (!play)
            {
                UpdateTrajectoryQueue();


                //m.PrintMotionState();
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
                SetPhaseValues();
                ExportData();
                export = false;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (currentFrame < clipLength) {
            frameToPlay = currentFrame / clipLength;
            animator.Play(clipName, 0, frameToPlay); // Frame is played

            currentFrame++;
            m = new MotionState(b);

            if (!play) {
                UpdateTrajectoryQueue();


                //m.PrintMotionState();
                msl.Add(m);
                AddTrajectoryInfo();

            }
            else
            {
                DrawDebugLines(msl[(int)currentFrame]);
            }

        }
        else {
            if (export) {
                ExportData();
                export = false;
            }
            
        }*/
            
    }

    private void InitializeTrajectoryQueue() {
        tp = new List<Vector3>();
        for (int i = 0; i < TRAJECTORY_WINDOW; i++) {
            //tp.Add(new Vector3(0,0,0));
            tp.Add(b.GetFlatPosition());
        }
    }


    // Adds the trajectory info to one of the past frames
    private void AddTrajectoryInfo() {
        //int half = TRAJECTORY_WINDOW / 2;
        int half = 600;
        


        if (currentFrame >= half)
        {
            List<Vector3> mstp = new List<Vector3>();   // List in which trajectory points will be stored for this motion state
            for (int i = -10; i < 10; i++)
            {
                mstp.Add(tp[(int)currentFrame + i * 60]);

            }

            //msl[(int)currentFrame - half].AddTrajectories(new List<Vector3>(tp));
            msl[(int)currentFrame - 600].AddTrajectories(mstp);

        }
    }

    // Dequeues oldest frame trajectory info on list and adds the current one
    private void UpdateTrajectoryQueue()
    {
        //tp.Dequeue();
        tp.Add(b.GetFlatPosition());
        previous_position = b.GetFlatPosition();

    }

    private void DrawDebugLines(MotionState m) {

        List<Vector3> tps = m.GetTP();
        Vector3 hipPos = b.GetHips().position;
        Vector3 hip_flat = new Vector3(hipPos.x, 0, hipPos.z);

        Debug.Log(MotionState.Vector3ListToString(tps));
        for (int i = 0; i < tps.Count-1; i++) {
            //Vector3 start = hip_flat + tps[i];
            Vector3 start = new Vector3(hip_flat.x + tps[i].x, 0.2f, hip_flat.z + tps[i].z);

            Vector3 end = new Vector3(hip_flat.x + tps[i+1].x, 0.2f, hip_flat.z + tps[i+1].z);
            //Vector3 end = hip_flat + tps[i + 1];
            Debug.DrawLine(start, end, Color.black);
        }

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
        

        // Export motion data
        using (StreamWriter file =
            new StreamWriter(@"Assets\AnimationFiles\" + DataFile, true))
        {
            foreach (MotionState m in msl)
            {
                //string line = m.GetP() + "," + m.GetQ() + "," + MotionState.Vector3ListToString(m.GetJ()) + "," + MotionState.Vector3ListToString(m.GetH());
                string line = m.ExportString();
                file.WriteLine(line);
                
            }
            Debug.Log("Motion data export completed.");
            
        }

        // Export trajectory data (for testing the network)
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"Assets\AnimationFiles\" + TrajectoriesFile))
        {
            foreach (MotionState m in msl)
            {
                string line = MotionState.ExportVector3ListAsString(m.GetTP());
                file.WriteLine(line);
                
            }
            Debug.Log("Trajectory data export completed.");

            play = true;
            currentFrame = 10;
        }
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
                MotionState ms = new MotionState(line);

                msl.Add(ms);

                //Debug.Log("Parsing frame " + counter);
                counter++;
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
        bool contactLastFrame = false;
        int framesSinceLastUpdate = 0;

        Debug.Log("Calculating Phase values...");

        for(int i = 0; i < msl.Count; i++)
        {
            if (msl[i].GetRightFootContact())
            {
                if (!contactLastFrame)
                {
                    UpdateLastFramesPhase(i, framesSinceLastUpdate, 2 * Mathf.PI);
                    contactLastFrame = true;
                }
                else msl[i].SetPhase(0);

                framesSinceLastUpdate = 0;
            }
            else if (msl[i].GetLeftFootContact()) {
                if (!contactLastFrame)
                {
                    UpdateLastFramesPhase(i, framesSinceLastUpdate, Mathf.PI);
                    contactLastFrame = true;
                }
                else msl[i].SetPhase(Mathf.PI);

                framesSinceLastUpdate = 0;
            }
            else
            {
                contactLastFrame = false;
                framesSinceLastUpdate++;
            }


        }

    }

    private void UpdateLastFramesPhase(int currentFrame, int n, float phase) {
        float step = Mathf.PI / n;

        for (int i = 0; i < n; i++) {
            msl[currentFrame - i].SetPhase(phase - i*step);
        }

    }
}




