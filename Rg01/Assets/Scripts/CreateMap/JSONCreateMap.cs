using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// miniJSonを使用するには以下のusingを追加する
using MiniJSON;
public class JSONCreateMap : MonoBehaviour {
	private const float CHIP_SIZE_X = 1.5f;
	private const float CHIP_SIZE_Y = 1.5f;
	private const float CHIP_SIZE_Z = 1.5f;

	// マップチップデータ一覧
	private enum MapChip{
		PanelNone,
		PanelFloor,
		PanelJumpUp,
		PanelGoal,
		PanelStart,
		PanelMax
	};
	
	public GameObject stagePanelsRoot; // パネル配置のためのルートオブジェクト.
	
	public GameObject prefabPanelError;
	public GameObject prefabPanelNone;
	public GameObject prefabPanelJumpUp;
	public GameObject prefabPanelFloor;
	public GameObject prefabPanelGoal;
	public GameObject prefabPanelStart;
	public GameObject prefabPanelMax;
	
	public GameObject prefabStartField;
	public GameObject prefabGoalField;

	public GameObject player = null;


	private readonly Dictionary<MapChip, GameObject> mapChipTagToPrefab;

	[SerializeField]
	private GameObject prefabStartLine = null;


	// Use this for initialization
	void Start () {
		StartCoroutine( "createPrefabsFromJSON" );
	}
	
