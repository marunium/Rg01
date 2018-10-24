using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DebugButtonsController : MonoBehaviour {

	[SerializeField]
	Button DebugButton;
	[SerializeField]
	GameObject DebugMenu;

	// Use this for initialization
	void Start ()
	{
		DebugMenu.SetActive(false);

		DebugButton.OnClickAsObservable()
			.Subscribe(_=> DebugMenu.SetActive(true))
			.AddTo(this);
	}
}
