using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField]
    private Button _playButton;

	[SerializeField]
	private GameObject _playCanvas;

	void Start()
    {
        _playButton.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        _playCanvas.SetActive(true);
		gameObject.SetActive(false);
    }
}
