using UnityEngine;

public static class TransformExtensions
{
    public static void ResetLocal(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void SetPositionX(this Transform transform, float x)
    {
        Vector3 position = transform.position;
        position.x = x;
        transform.position = position;
    }

    public static void SetPositionY(this Transform transform, float y)
    {
        Vector3 position = transform.position;
        position.y = y;
        transform.position = position;
    }

    public static void SetPositionZ(this Transform transform, float z)
    {
        Vector3 position = transform.position;
        position.z = z;
        transform.position = position;
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public static void DestroyAllChildrenImmediate(this Transform transform)
    {
        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public static Transform FindDeepChild(this Transform parent, string name)
    {
        Transform result = parent.Find(name);

        if (result != null)
        {
            return result;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            result = child.FindDeepChild(name);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
