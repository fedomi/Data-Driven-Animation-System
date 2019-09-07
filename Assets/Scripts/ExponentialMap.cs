using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExponentialMap
{

    // Parameterizes an Euler Angle rotation vector v using Exponential Map
    public static Quaternion Map(Vector3 v) {
        //float theta = v.magnitude;
        Quaternion res;
        Vector3 vrad = v * Mathf.Deg2Rad;
        float theta = vrad.magnitude;

        //Debug.Log("Theta value was: " + theta);
        if (v.x == 0 && v.y == 0 && v.z == 0)
            res = new Quaternion(0, 0, 0, 1);
        else {
            float alpha = (Mathf.Sin(theta / 2) / theta);
            Vector3 a = new Vector3(alpha * vrad.x, alpha * vrad.y, alpha * vrad.z);

            float b = Mathf.Cos(theta/2);
            res = new Quaternion(a.x, a.y, a.z, b);
        }

        return res;
    }


    // Gets an exponential map quaternion and returns its Euler Angle representation
    public static Vector3 Inverse(Quaternion q) {
        Vector3 qv = new Vector3(q.x, q.y, q.z);
        if (qv.magnitude == 0)
            return new Vector3(0, 0, 0);

        float alpha = 2 * Mathf.Rad2Deg * Mathf.Acos(q.w) / qv.magnitude;

        return alpha * qv;
    }
}
