using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.HelloAR;

/*
 *		Simple plane detector
 *		- adds simple visualizer plane when detects floor
 */
public class DetectPlanes : MonoBehaviour 
{
	enum InfoUI { SearchingForPlane, TapToPlace };

	[SerializeField]
	private GameObject TrackedPlanePrefab;

	[SerializeField]
	 private List<GameObject> _infoUI = new List<GameObject>();
	 

	 private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();
	 private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();
	 
	void Update()
	{
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

            // Hide snackbar when currently tracking at least one plane.
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
			_infoUI[(int)InfoUI.TapToPlace].SetActive(!showSearchingUI);
	}
}
