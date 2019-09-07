using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : MonoBehaviour
{
    private LineRenderer lr;
    public GameObject sphere;
    public GameObject cylinder;
    public Vector3 start;
    public Vector3 end;
    
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        sphere.transform.position = end;
    }

    public void SetParameters(Vector3 s, Vector3 e) {
        start = s;
        end = e;
        sphere.transform.position = end;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);*/
        UpdateBonePosition(start, end, 0.05f);
    }


    void UpdateBonePosition(Vector3 s, Vector3 e, float width)
    {
        Vector3 offset = e - s;
        Vector3 scale = new Vector3(width, offset.magnitude / 2.0f, width);
        Vector3 position = start + (offset / 2.0f);


        cylinder.transform.position = position;
        cylinder.transform.up = offset;
        cylinder.transform.localScale = scale;

    }
}
