using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Functions : MonoBehaviour
{
    public static Vector2 RotateByAngle(Vector2 vector, float angle)
    {
        Vector2 nv;
        float theta = angle * Mathf.Deg2Rad;
        float cs = Mathf.Cos(theta);
        float sn = Mathf.Sin(theta);
        nv.x = vector.x * cs - vector.y * sn;
        nv.y = vector.x * sn + vector.y * cs;
        return nv;
    }
    //Added function to help with AI movement
    public static Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
