using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyStatusViewController : MonoBehaviour {

	[SerializeField]
	Slider HpGauge;
	[SerializeField]
	Slider SPGauge;


	// Use this for initialization
	void Start ()
	{
		HpGauge.value = 1;
		SPGauge.value = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (HpGauge.value > 0)
		{
			HpGauge.value -= 0.01f;
		}
		if (SPGauge.value > 0)
		{
			SPGauge.value -= 0.01f;
		}
	}
}
