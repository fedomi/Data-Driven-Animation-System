using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimationDataC : MonoBehaviour
{
    #region Attributes

    public bool export = true;
    private Body b;
    private MotionStateC m;
    private List<MotionStateC> msl;
    public float currentFrame = 0;
    private float clipLength = 8168;
    private float frameToPlay;
    private Animator animator;
    private string clipName = "Locomotion";

    private List<Vector3> tp;   // Trajectory positions
    private Vector3 previous_position;
    private int TRAJECTORY_WINDOW = 960;

    private string DataFile = "LocomotionPFNN.txt";
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
        msl = new List<MotionStateC>();
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
                m = new MotionStateC(b);

                Debug.DrawLine(b.GetHips().position, b.GetHips().position + new Vector3(m.GetQ().x, 0, m.GetQ().y), Color.green);
                //Debug.Log("Forward direction: " + m.GetQ().x + "," + m.GetQ().y);
            }
            else
            {
                currentFrame++;
                m = new MotionStateC(b);

                b.ApplyPose(msl[(int)currentFrame].GetJ());
            }

            if (!play)
            {
                UpdateTrajectoryQueue();


                //m.PrintMotionStateC();
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

    // Update is called once per frame
    void Update()
    {
        /*
        if (currentFrame < clipLength) {
            frameToPlay = currentFrame / clipLength;
            animator.Play(clipName, 0, frameToPlay); // Frame is played

            currentFrame++;
            m = new MotionStateC(b);

            if (!play) {
                UpdateTrajectoryQueue();


                //m.PrintMotionStateC();
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

    private void DrawDebugLines(MotionStateC m) {

        List<Vector3> tps = m.GetTP();
        Vector3 hipPos = b.GetHips().position;
        Vector3 hip_flat = new Vector3(hipPos.x, 0, hipPos.z);

        Debug.Log(MotionStateC.Vector3ListToString(tps));
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

        Debug.Log("Exporting " + msl.Count + " motion states.");
        // Export motion data
        using (StreamWriter file =
            new StreamWriter(@"Assets\AnimationFiles\" + DataFile, true))
        {
            foreach (MotionStateC m in msl)
            {
                //string line = m.GetP() + "," + m.GetQ() + "," + MotionStateC.Vector3ListToString(m.GetJ()) + "," + MotionStateC.Vector3ListToString(m.GetH());
                string line = m.ExportString();
                file.WriteLine(line);
                
            }
            Debug.Log("Motion data export completed.");
            file.Close();    
        }
        

        // Export trajectory data (for testing the network)
        using (StreamWriter file =
            new StreamWriter(@"Assets\AnimationFiles\" + TrajectoriesFile, true))
        {
            foreach (MotionStateC m in msl)
            {
                string line = MotionStateC.ExportVector3ListAsString(m.GetTP());
                if(line.Length > 0)
                    file.WriteLine(line);
                
            }
            Debug.Log("Trajectory data export completed.");
            file.Close();
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
                MotionStateC ms = new MotionStateC(line);

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




