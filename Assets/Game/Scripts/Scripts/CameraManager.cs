using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public static Camera Cam;

    private void Awake()
    {
        Cam = Camera.main;
        Instance = this;
    }

    private void Update()
    {
    }
}