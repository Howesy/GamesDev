using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CameraPlayerTracker : MonoBehaviour
{
    [SerializeField] public Camera _camera;
    public float meshResolution;
    public float viewAngle;
    private static bool playerDetected;
    Plane[] _cameraFrustum;
    Collider _collider;

    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    void Update()
    {
      
        if (determinePlayerIntersection())
        {
            print("The CAMERA SEES YOU BOY!");
            playerDetected = true;
        }
        else
        {
            print("WHERE THE HELL YOU GO BOY?");
            playerDetected = false;
        }
    }

    //Calculates whether the gameObject in question is inside of the assigned cameras frustum range.
    public bool determinePlayerIntersection()
    {
        Bounds bounds = _collider.bounds;
        _cameraFrustum = GeometryUtility.CalculateFrustumPlanes(_camera);
        return GeometryUtility.TestPlanesAABB(_cameraFrustum, bounds);
    }

    public static bool isPlayerDetected()
    {
        return playerDetected;
    }

}
