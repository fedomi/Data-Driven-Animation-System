using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWorldPosition : MonoBehaviour
{
    public Vector3 Position;
    public Vector3 Rotation;

    private void Update()
    {
        Position = transform.position;
        Rotation = transform.rotation.eulerAngles;
    }
}
