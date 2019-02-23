using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PositionJoystick : MonoBehaviour {

	[SerializeField]
	VariableJoystick VariableJoystick;

	[SerializeField]
	GameObject Model;
	[SerializeField]
	Animator ModelAnimator;

	private void Start()
	{
		VariableJoystick.JoysticStateAsObservable()
			.DistinctUntilChanged()
			.Subscribe(isMove=> {
				ModelAnimator.SetBool("Run", isMove);
				Debug.Log("RUn");
			})
			.AddTo(this);
	}

	private void FixedUpdate()
	{
		if (VariableJoystick.isJoysticState)
		{
			var pos = Model.transform.position;
			pos.x += VariableJoystick.JoystickPos.x * (0.0001f * PlayerPrefs.GetInt("speed_offset", 1));
			pos.z += VariableJoystick.JoystickPos.y * (0.0001f * PlayerPrefs.GetInt("speed_offset", 1));
			Model.transform.position = pos;


			//var r = Mathf.Atan2(VariableJoystick.JoystickPos.y, VariableJoystick.JoystickPos.x);
			//var rote = Model.transform.rotation;
			//rote.y = r;

			var direction = new Vector3(VariableJoystick.JoystickPos.x, 0, VariableJoystick.JoystickPos.y); 

			Model.transform.rotation = Quaternion.LookRotation(direction);

		}
	}
}
