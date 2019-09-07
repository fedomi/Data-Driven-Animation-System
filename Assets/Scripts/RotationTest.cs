using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public Vector3 angles;
    public Transform cube;
    public List<Vector2> points;
    public int index;
    public float alpha;
    // Start is called before the first frame update
    void Start()
    {
        
        
          
    }

    // Update is called once per frame
    void Update()
    {
        cube.position = RotatePointAroundPivot(transform.position, cube.position, alpha);
        //Debug.Log(PointToAngleAndDistance(cube.position).ToString());
        //Debug.DrawLine(transform.position, AngleAndDistanceToPoint(points[index]), Color.black);
        //DrawDebugLines();
        ///Angle();
        //transform.position = RotatePointAroundPivot(transform.position);
    }

   Vector3 RotatePointAroundPivot(Vector3 pivot, Vector3 point, float a){
       Vector3 dir = point - pivot; // get point direction relative to pivot
       dir = Quaternion.Euler(new Vector3(0,a,0)) * dir; // rotate it
       point = dir + pivot; // calculate rotated point
       return point; // return it
   }

    void Angle() {
        /*Vector2 pivotForward2d = new Vector2(pivotObject.forward.x, pivotObject.forward.z);
        Vector2 pivotPosition = new Vector2(pivot.x, pivot.z);
        Vector2 thisPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 direction = thisPosition - pivotPosition;
        Debug.Log(Vector2.SignedAngle(pivotForward2d, direction));*/
    }


    private void DrawDebugLines()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5, Color.green);
        Vector3 hipPos = transform.position;
        Vector3 fwd = transform.forward;
        List<Vector3> tp = new List<Vector3>();

        Vector3 hip_flat = new Vector3(hipPos.x, 0.2f, hipPos.z);
        //Debug.Log(MotionStateD.Vector2ListToString(tps));
        for (int i = 0; i < points.Count; i++)
        {
            tp.Add(AngleAndDistanceToPoint(points[i]));

            if(i>0) Debug.DrawLine(tp[i], tp[i-1], new Color(0,0,45*i));
        }

    }

    private Vector3 AngleAndDistanceToPoint(Vector3 pivot, Vector3 fwd, Vector2 p_info) {
        Vector3 point = pivot + fwd * p_info.y;


        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0, p_info.x, 0)) * dir; // rotate it
        point = dir + transform.position; // calculate rotated point
        return point; // return it
    }

    private Vector3 AngleAndDistanceToPoint(Vector2 p_info)
    {
        Vector3 fwd = transform.forward;
        Vector3 pivot = transform.position;
        Vector3 point = pivot + fwd * p_info.y;

        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0, p_info.x, 0)) * dir; // rotate it
        point = dir + transform.position; // calculate rotated point
        return point; // return it
    }


    private Vector2 PointToAngleAndDistance(Vector3 point)
    {
        Vector3 fwd = transform.forward;
        Vector3 pivot = transform.position;

        Vector3 direction = point - pivot;
        float distance = direction.magnitude;

        float angle = Vector3.SignedAngle(fwd, direction, Vector3.up);

        return new Vector2(angle, distance);

    }

}
