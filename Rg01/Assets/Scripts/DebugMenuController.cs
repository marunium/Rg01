using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DebugMenuController : MonoBehaviour {

	[SerializeField]
	Button CloseButton;

	[SerializeField]
	Slider Slider;

	// Use this for initialization
	void Start ()
	{
		CloseButton.OnClickAsObservable()
			.Subscribe(_=>gameObject.SetActive(false))
			.AddTo(this);

		Slider.OnValueChangedAsObservable()
			.Subscribe(value=> PlayerPrefs.SetInt("speed_offset", (int)value))
			.AddTo(this);
	}
}
