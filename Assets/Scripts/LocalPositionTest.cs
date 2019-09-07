using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPositionTest : MonoBehaviour
{
    public Transform cube1, cube2, cube3, cube4;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {

            Vector3 local = cube3.position - cube2.position;
            cube4.position = cube3.position + local;

        }
    }
}
