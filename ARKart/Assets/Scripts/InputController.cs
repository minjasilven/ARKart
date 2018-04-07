using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class InputController : MonoBehaviour
{
    public delegate void TouchDetectedEvent(TrackableHit touchPos);
    public static TouchDetectedEvent TouchDetected;
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


        if (!_startCapturingTouches)
            return;
        StartCapturingTouches();

#endif
    }

    void StartCapturingTouches()
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
            if (TouchDetected != null)
                TouchDetected(hit);
        }
    }

    private void OnCalibrationDone()
    {
        _startCapturingTouches = true;
    }
}
