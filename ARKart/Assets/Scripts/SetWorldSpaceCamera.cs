using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWorldSpaceCamera : MonoBehaviour
{
    void Start()
    {
        GameObject _camera = GameObject.FindWithTag("MainCamera");
        gameObject.GetComponent<Canvas>().worldCamera = _camera.GetComponent<Camera>();
    }
}
