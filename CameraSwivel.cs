using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CameraSwivel : MonoBehaviour
{
    [SerializeField] public float rotationSpeed = 15f;
    [SerializeField] Transform _player;
    public Camera _camera;
    Light _spotlight;

    void Start()
    {
        _spotlight = GetComponentInChildren<Light>();
        _camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (CameraPlayerTracker.isPlayerDetected())
        {
            //_camera.transform.LookAt(_player);
            _spotlight.color = Color.red;
        } else
        {
            _spotlight.color = Color.green;
        }
        transform.localEulerAngles = new Vector3(0, Mathf.PingPong(Time.time * rotationSpeed, 90) - 48, 0);
    }
}
