using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{
    public static CameraManagerScript instance;
    public static Camera Cam;
    private void Awake()
    {
        Cam = Camera.main;
        instance = this;
    }

    private void Update()
    {
        
    }
}
