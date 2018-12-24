using UnityEngine;
using System.Collections;

public class CompleteCameraController : MonoBehaviour
{

	public GameObject Target;       //プレイヤーゲームオブジェクトへの参照を格納する Public 変数


	private Vector3 _offset;         //プレイヤーとカメラ間のオフセット距離を格納する Public 変数


	// イニシャライゼーションに使用ます。
	void Start()
	{
		//プレイヤーとカメラ間の距離を取得してそのオフセット値を計算し、格納します。
		_offset = transform.position - Target.transform.position;
	}

	// 各フレームで、Update の後に LateUpdate が呼び出されます。
	void LateUpdate()
	{
		//カメラの transform 位置をプレイヤーのものと等しく設定します。ただし、計算されたオフセット距離によるずれも加えます。
		transform.position = Target.transform.position + _offset;
		//transform.rotation = Quaternion.Euler(0, Target.transform.localEulerAngles.y, 0);
		//transform.RotateAround(Target.transform.position, Vector3.right, mouseInputY);
	}
}