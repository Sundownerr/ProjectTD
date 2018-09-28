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
}
