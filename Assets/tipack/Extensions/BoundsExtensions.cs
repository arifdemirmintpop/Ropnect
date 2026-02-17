using UnityEngine;
using System.Collections;

public static class BoundsExtensions
{
    public static Bounds GetBounds(Transform transform)
    {
        Quaternion rot = transform.rotation;
        transform.rotation = Quaternion.identity;

        var bounds = new Bounds(transform.position, Vector3.zero);
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
            bounds.Encapsulate(renderer.bounds);

        transform.rotation = rot;
        return bounds;
    }
}

