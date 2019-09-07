using RunPythonScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RuntimeNNF : MonoBehaviour
{

    #region Attributes

    private Body b;
    private MotionStateF m;
    private List<MotionStateF> msl;
    [SerializeField]
    private float currentFrame = 10;

    private Animator animator;
    
    private List<Vector3> tp;   // Trajectory positions
    private Vector3 previous_position;
    private int TRAJECTORY_WINDOW = 960;

    private string DataFile = "MotionData_Experiment0708.txt";
    //private string DataFile = "MotionData_011.txt";
    private string TrajectoriesFile = "TrajectoryTestData.txt";
    string data;

    public bool A = true;
    public bool drawTrajectoryLines = true;

    #region Python script attributes
    private static string filePythonExePath = "C:/Users/fdomi/AppData/Local/Programs/Python/Python35/python.exe";
    private static string filePythonNamePath = "F:/ProyectosUnity/VanillaMotionMatching/VanillaMotionMatching/Assets/Scripts/Python/TrainedNNF_Multi5.py";
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
        msl = new List<MotionStateF>();
        Debug.Log("Collecting animation data...");
        previous_position = b.GetFlatPosition();
        InitializeTrajectoryQueue();

        ImportData();
        RunPythonScript();

        initialized = true;
        data = msl[(int)currentFrame].ExportString();
    }

    // Update is called once per frame
    


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
            MotionStateF m = new MotionStateF(numbers);
            
            /*
            string received_data = MotionStateF.ExportVector3ListAsString(msl[(int)(currentFrame+1)].GetJ());
            //Debug.Log("Received: " + received_data);
            string[] numbers = received_data.Split(',');
            Debug.Log("Received " + numbers.Length + " numbers");
            MotionStateF m = new MotionStateF(numbers);
            */
    //Debug.Log("Applying pose to " + m.GetJ().Count + " joints.");
   // b.ApplyPose(m.GetJ());
            //Debug.Log("Receiving A: " + received_data);
            //Debug.Log("Applying pose: " + MotionStateF.ExportVector3ListAsString(m.GetJ()));
            //UnityEngine.Debug.Log("Receiving B: " + mlSharpPython.ReadFromPython());
     //       currentTime = 0.0f;
       //     currentFrame++;
            
        //}
        
    //}
        // */

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) {
        if (A)
        {
            data = msl[(int)currentFrame].ExportString();
            Debug.Log("------------------------------------");
            Debug.Log("Sending A: " + data);
            //Debug.Log("Sending data of length: " + data.Length);
            mlSharpPython.SendToPython(data);

            string received_data = mlSharpPython.ReadFromPython();
            Debug.Log("Received A: " + received_data);
            string[] numbers = received_data.Split(' ');
            //MotionStateF m = new MotionStateF(numbers);
            MotionStateF m = new MotionStateF();
            m.AddPose(numbers);
            m.SetP(new Vector2(b.GetHips().position.x, b.GetHips().position.z));
            m.SetQ(new Vector2(b.GetHips().forward.x, b.GetHips().forward.z));
            m.SetTP(msl[(int)currentFrame + 1].GetTP());
            m.SetTD(msl[(int)currentFrame + 1].GetTD());
            m.SetPhase(msl[(int)currentFrame + 1].GetPhase());
            DrawDebugLines(m);
            b.ApplyPose(m.GetJ(), m.GetRV(), m.GetDeltaQ());
            
        }
        else {
            Debug.Log("------------------------------------");
            Debug.Log("Sending B: " + data);
            mlSharpPython.SendToPython(data);
            string received_datab = mlSharpPython.ReadFromPython();
            string[] numbers = received_datab.Split(' ');

            MotionStateF m = new MotionStateF();
            m.AddPose(numbers);
            m.SetP(new Vector2(b.GetHips().position.x, b.GetHips().position.z));
            m.SetQ(new Vector2(b.GetHips().forward.x, b.GetHips().forward.z));
            m.SetTP(msl[(int)currentFrame + 1].GetTP());
            m.SetTD(msl[(int)currentFrame + 1].GetTD());
            m.SetPhase(msl[(int)currentFrame + 1].GetPhase());

            //Debug.Log("Pose: " + MotionStateF.ExportVector3ListAsString(m.GetJ()));
            Debug.Log("Received B: " + received_datab);
            DrawDebugLines(m);

            data = m.ExportString();
            b.ApplyPose(m.GetJ(), m.GetRV(), m.GetDeltaQ());
            DrawDebugLines(m);
        }
        //data = m.ExportString();
        /*MotionStateF m = new MotionStateF();
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
        int half = 120;



        if (currentFrame >= half)
        {
            List<Vector3> mstp = new List<Vector3>();
            for (int i = -10; i < 10; i++)
            {
                mstp.Add(tp[(int)currentFrame + i * 20]);

            }

            //msl[(int)currentFrame - half].AddTrajectories(new List<Vector3>(tp));
            //msl[(int)currentFrame - 600].AddTrajectories(mstp);

        }
    }

    // Dequeues oldest frame trajectory info on list and adds the current one
    private void UpdateTrajectoryQueue()
    {
        //tp.Dequeue();
        tp.Add(b.GetFlatPosition());
        previous_position = b.GetFlatPosition();

    }

    private void DrawDebugLines(MotionStateF m)
    {

        if (drawTrajectoryLines)
        {
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
            Debug.Log("Drawing " + points.Count + " trajectory points");
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

    private Vector2 PointToAngleAndDistance(Vector3 point, Vector3 pivot, Vector3 forward)
    {
        Vector3 direction = point - pivot;
        float distance = direction.magnitude;

        float angle = Vector3.SignedAngle(forward, direction, Vector3.up);
        //Debug.Log("Angle between " + forward.ToString("F8") + " and " + direction.ToString("F8") + ": " + angle);


        return new Vector2(angle, distance);

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
                    MotionStateF ms = new MotionStateF(line);

                    msl.Add(ms);

                }*/
                if (line.Length >= 170)
                {
                    MotionStateF ms = new MotionStateF(line);

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
