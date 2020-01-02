using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCamera : MonoBehaviour
{
    void Start()
    {
        transform.Rotate(Vector3.up, -90, Space.Self);
        transform.Rotate(Vector3.up, -90, Space.Self);
        transform.Rotate(Vector3.left, -55, Space.Self);
    }
    void Update()
    {
        if (Input.GetKeyDown("right"))
        {
            transform.Rotate(Vector3.up, -90, Space.Self);
        }
        if (Input.GetKeyDown("left"))
        {
            transform.Rotate(Vector3.up, 90, Space.Self);
        }
        if (Input.GetKeyDown("up"))
        {
            transform.Rotate(Vector3.left, -55, Space.Self);
        }
    }
    public void RotateLeft()
    {
        transform.Rotate(Vector3.up, 90, Space.Self);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up, -90, Space.Self);
    }

}
