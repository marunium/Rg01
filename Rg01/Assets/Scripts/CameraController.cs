using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private GameObject player = null;
    [SerializeField]
    private Slider zoomSlider;
    private Vector3 offset = new Vector3(0.0f, 0.08f, 0.0f);// プレイヤーの首辺りで

    [SerializeField] private float distance = 0.9f; // ズーム
    [SerializeField] private float polarAngle = 73.0f; // angle with y-axis
    private float azimuthalAngle = 0.0f; // angle with x-axis
    private float azimuthalAngle2 = 0.0f; // angle with x-axis

    [SerializeField] private float minPolarAngle = 50.0f;
    [SerializeField] private float maxPolarAngle = 85.0f;
    [SerializeField] private float mouseXSensitivity = 2.0f;    // 横回転速度（手動）
    [SerializeField] private float mouseAutoXSensitivity = 2.0f;// 横回転速度（自動）
    [SerializeField] private float mouseYSensitivity = 2.0f;    // 縦回転速度

    [SerializeField] private bool isAuto = false;


    public void ResetSetUp(float angle)
    {
        azimuthalAngle = AngleCalculation(angle);
        Vector3 lookAtPos = player.transform.position + offset;
        UpdatePosition(lookAtPos);
        transform.LookAt(lookAtPos);
    }

    public void AutoSetUp(float angle)
    {
        if (!isAuto) return;
        azimuthalAngle2 = AngleCalculation(angle);
    }
    float AngleCalculation(float angle)
    {
        float res_angle = angle - 270;
        if (res_angle > 0) res_angle = 360 - res_angle;
        else res_angle *= -1;
        return res_angle;
    }

    public void ZoomInOut()
    {
        distance = zoomSlider.value;
    }
    
    bool isTouch = false;
    public void DungeonUpdate()
    {
        if (isAuto)
        {
            if (azimuthalAngle < azimuthalAngle2)
            {
                if (Mathf.Abs(azimuthalAngle - azimuthalAngle2) > 180)
                {
                    azimuthalAngle -= mouseAutoXSensitivity;
                    if (azimuthalAngle <= 0) azimuthalAngle = 360 + azimuthalAngle;
                }
                else
                {
                    azimuthalAngle += mouseAutoXSensitivity;
                    if (azimuthalAngle > azimuthalAngle2) azimuthalAngle = azimuthalAngle2;
                }
            }
            else
            {
                if (Mathf.Abs(azimuthalAngle - azimuthalAngle2) > 180)
                {
                    azimuthalAngle += mouseAutoXSensitivity;
                }
                else
                {
                    azimuthalAngle -= mouseAutoXSensitivity;
                    if (azimuthalAngle < azimuthalAngle2) azimuthalAngle = azimuthalAngle2;
                }
            }
            azimuthalAngle = Mathf.Repeat(azimuthalAngle, 360);
        }
        else
        {
            if(isTouch) UpdateAngle(moveVector.x, moveVector.y);
        }
        Vector3 lookAtPos = player.transform.position + offset;
        UpdatePosition(lookAtPos);
        transform.LookAt(lookAtPos);
    }

    Vector3 touchPosition;
    Vector2 moveVector;
    void Update()
    {
        if (!isAuto)
        {
            if (Input.GetMouseButtonDown(0) && !isTouch)
            {
#if UNITY_EDITOR
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
#elif UNITY_ANDROID
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return;
                }
#else
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
#endif
                isTouch = true;
                moveVector = new Vector2(0, 0);
                touchPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(0) && isTouch)
            {
                // 実機とエディタのスワイプ量を統一
                moveVector = new Vector2(
                  ((Input.mousePosition.x - touchPosition.x) / Screen.width) * 5.0f,// 横回転は少し早くしたい
                  (Input.mousePosition.y - touchPosition.y) / Screen.height
                );
            }
        }
        if (Input.GetMouseButtonUp(0) && isTouch)
        {
            touchPosition = new Vector3(0, 0, 0);
            moveVector = new Vector2(0, 0);
            isTouch = false;
        }
    }

    void UpdateAngle(float x, float y)
    {
        x = azimuthalAngle - x * mouseXSensitivity;
        azimuthalAngle = Mathf.Repeat(x, 360);

        y = polarAngle + y * mouseYSensitivity;
        polarAngle = Mathf.Clamp(y, minPolarAngle, maxPolarAngle);
    }

    void UpdatePosition(Vector3 lookAtPos)
    {
        float da = azimuthalAngle * Mathf.Deg2Rad;
        float dp = polarAngle * Mathf.Deg2Rad;
        transform.position = new Vector3(
            lookAtPos.x + distance * Mathf.Sin(dp) * Mathf.Cos(da),
            lookAtPos.y + distance * Mathf.Cos(dp),
            lookAtPos.z + distance * Mathf.Sin(dp) * Mathf.Sin(da));
    }

    public void AutoChange(bool auto) { isAuto = auto; }

    public void Reset()
    {
        azimuthalAngle = AngleCalculation(player.transform.rotation.y);
        azimuthalAngle2 = AngleCalculation(player.transform.rotation.y);
        Vector3 lookAtPos = player.transform.position + offset;
        UpdatePosition(lookAtPos);
        transform.LookAt(lookAtPos);
    }
}