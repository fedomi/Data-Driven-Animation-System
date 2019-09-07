using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CheckFrames : MonoBehaviour
{
    [Header("Parameters")]
    public int numberOfIntervals = 10;
    public int interval = 0;
    public float speed = 1;
    public int currentFrame = 0;
    public string exportFileName = "fixed_output.txt";

    private string file_name = "MotionData_Experiment01.txt";
    private Animator _animator;
    private List<MotionStateF> msl;
    private int N;
    private Body b;
    private Vector2 intervalLimits;
    private float currentTimer = 0;
    private MotionStateF m;
    private bool deletedItems = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

        msl = new List<MotionStateF>();
        ImportData();
        N = msl.Count;
        b = new Body(transform);

        

    }

    // Update is called once per frame
    void Update()
    {
        float intervalLength = (2 * Mathf.PI) / (float)numberOfIntervals;
        intervalLimits = new Vector2(interval * intervalLength, (interval + 1) * intervalLength);

        currentTimer += Time.deltaTime;
        m = msl[currentFrame];
        float phase = m.GetPhase();
        if (phase >= intervalLimits.x && phase < intervalLimits.y)
        {
            b.ApplyPose(m.GetJ());
            if (currentTimer > speed / 10.0f)
            {
                currentFrame++;
                currentTimer = 0;
            }
        }
        else {
            Debug.Log("Waiting for a frame within phase interval.");
            currentFrame++;
            currentTimer = 0;
        }




    }


    private void ImportData()
    {
        // Read file using StreamReader. Reads file line by line  
        using (StreamReader file = new StreamReader(@"Assets\AnimationFiles\" + file_name))
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
                if (line.Length >= 193)
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

    public void ExportData()
    {

        Debug.Log("Exporting " + msl.Count + " motion states.");
        // Export motion data
        using (StreamWriter file =
            new StreamWriter(@"Assets\AnimationFiles\" + exportFileName , true))
        {
            foreach (MotionStateF m in msl)
            {
                string line = m.ExportString();
                file.WriteLine(line);

            }
            Debug.Log("Motion data export completed.");
            file.Close();
        }
    }

    public void DeleteClip() {

        int startFrames = msl.Count;

        int intervalStartIndex = currentFrame;
        int intervalEndIndex = currentFrame;
        float f_phase = m.GetPhase();  // frame phase

        // If the current frame is out of the interval, goes back till the last valid phase interval
        while (!(f_phase > intervalLimits.x && f_phase < intervalLimits.y))
        {
            intervalStartIndex--;
            intervalEndIndex--;
            f_phase = msl[intervalEndIndex].GetPhase();
        }

        // Search for the start of the clip
        Debug.Log("Deleting past phase frames");
        while (f_phase > intervalLimits.x && f_phase < intervalLimits.y && intervalStartIndex > 0)
        {
            intervalStartIndex--;
            Debug.Log(intervalStartIndex + " - " + intervalLimits.x + " - " + f_phase);
            f_phase = msl[intervalStartIndex].GetPhase();
        }

        // Search for the end of the clip
        Debug.Log("Deleting future phase frames");
        f_phase = m.GetPhase();
        while (f_phase > intervalLimits.x && f_phase < intervalLimits.y && intervalEndIndex < msl.Count)
        {
            intervalEndIndex++;
            Debug.Log(intervalStartIndex + " - " + intervalLimits.x + " - " + f_phase);
            f_phase = msl[intervalEndIndex].GetPhase();
        }
        
        
        // Remove clip
        int elementsToRemove = intervalEndIndex - intervalStartIndex;

        msl.RemoveRange(intervalStartIndex, elementsToRemove);

        Debug.Log("Deleted " + elementsToRemove + " frames, from frame " + intervalStartIndex + " to " + intervalEndIndex);
        Debug.Log("Frame list has gone from " + startFrames + " to " + msl.Count);
        deletedItems = true;
    }

    void OnDrawGizmos()
    {
        if (m != null)
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
}
