using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExponentialMapTest : MonoBehaviour
{

    public Transform cube1, cube2, cube3;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("----------------------");
            Debug.Log("Quaternion of cube 1 is: " + cube1.rotation.ToString("F4"));
            Debug.Log("Euler Angles of cube 1 are: " + cube1.eulerAngles.ToString("F4"));
            Quaternion q2 = ExponentialMap.Map(cube1.eulerAngles);
            //Debug.Log("Quaternion of cube 2 is: " + q2);

            cube2.eulerAngles = ExponentialMap.Inverse(q2);
            
            Debug.Log("Exponential map of cube 2 is: " + q2.ToString("F4"));
            Debug.Log("Euler Angles of cube 2 are: " + ExponentialMap.Inverse(q2).ToString("F4"));

            cube3.rotation = cube1.rotation.normalized;
            Debug.Log("Quaternion of cube 3 is: " + cube3.rotation.ToString("F4"));

        }

    }
}
