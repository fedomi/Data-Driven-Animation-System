using RunPythonScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RuntimeNNB : MonoBehaviour
{

    #region Attributes

    private Body b;
    private MotionStateB m;
    private List<MotionStateB> msl;
    [SerializeField]
    private float currentFrame = 10;

    private Animator animator;
    
    private List<Vector3> tp;   // Trajectory positions
    private Vector3 previous_position;
    private int TRAJECTORY_WINDOW = 960;

    private string DataFile = "LocomotionPFNN.txt";
    //private string DataFile = "MotionData_011.txt";
    private string TrajectoriesFile = "TrajectoryTestData.txt";
    string data;

    public bool A = true;
    #region Python script attributes
    private static string filePythonExePath = "C:/Users/fdomi/AppData/Local/Programs/Python/Python35/python.exe";
    private static string filePythonNamePath = "F:/ProyectosUnity/VanillaMotionMatching/VanillaMotionMatching/Assets/Scripts/Python/TrainedNNB.py";
    private IMLSharpPython mlSharpPython;

    private float currentTime = -0.0f;
    private float inputTimer = 1.0f;
    private bool sending = true;
    private bool receiving = false;
    private bool initialized = false;
    #endregion

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        
        //animator.speed = 0;
        b = new Body(transform);
        msl = new List<MotionStateB>();
        Debug.Log("Collecting animation data...");
        previous_position = b.GetFlatPosition();
        InitializeTrajectoryQueue();

        ImportData();
        RunPythonScript();

        initialized = true;
        data = msl[(int)currentFrame].ExportString();
    }

    // Update is called once per frame
    void Update()
    {/*
        if (currentTime > inputTimer)
        {
            string data = msl[(int)currentFrame].ExportString();

            Debug.Log("Sending: " + data);
            mlSharpPython.SendToPython(data);
            string received_data = mlSharpPython.ReadFromPython();
            string[] numbers = received_data.Split(' ');
            MotionStateB m = new MotionStateB(numbers);

            Debug.Log("Applying pose to " + m.GetJ().Count + " joints.");
            b.ApplyPose(m.GetJ(), m.GetH());
            //Debug.Log("Receiving A: " + received_data);
            //Debug.Log("Applying pose: " + MotionStateB.ExportVector3ListAsString(m.GetJ()));
            //UnityEngine.Debug.Log("Receiving B: " + mlSharpPython.ReadFromPython());
            currentTime = 0.0f;
            currentFrame++;
        }

        currentTime += Time.deltaTime;*/
    }


    /*  // Old FixedUpdate
     private void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) {
            string data = msl[(int)currentFrame].ExportString();

            Debug.Log("Sending: " + data);
            
            mlSharpPython.SendToPython(data);

            string received_data = mlSharpPython.ReadFromPython();
            Debug.Log("Received: " + received_data);
            string[] numbers = received_data.Split(' ');
            //Debug.Log("Numbers received: " + numbers.Length);
            MotionStateB m = new MotionStateB(numbers);
            
            /*
            string received_data = MotionStateB.ExportVector3ListAsString(msl[(int)(currentFrame+1)].GetJ());
            //Debug.Log("Received: " + received_data);
            string[] numbers = received_data.Split(',');
            Debug.Log("Received " + numbers.Length + " numbers");
            MotionStateB m = new MotionStateB(numbers);
            */
    //Debug.Log("Applying pose to " + m.GetJ().Count + " joints.");
   // b.ApplyPose(m.GetJ());
            //Debug.Log("Receiving A: " + received_data);
            //Debug.Log("Applying pose: " + MotionStateB.ExportVector3ListAsString(m.GetJ()));
            //UnityEngine.Debug.Log("Receiving B: " + mlSharpPython.ReadFromPython());
     //       currentTime = 0.0f;
       //     currentFrame++;
            
        //}
        
    //}
        // */

    private void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) {
            if (A)
            {
                data = msl[(int)currentFrame].ExportString();
                Debug.Log("------------------------------------");
                Debug.Log("Sending A: " + data);

                mlSharpPython.SendToPython(data);

                string received_data = mlSharpPython.ReadFromPython();
                Debug.Log("Received A: " + received_data);
                string[] numbers = received_data.Split(' ');
                //MotionStateB m = new MotionStateB(numbers);
                MotionStateB m = new MotionStateB();
                m.AddPose(numbers);
                m.SetTP(msl[(int)currentFrame + 1].GetTP());
                m.SetPhase(msl[(int)currentFrame + 1].GetPhase());
                b.ApplyPose(m.GetJ());
                DrawDebugLines(m);
            }
            else {
                Debug.Log("------------------------------------");
                Debug.Log("Sending B: " + data);
                mlSharpPython.SendToPython(data);
                string received_datab = mlSharpPython.ReadFromPython();
                string[] numbers = received_datab.Split(' ');

                MotionStateB m = new MotionStateB();
                m.AddPose(numbers);
                m.SetTP(msl[(int)currentFrame + 1].GetTP());
                m.SetPhase(msl[(int)currentFrame + 1].GetPhase());
                //Debug.Log("Pose: " + MotionStateB.ExportVector3ListAsString(m.GetJ()));
                Debug.Log("Received B: " + received_datab);

                data = m.ExportString();
                b.ApplyPose(m.GetJ());
                DrawDebugLines(m);
            }
            //data = m.ExportString();
            /*MotionStateB m = new MotionStateB();
            m.AddPose(numbers);
            m.SetTP(msl[(int)currentFrame + 1].GetTP());*/




        currentTime = 0.0f;
        currentFrame++;
            
        //}
        
    }

    

    private void RunPythonScript() {
        string standardError;
        mlSharpPython = new MLSharpPython(filePythonExePath);
        mlSharpPython.ExecutePythonScriptInBackground(filePythonNamePath, out standardError);
        if (string.IsNullOrEmpty(standardError))
        {
            //UnityEngine.Debug.Log(outputText);
            //Console.WriteLine(outputText);

        }
        else
        {
            Debug.Log(standardError);
            Console.WriteLine(standardError);
        }


    }

    private void InitializeTrajectoryQueue()
    {
        tp = new List<Vector3>();
        for (int i = 0; i < TRAJECTORY_WINDOW; i++)
        {
            //tp.Add(new Vector3(0,0,0));
            tp.Add(b.GetFlatPosition());
        }
    }


    // Adds the trajectory info to one of the past frames
    private void AddTrajectoryInfo()
    {
        //int half = TRAJECTORY_WINDOW / 2;
        int half = 600;



        if (currentFrame >= half)
        {
            List<Vector3> mstp = new List<Vector3>();
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

    private void DrawDebugLines(MotionStateB m)
    {

        List<Vector3> tps = m.GetTP();
        Vector3 hipPos = b.GetHips().position;
        Vector3 hip_flat = new Vector3(hipPos.x, 0, hipPos.z);

        Debug.Log(MotionStateB.Vector3ListToString(tps));
        for (int i = 0; i < tps.Count - 1; i++)
        {
            //Vector3 start = hip_flat + tps[i];
            Vector3 start = new Vector3(hip_flat.x + tps[i].x, 0.2f, hip_flat.z + tps[i].z);

            Vector3 end = new Vector3(hip_flat.x + tps[i + 1].x, 0.2f, hip_flat.z + tps[i + 1].z);
            //Vector3 end = hip_flat + tps[i + 1];
            Debug.DrawLine(start, end, Color.black);
        }

    }

    private List<Vector3> GenerateTrajectory()
    {
        List<Vector3> t = new List<Vector3>();

        for (int i = 0; i < 50; i++)
        {
            t.Add(new Vector3(i * 0.1f, 0, 0));
        }

        return t;
    }

    private void DrawTrajectory(List<Vector3> t)
    {
        for (int i = 0; i < t.Count - 1; i++)
        {
            //Vector3 start = hip_flat + tps[i];
            Vector3 start = new Vector3(t[i].x, 0.2f, t[i].z);

            Vector3 end = new Vector3(t[i + 1].x, 0.2f, t[i + 1].z);
            //Vector3 end = hip_flat + tps[i + 1];
            Debug.DrawLine(start, end, Color.red);
        }

    }

    private void ImportData()
    {
        // Read file using StreamReader. Reads file line by line  
        using (StreamReader file = new StreamReader(@"Assets\AnimationFiles\" + DataFile))
        {
            int counter = 0;
            string ln;

            while ((ln = file.ReadLine()) != null)
            {
                string[] line = ln.Split(',');
                //Debug.Log(line.Length);
                /*if(line.Length == 58)
                {
                    MotionStateB ms = new MotionStateB(line);

                    msl.Add(ms);

                }*/
                if (line.Length == 119)
                {
                    MotionStateB ms = new MotionStateB(line);

                    msl.Add(ms);

                }

                //Debug.Log("Parsing frame " + counter);
                counter++;
            }
            file.Close();
            Debug.Log("Data imported correctly.");
            Debug.Log(msl.Count + " motion states were loaded.");
        }

    }


}
