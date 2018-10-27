using UnityEngine;
#pragma warning disable CS1591 
public abstract class ExtendedMonoBehaviour : MonoBehaviour
{
    public new Transform transform
    {
        get
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = base.transform;
            }
            return CachedTransform;
        }
    }

 
    protected Transform CachedTransform;

    protected virtual void Awake()
    {
        if ((object)CachedTransform == null)
        {
            CachedTransform = base.transform;
        }
    }

    public static float CalcDistance(Vector3 pos1, Vector3 pos2)
    {
        Vector3 heading;
        float distanceSquared;
        float distance;

        heading.x = pos1.x - pos2.x;
        heading.y = pos1.y - pos2.y;
        heading.z = pos1.z - pos2.z;

        distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
        distance = Mathf.Sqrt(distanceSquared);

        return distance;
    }

    public static float GetPercentOfValue(float desiredPercent, float value)
    {
        return value / 100 * desiredPercent;
    }

    public static string KiloFormat(float num)
    {
        if (num >= 1000000000)
            return (num / 1000000000).ToString("#.0" + "B");

        if (num >= 1000000)
            return (num / 1000000).ToString("#" + "M");

        if (num >= 100000)
            return (num / 1000).ToString("#.0" + "K");

        if (num >= 10000)
            return (num / 1000).ToString("0.#" + "K");

        if (num >= 1000)
            return (num / 1000).ToString("0.#" + "K");

        return num.ToString("0.#");
    }
}
