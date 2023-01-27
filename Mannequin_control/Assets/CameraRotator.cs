using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float speed_of_camera_rotation;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, speed_of_camera_rotation * Time.deltaTime, 0);
        // transform.Translate(speed_of_camera_rotation * Time.deltaTime, 0, 0);
    }
}
