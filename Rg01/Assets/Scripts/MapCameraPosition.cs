using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraPosition : MonoBehaviour
{
    [SerializeField]
    private Transform Target;

    private bool isExpansion = false;       // 拡大フラグ
    private float expansionTime = 0.0f;     // 拡大時間
    private float dungeonSize = 0.0f;       // ダンジョンサイズ
    private const float SPEED = 0.2f;       // 拡大縮小速度

    private Vector3 beforePosition;
    private Vector3 afterPosition;
    
    void Start()
    {
        transform.position = new Vector3(Target.position.x, Target.position.y + 5.0f, Target.position.z);
        beforePosition = transform.position;
        afterPosition = transform.position;
    }
    

    void FixedUpdate()
    {
        expansionTime += Time.deltaTime;
        float ratio = Mathf.Lerp(0, 1, expansionTime / SPEED);
        if (!isExpansion)
        {
            if (ratio >= 1)
            {
                transform.position = new Vector3(Target.position.x, Target.position.y + 5.0f, Target.position.z-10);
            }
            else
            {
                transform.position = Vector3.Lerp(beforePosition, afterPosition, ratio);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(beforePosition, afterPosition, ratio);
        }
        transform.rotation = Quaternion.Euler(90, 90, 0);
    }

    public void Expension()
    {
        isExpansion = !isExpansion;
        expansionTime = 0.0f;
        beforePosition = transform.position;
        afterPosition = isExpansion ? new Vector3(-dungeonSize / 2, 5.0f, dungeonSize / 2) : new Vector3(Target.position.x, Target.position.y + 5.0f, Target.position.z);
    }

    public void SetDungeonSize(int size)
    {
        dungeonSize = size * 11.0f;
    }
}
