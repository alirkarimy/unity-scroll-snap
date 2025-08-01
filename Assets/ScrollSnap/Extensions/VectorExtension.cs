using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension 
{

    public static Vector3 RemoveZ(this Vector3 vector3)
    {
        vector3.z = 0;
        return vector3;
    }
    public static Vector3 ReplaceZ(this Vector3 vector3, float newZ)
    {
        vector3.z = newZ;
        return vector3;
    }

    public static Vector3 ReverseY(this Vector3 vector3)
    {
        vector3.y *= -1;
        return vector3;
    }

    public static Vector3 ReplaceYZ(this Vector3 vector3, Vector3 newYZ)
    {
        vector3.y = newYZ.y;
        vector3.z = newYZ.z;
        return vector3;
    }
    public static Vector2 AddZ(this Vector2 vector2, float z)
    {
        Vector3 castedVector3 = vector2;
        castedVector3.z = z;
        return castedVector3;
    }

    public static Vector3 MultiplyElements(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3 DevideElements(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
}
