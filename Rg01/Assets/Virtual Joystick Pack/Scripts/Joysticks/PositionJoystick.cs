using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionJoystick : MonoBehaviour {

	[SerializeField]
	VariableJoystick VariableJoystick;

	[SerializeField]
	GameObject Model;

	private void FixedUpdate()
	{
		if (VariableJoystick.isJoysticState)
		{
			var pos = Model.transform.position;
			pos.x += VariableJoystick.JoystickPos.x * (0.0001f * PlayerPrefs.GetInt("speed_offset", 1));
			pos.z += VariableJoystick.JoystickPos.y * (0.0001f * PlayerPrefs.GetInt("speed_offset", 1));
			Model.transform.position = pos;
		}
	}
}
