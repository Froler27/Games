using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 viewPort;
    private float fov;
    private float aspect;
    // Start is called before the first frame update
    void Start()
    {
        fov = Camera.main.fieldOfView;
        aspect = Camera.main.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
