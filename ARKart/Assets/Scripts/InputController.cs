using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class InputController : MonoBehaviour
{
    public delegate void TouchDetectedEvent(Vector3 touchPos);
    public static TouchDetectedEvent TouchDetected;

    public GameObject _floorPlane;
    public GameObject _worldMenuCanvas;
    private Vector2 touchOrigin = -Vector2.one;

    private bool _startCapturingTouches = false;

    void Awake()
    {
        DetectPlanes.CalibrationDone += OnCalibrationDone;

    }

    void Update()
    {

#if UNITY_STANDALONE || UNITY_WEBPLAYER

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        /* if (!_startCapturingTouches)
             return;*/
        /* if (Input.touchCount > 0)
         {
             Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
             RaycastHit hit;

             if (Physics.Raycast(ray, out hit))
             {

                 if (TouchDetected != null)
                 {
                     _text.text = "event sent";
                     TouchDetected(hit.point);
                 }
             }
         }*/
        if (!_startCapturingTouches)
            return;
        CalibrationPhaseTouches();

#endif
    }

    void CalibrationPhaseTouches()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            var andyObject = Instantiate(_floorPlane, hit.Pose.position, hit.Pose.rotation);

            var worldCanvas = Instantiate(_worldMenuCanvas, hit.Pose.position, hit.Pose.rotation);
            // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
            // world evolves.
            var anchor = hit.Trackable.CreateAnchor(hit.Pose);

            // Andy should look at the camera but still be flush with the plane.
            if ((hit.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
            {
                // Get the camera position and match the y-component with the hit position.
                Vector3 cameraPositionSameY = Camera.main.transform.position;
                cameraPositionSameY.y = hit.Pose.position.y;

                // Have Andy look toward the camera respecting his "up" perspective, which may be from ceiling.
                worldCanvas.transform.LookAt(-cameraPositionSameY, worldCanvas.transform.up);
            }

            // Make Andy model a child of the anchor.
            andyObject.transform.parent = anchor.transform;
            worldCanvas.transform.parent = anchor.transform;
        }
    }

    private void OnCalibrationDone()
    {
        _startCapturingTouches = true;
    }
}
