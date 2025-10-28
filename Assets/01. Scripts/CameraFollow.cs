using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target_P")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5.0f;

    [Header("FollowBorder")]
    [SerializeField] private float followBorderX = 0.8f;
    [SerializeField] private float followBorderY = 0.05f;

    [Header("CameraBorder")]
    [SerializeField] private Vector2 minBorder;
    [SerializeField] private Vector2 maxBorder;


    private Vector3 targetPosition;
    private Vector3 nowPosition;

    //이동후 카메라 처리
    void LateUpdate()
    {
        if(target == null)
        {
            return;
        }

        nowPosition = transform.position;
        targetPosition = new Vector3(target.position.x, target.position.y, nowPosition.z);

        float distX = targetPosition.x - nowPosition.x;
        if(Mathf.Abs(distX) > followBorderX)
        {
            nowPosition.x = Mathf.Lerp(nowPosition.x, targetPosition.x, smoothSpeed * Time.deltaTime);
        }

        float distY = targetPosition.y - nowPosition.y;
        if(Mathf.Abs(distY) > followBorderY)
        {
            nowPosition.y = Mathf.Lerp(nowPosition.y,targetPosition.y, smoothSpeed * Time.deltaTime);   
        }

        nowPosition.x = Mathf.Clamp(nowPosition.x, minBorder.x, maxBorder.x);
        nowPosition.y = Mathf.Clamp(nowPosition.y, minBorder.y, maxBorder.y);

        transform.position = nowPosition;
    }
}
