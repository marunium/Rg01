using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class RoteJoystick : MonoBehaviour {

	[SerializeField]
	VariableJoystick VariableJoystick;

	[SerializeField]
	GameObject Camera;

	Quaternion _cameraRote;

	private void Start()
	{
		_cameraRote =new Quaternion(
			Camera.transform.localRotation.x, 
			Camera.transform.localRotation.y,
			Camera.transform.localRotation.z,
			Camera.transform.localRotation.w);

		VariableJoystick.JoysticSubject
			.Where(x=>!x)
			.Subscribe(_=>
			{
				var rote = Camera.transform.localRotation;
				rote.x = _cameraRote.x;
				rote.y = _cameraRote.y;
				Camera.transform.localRotation = rote;
			})
			.AddTo(this);
	}

	private void FixedUpdate()
	{
		if (VariableJoystick.isJoysticState)
		{
			var pos = Camera.transform.localRotation;
			pos.y += VariableJoystick.JoystickPos.x * (0.0001f * PlayerPrefs.GetInt("speed_offset", 1));
			pos.x += (VariableJoystick.JoystickPos.y * -1) * (0.0001f * PlayerPrefs.GetInt("speed_offset", 1));
			Camera.transform.localRotation = pos;
		}
	}
}
