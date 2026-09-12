using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 WithX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }

    public static Vector3 WithY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }

    public static Vector3 WithZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    public static Vector2 ToVector2XY(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector2 ToVector2XZ(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static float DistanceXZ(this Vector3 a, Vector3 b)
    {
        float deltaX = a.x - b.x;
        float deltaZ = a.z - b.z;

        return Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);
    }

    public static bool IsWithinDistance(this Vector3 a, Vector3 b, float distance)
    {
        float sqrDistance = (a - b).sqrMagnitude;
        float sqrThreshold = distance * distance;

        if (sqrDistance <= sqrThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    public static Vector3 RandomPointInRadius(this Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;

        return new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);
    }

    public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }
}
