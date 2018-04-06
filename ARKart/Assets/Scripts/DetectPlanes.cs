﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.HelloAR;
using System;

/*
 *		Simple plane detector
 *		- adds simple visualizer plane when detects floor
 */
public class DetectPlanes : MonoBehaviour
{
    #region Variables
    enum InfoUI { SearchingForPlane, TapToPlace };

    [SerializeField]
    private GameObject TrackedPlanePrefab;

    [SerializeField]
    private GameObject _locateFloorCanvasParent;

    [SerializeField]
    private List<GameObject> _infoUI = new List<GameObject>();

    [SerializeField]
    private GameObject _floorPlane;
    private bool _planeInstantiated = false;
    private bool _calibrationPhaseStarted = false;
    private bool _calibrationDone = false;

    private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();
    private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

    public delegate void CalibrationDoneEvent();
    public static CalibrationDoneEvent CalibrationDone;

    #endregion

    void Awake()
    {
        AddMissingReferences();

        GameLogicController.CurrentGamePhase += OnCurrentGamePhase;
        InputController.TouchDetected += OnTouchDetected;
    }

    void Update()
    {
        if (!_calibrationPhaseStarted)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (_floorPlane && !_planeInstantiated)
        {
            GameObject floor = Instantiate(_floorPlane, Vector3.zero, Quaternion.identity);
            _planeInstantiated = true;
        }

#elif UNITY_IOS || UNITY_ANDROID
        // Check that motion tracking is tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            if (Session.Status.IsValid())
            {
                _infoUI[(int)InfoUI.SearchingForPlane].SetActive(true);
            }

            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
        Session.GetTrackables<TrackedPlane>(m_NewPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < m_NewPlanes.Count; i++)
        {
            // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
            // the origin with an identity  otation since the mesh for our prefab is updated in Unity World
            // coordinates.
            GameObject planeObject = Instantiate(TrackedPlanePrefab, Vector3.zero, Quaternion.identity,
                transform);
            planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
        }

        Session.GetTrackables<TrackedPlane>(m_AllPlanes);
        bool showSearchingUI = true;
        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            {
                showSearchingUI = false;
                break;
            }
        }

        _infoUI[(int)InfoUI.SearchingForPlane].SetActive(showSearchingUI);

        if(!showSearchingUI)
        {
            _infoUI[(int)InfoUI.TapToPlace].SetActive(!showSearchingUI);

            if(CalibrationDone != null)
                CalibrationDone();
            _calibrationDone = true;
        }
#endif
    }

    void AddMissingReferences()
    {
        if (_locateFloorCanvasParent == null)
        {
            _locateFloorCanvasParent = GameObject.Find(ConstantStrings.LOCATEFLOORCANVAS);
        }
        if (_infoUI.Count == 0)
        {
            Debug.LogWarning("List of Info UI's was null. Children added but not necessarily on correct order");
            for (int i = 0; i < _locateFloorCanvasParent.transform.childCount; i++)
            {
                _infoUI.Add(_locateFloorCanvasParent.transform.GetChild(i).gameObject);
            }
        }
    }

    #region Events
    private void OnCurrentGamePhase(GameEnums.GamePhase phase)
    {
        if (phase == GameEnums.GamePhase.CALIBRATION_PHASE)
        {
            _calibrationPhaseStarted = true;
        }
    }

    private void OnTouchDetected(Vector3 touchPos)
    {
        if(_calibrationDone)
        {
            Instantiate(_floorPlane, touchPos, Quaternion.identity);
        }
    }
    #endregion
}
