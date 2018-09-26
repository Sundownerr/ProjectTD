using UnityEngine;

public abstract class ExtendedMonoBehaviour : MonoBehaviour
{
    protected Transform CachedTransform;

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

    protected virtual void Awake()
    {
        if ((object)CachedTransform == null)
        {
            CachedTransform = base.transform;
        }
    }
}
