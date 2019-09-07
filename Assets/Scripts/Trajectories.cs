using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Invector.CharacterController
{
    public class Trajectories : MonoBehaviour
    {
        #region Attributes
        private vThirdPersonController cc;
        private Vector3 td;
        private float speed;

        private float currentFrame = 0;
        private float T_POINT = 10;
        private Vector3 pos, prev_pos;

        
        private Queue<Vector3> t_past;
        public List<Vector3> t_future;
        #endregion


        // Start is called before the first frame update
        void Start()
        {
            cc = GetComponent<vThirdPersonController>();
            if (cc != null)
                cc.Init();

            speed = cc.velocity;
            pos = transform.position;
            prev_pos = pos;

            t_past = new Queue<Vector3>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 input = cc.input;
            td = cc.targetDirection;
            speed = cc.speed;
            
            Debug.Log("Input: " + input.x + "-" + input.y);
            /*Debug.Log("Target Direction: " + td.x + "-" + td.y + "-" + td.z);
            Debug.Log("Speed: " + speed);
            */

            currentFrame++;
            if (currentFrame % T_POINT == 0) {
                currentFrame = 0;


                pos = transform.position;
                float d = Vector3.Distance(pos, prev_pos);
                //Debug.Log("Distance in one trajectory window: " + d);
                prev_pos = pos;

                t_past.Enqueue(pos);
                if (t_past.Count > 10) t_past.Dequeue();
            }
            t_future = PredictTrajectories(input);

            Debug.Log("Current Position: " + pos.x + "-" + pos.y + "-" + pos.z);
            Debug.Log("Future Position: " + t_future[0].x + "-" + t_future[0].y + "-" + t_future[0].z);


            DrawDebugLines();
        }

        private List<Vector3> PredictTrajectories(Vector2 input) {
            List<Vector3> tf = new List<Vector3>();

            Vector3 input3D = new Vector3(input.x, 0, input.y);

            for (int i = 0; i < 10; i++) {
                Vector3 p = 0.2f * i * input3D;
                tf.Add(pos - p);
            }
                        

            return tf;
        }

        private void DrawDebugLines()
        {
            Vector3 start, end;


            List<Vector3> tp = new List<Vector3>(t_past);
            for (int i = 0; i < tp.Count - 1; i++)
            {
                //Vector3 start = hip_flat + tps[i];
                start = new Vector3(tp[i].x, 0.2f, tp[i].z);

                end = new Vector3(tp[i + 1].x, 0.2f, tp[i + 1].z);
                //Vector3 end = hip_flat + tps[i + 1];
                Debug.DrawLine(start, end, Color.black);
            }

            start = pos;
            end = t_future[0];
            Debug.DrawLine(start, end, Color.black);

            for (int i = 1; i < t_future.Count-1; i++) {
                start = t_future[i];
                end = t_future[i+1];
                Debug.DrawLine(start, end, Color.black);

            }

        }
    }
}
