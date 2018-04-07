using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class PositionMenuCanvas : MonoBehaviour
{

    public Camera _androidCam;

    public GameObject _floorPlane;
    public GameObject _worldMenuCanvas;
    void Awake()
    {
        InputController.TouchDetected += OnTouchDetected;
    }

    private void OnTouchDetected(TrackableHit touch)
    {
        var andyObject = Instantiate(_floorPlane, touch.Pose.position, touch.Pose.rotation);

        //var worldCanvas = Instantiate(_worldMenuCanvas, touch.Pose.position, touch.Pose.rotation);
        _worldMenuCanvas.transform.position = touch.Pose.position;
		//_worldMenuCanvas.transform.rotation = touch.Pose.rotation;


        // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
        // world evolves.
        var anchor = touch.Trackable.CreateAnchor(touch.Pose);

        // Andy should look at the camera but still be flush with the plane.
        if ((touch.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
        {
            // Get the camera position and match the y-component with the hit position.
            Vector3 cameraPositionSameY = Camera.main.transform.position;
            cameraPositionSameY.y = touch.Pose.position.y;

            /*Vector3 cameraPos = new Vector3(Camera.main.transform.position.x,
                                            _worldMenuCanvas.transform.position.y,
                                             Camera.main.transform.position.z);
        _worldMenuCanvas.transform.LookAt(-cameraPos);*/

            // Have Andy look toward the camera respecting his "up" perspective, which may be from ceiling.
             //_worldMenuCanvas.transform.LookAt(-cameraPositionSameY, _worldMenuCanvas.transform.up);
        }
        /*Vector3 cameraPos = new Vector3(Camera.main.transform.position.x,
                                                    Camera.main.transform.position.y,
                                                     Camera.main.transform.position.z);
        _worldMenuCanvas.transform.LookAt(-cameraPos, _worldMenuCanvas.transform.up);*/
        // Make Andy model a child of the anchor.
        andyObject.transform.parent = anchor.transform;
        _worldMenuCanvas.transform.parent = anchor.transform;

        _worldMenuCanvas.SetActive(true);
    }
}