	IEnumerator createPrefabsFromJSON( )  {
		string read_json_data = "";
		
		string map_name = "mapData_tutorial";
		//map_name = SaveManager.loadSelectedStage();
		
		if (map_name.Contains("dl:")) {
			// Downloaded file.
			string mn = map_name.Replace("dl:", "");
			
			WWW filereader = new WWW("file://" + Application.persistentDataPath + "/" + mn + ".json");
			while (!filereader.isDone) {
				yield return null;
			}
			if (!string.IsNullOrEmpty(filereader.error)) {
				Debug.LogError("maplist error:" + filereader.error);
				yield break;
			}
			
			read_json_data = filereader.text;
			
		} else {
			// From Resource.
			TextAsset ReadJSONData = Instantiate (Resources.Load ("mapData1")) as TextAsset;
			//Debug.Log("map_name"+map_name);
			read_json_data = (string)ReadJSONData.text;
		}
		
		
		var MapList = (Dictionary<string, object>)Json.Deserialize( read_json_data );
		
		yield return new WaitForSeconds(1.0f);
		GameObject nowLoadingRichClose = GameObject.Find("NowLoadingRichClose") as GameObject;
		//var nowLoadingRichCloseScript = nowLoadingRichClose.GetComponent<NowLoadingRich>();
		
		// longにキャストしてからintにキャストするとintにキャストできるようになる
		int width = ( int )( long )MapList[ "width" ];
		int depth = ( int )( long )MapList[ "depth" ];
		int chip_pos_shift_index = 0;
		int breakTimeInterval = 0;
		bool foundStartPanel = false;
		Vector3 startPanelPositionLeftEnd = Vector3.zero;
		Vector3 startPanelPositionRightEnd = Vector3.zero;
		//Debug.Log("map size:" + width + ", " + depth + "[" + ((List<object>)MapList["chips"]).Count + "]");
		foreach (Dictionary<string, object> LoadMap in MapList["chips"] as List<object>) {
			//GameObject obj = ( GameObject )Instantiate( prefab );
			GameObject obj;
			// 読み込んだチップ名をMapChip列挙体型に変換
			string read_chip_name = LoadMap[ "name" ] as string;
			GameObject map_chip_prefab = null;
			//
			// チップ名からそのチップが格納されているディレクトリを取得して読み込む
			map_chip_prefab = LoadMapChipPrefab(read_chip_name);

			if (map_chip_prefab != null) {
				//Debug.Log("x,y,z:" + CHIP_SIZE_X + "," + CHIP_SIZE_Y + "," + CHIP_SIZE_Z);
				
				int x = chip_pos_shift_index % width;
				int z = chip_pos_shift_index / width;
				// 位置を設定
				// 	パネルは (0, y, 0) の位置から配置し始める。
				// 	横幅個数分、+x 方向へ順々に配置する。
				// 	横幅個数分配置後、-z 方法へ１パネル分ずらし、また横幅個数分配置していく。
				// 	つまり、スタート位置は -z 方向で、プレイヤーは +z 方向へ進むことでゴールすることになる.
				float pos_x = 0.0f + ( x * CHIP_SIZE_X );
				float pos_y = 0.0f + ( CHIP_SIZE_Y );
				float pos_z = 0.0f + ( -z * CHIP_SIZE_Z );
				Vector3 pos = new Vector3( pos_x, pos_y, pos_z);
				//Debug.Log("create panel:" + pos + "width" + width);
				
				obj = ( GameObject )Instantiate( map_chip_prefab, pos, Quaternion.identity );
				if (!foundStartPanel) {
					if (map_chip_prefab == prefabPanelStart) {
						startPanelPositionLeftEnd = pos;
						foundStartPanel = true;
					}
				} else {
					if (map_chip_prefab == prefabPanelStart) {
						if (startPanelPositionLeftEnd.z == pos.z) {
							startPanelPositionRightEnd = pos;
						}
					}
				}
				
				// 名前を設定
				obj.name = read_chip_name;
				// ルートオブジェクトを設定.
				// 	シーン内でオブジェクトが散らかるの防ぐため.
				obj.transform.parent = stagePanelsRoot.transform;
				
			}
			chip_pos_shift_index++;
			
			breakTimeInterval++;
			if (breakTimeInterval > 30) {
				breakTimeInterval = 0;
				yield return null;
			}
			
		}

		// setup start line.
		{
			if ((startPanelPositionLeftEnd == Vector3.zero) && (startPanelPositionRightEnd == Vector3.zero)) {
				Debug.LogError("JSONCreateMap: Not found start panels.");
			} else {
				if (startPanelPositionRightEnd.x > startPanelPositionLeftEnd.x)
				{ 
					GameObject startLine = Instantiate(prefabStartLine, Vector3.zero, prefabStartLine.transform.localRotation) as GameObject;
					Vector3 scale = prefabStartLine.transform.localScale;
					scale.x *= (startPanelPositionRightEnd.x - startPanelPositionLeftEnd.x) / CHIP_SIZE_X + 1.0f;
					startLine.transform.localScale = scale;
					startLine.transform.localPosition = new Vector3(((startPanelPositionRightEnd.x - startPanelPositionLeftEnd.x) / 2.0f) + startPanelPositionLeftEnd.x, startPanelPositionLeftEnd.y + 0.01f, startPanelPositionLeftEnd.z + 0.75f);
					startLine.transform.SetParent(stagePanelsRoot.transform);
				}
				else
				{
					GameObject startLine = Instantiate(prefabStartLine, Vector3.zero, prefabStartLine.transform.localRotation) as GameObject;
					Vector3 scale = prefabStartLine.transform.localScale;
					scale.x = CHIP_SIZE_X;
					startLine.transform.localScale = scale;
					startLine.transform.localPosition = new Vector3(startPanelPositionLeftEnd.x, startPanelPositionLeftEnd.y + 0.01f, startPanelPositionLeftEnd.z + 0.75f);
					startLine.transform.SetParent(stagePanelsRoot.transform);
				}
			}
		}
		
		// ステージの外周を埋める強制落下判定エリア.
		// 	ブランクパネルを超拡大し、ステージの四方に配置する.
		{
			float stageWidth = width * CHIP_SIZE_X;
			float stageDepth = depth * CHIP_SIZE_Z;
			float stageCenterX = -(CHIP_SIZE_X / 2.0f) + (stageWidth / 2.0f);
			float stageCenterZ = (CHIP_SIZE_Z / 2.0f) - (stageDepth / 2.0f);
			
			//GameObject stageCenterObject = (GameObject)Instantiate(prefabPanelNone, Vector3.zero, Quaternion.identity);
			//stageCenterObject.name = "stageCenterObject";
			//stageCenterObject.transform.parent = stagePanelsRoot.transform;
			//stageCenterObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			//stageCenterObject.transform.localPosition = new Vector3(stageCenterX, 1.5f, stageCenterZ);
			
			int rateOutsideAreaPanelWidth = System.Math.Max(1000, width);
			int rateOutsideAreaPanelDepth = System.Math.Max(1000, depth);
			float outsidePanelWidth = rateOutsideAreaPanelWidth * CHIP_SIZE_X;
			float outsidePanelDepth = rateOutsideAreaPanelDepth * CHIP_SIZE_Z;
			
			GameObject oku = (GameObject)Instantiate(prefabPanelNone, Vector3.zero, Quaternion.identity);
			oku.name = "outside";
			oku.transform.parent = stagePanelsRoot.transform;
			oku.transform.localScale = new Vector3(rateOutsideAreaPanelWidth, 1.0f, rateOutsideAreaPanelDepth);
			oku.transform.localPosition = new Vector3(stageCenterX, 1.5f, stageCenterZ + (stageDepth / 2 + outsidePanelDepth / 2));
			
			GameObject temae = (GameObject)Instantiate(prefabPanelNone, Vector3.zero, Quaternion.identity);
			temae.name = "outside";
			temae.transform.parent = stagePanelsRoot.transform;
			temae.transform.localScale = new Vector3(rateOutsideAreaPanelWidth, 1.0f, rateOutsideAreaPanelDepth);
			temae.transform.localPosition = new Vector3(stageCenterX, 1.5f, stageCenterZ - (stageDepth / 2 + outsidePanelDepth / 2));
			
			GameObject hidari = (GameObject)Instantiate(prefabPanelNone, Vector3.zero, Quaternion.identity);
			hidari.name = "outside";
			hidari.transform.parent = stagePanelsRoot.transform;
			hidari.transform.localScale = new Vector3(rateOutsideAreaPanelWidth, 1.0f, rateOutsideAreaPanelDepth);
			hidari.transform.localPosition = new Vector3(stageCenterX - (stageWidth / 2 + outsidePanelWidth / 2), 1.5f, stageCenterZ);
			
			GameObject migi = (GameObject)Instantiate(prefabPanelNone, Vector3.zero, Quaternion.identity);
			migi.name = "outside";
			migi.transform.parent = stagePanelsRoot.transform;
			migi.transform.localScale = new Vector3(rateOutsideAreaPanelWidth, 1.0f, rateOutsideAreaPanelDepth);
			migi.transform.localPosition = new Vector3(stageCenterX + (stageWidth / 2 + outsidePanelWidth / 2), 1.5f, stageCenterZ);
		}
		
		// adjust player's position.
		if (player != null) {
			//var startPanel = GameObject.Find("PanelStart") as GameObject;
			//if (startPanel != null) {
			//	var start_position = startPanel.transform.position;
			//	start_position.y += 3.0f;
			//	player.transform.position = start_position;
			//} else {
				var startposition = new Vector3(width / 2 * CHIP_SIZE_X - (CHIP_SIZE_X / 2), 3.0f, (CHIP_SIZE_Z) - (depth * CHIP_SIZE_Z));
				player.transform.position = startposition;
			//}
		}

		// Close "Now Loading".
		//nowLoadingRichCloseScript.Close();

		yield return new WaitForSeconds(0.8f);

		//stageOpenDialog.Show();

		yield return new WaitForSeconds(0.99f);
	}

	GameObject LoadMapChipPrefab(string in_read_chip_type)
	{
		GameObject result = null;
		MapChip parse_map_name = (MapChip)Enum.Parse(typeof(MapChip), in_read_chip_type);
		switch (parse_map_name)
		{
			case MapChip.PanelNone:
				result = prefabPanelNone;
				break;
			case MapChip.PanelJumpUp:
				result = prefabPanelJumpUp;
				break;
			case MapChip.PanelGoal:
				result = prefabPanelGoal;
				break;
			case MapChip.PanelStart:
				result = prefabPanelStart;
				break;
			case MapChip.PanelFloor: // @todo delete...
				result = prefabPanelFloor;
				break;
			case MapChip.PanelMax:
			default:
				Debug.LogError("不正なチップ名が指定されています chip_name : " + parse_map_name);
				result = prefabPanelError;
				break;
		}
		return result;
	}
}
