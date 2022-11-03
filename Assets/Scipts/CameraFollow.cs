using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform MegaMan;
    [SerializeField] float timeOffset;
    [SerializeField] Vector3 offsetPos;

    [SerializeField] Vector3 boundsMin;
    [SerializeField] Vector3 boundsMax;

    private void LateUpdate()
    {
        //If character exists clamp and follow it
        if (MegaMan != null)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = MegaMan.position;

            targetPos.x += offsetPos.x;
            targetPos.y += offsetPos.y;
            targetPos.z = transform.position.z;

            targetPos.x = Mathf.Clamp(targetPos.x, boundsMin.x, boundsMax.x);
            targetPos.y = Mathf.Clamp(targetPos.y, boundsMin.y, boundsMax.y);

            float t = 1f - Mathf.Pow(1f - timeOffset, Time.deltaTime * 30);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
        }
    }
}