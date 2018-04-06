using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectEnabler : MonoBehaviour 
{
	public enum NeededObject { Camera, 
							Light };

	[Header("Cameras")]
	[SerializeField]
	private GameObject _cameraParent;

	[SerializeField]
	private List<GameObject> _sceneCameras = new List<GameObject>();
	private List<GameObject> _currentlyUsedCameras;

	[Header("Lights")]
	[SerializeField]
	private GameObject _lightController;

	[SerializeField]
	private List<GameObject> _sceneLights = new List<GameObject>();
	private List<GameObject> _currentlyUsedLights;

	void Awake()
	{
		if(_cameraParent == null)
			_cameraParent = GameObject.Find(ConstantStrings.CAMERAPARENT);
		if(_lightController == null)
			_lightController = GameObject.Find(ConstantStrings.LIGHTCONTROLLER);

		InitializeLists();

		#if UNITY_STANDALONE || UNITY_EDITOR
		EnableObjects(ConstantStrings.DEFAULT);

		#elif UNITY_ANDROID
		EnableObjects(ConstantStrings.ANDROID);

		#endif
	}

	public List<GameObject> GetCurrentObjects(NeededObject obj)
	{
		switch(obj)
		{
			case NeededObject.Camera:
				return _currentlyUsedCameras;
			break;
			case NeededObject.Light:
				return _currentlyUsedLights;
			break;
			default:
				Debug.LogWarning("GetCurrentObjects: Default enum shouldn't happen.");
				return null;
			break;
		}
	}

	///<summary>
	/// Adds all cameras and lights to a list
	///</summary>
	void InitializeLists()
	{
		if(_sceneCameras.Count == 0 && _cameraParent != null)
		{
			for (int i = 0; i < _cameraParent.transform.childCount; i++)
			{
				_sceneCameras.Add(_cameraParent.transform.GetChild(i).gameObject);
			}
		}
		else
		{
			Debug.LogWarning("References missing from GameController or it's children");
		}

		if(_sceneLights.Count == 0 && _lightController != null)
		{
			for (int i = 0; i < _lightController.transform.childCount; i++)
			{
				_sceneLights.Add(_lightController.transform.GetChild(i).gameObject);
			}
		}
		else
		{
			Debug.LogWarning("References missing from GameController or it's children");
		}
	}

	/// <summary>
	/// Sets lights active depending on chosen platform
	///</summary>
	void EnableObjects(string deviceInUse) 
	{
		foreach (GameObject camera in _sceneLights)
		{
			Debug.Log("camera: " + camera);
			bool isEnabled = (camera.layer == LayerMask.NameToLayer(deviceInUse)) ? true : false;
			camera.SetActive(isEnabled);
			_currentlyUsedCameras.Add(camera);
		}

		foreach (GameObject light in _sceneLights)
		{
			bool isEnabled = (light.layer == LayerMask.NameToLayer(deviceInUse)) ? true : false;
			light.SetActive(isEnabled);
			_currentlyUsedLights.Add(light);
		}
	}
}
