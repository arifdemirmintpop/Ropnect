using UnityEngine;
using System.Collections;

namespace tiplay.SnapTool
{
    public static class SnapManager
    {
        public static bool GetSnapLocation(Transform transform, SnapSettings settings, out Vector3 position, out Quaternion rotation)
        {
            position = transform.position;
            rotation = transform.rotation;

            Ray ray = new Ray(transform.TransformPoint(-settings.positionOffset), Vector3.down);

            if (settings.raySpace == Space.Self)
                ray.direction = transform.TransformDirection(Vector3.down);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, settings.layerMask, settings.triggerInteraction))
                return false;

            position = GetSnapPoint(transform, hit, settings);

            if (!settings.keepRotation)
                rotation = GetNormalRotation(transform, hit, settings);

            return true;
        }

        public static bool GetSnapLocation(Transform transform, RaycastHit hit, SnapSettings settings, out Vector3 position, out Quaternion rotation)
        {
            position = transform.position;
            rotation = transform.rotation;

            if (!hit.collider)
                return false;

            position = GetSnapPoint(transform, hit, settings);

            if (!settings.keepRotation)
                rotation = GetNormalRotation(transform, hit, settings);

            return true;
        }

        private static Vector3 GetSnapPoint(Transform transform, RaycastHit hit, SnapSettings settings)
        {
            if (!hit.collider)
                return transform.position;

            Vector3 offset = GetSnapPointOffset(transform, hit, settings);

            return hit.point + offset;
        }

        private static Vector3 GetSnapPointOffset(Transform transform, RaycastHit hit, SnapSettings settings)
        {
            if (settings.keepRotation)
                return transform.TransformVector(settings.positionOffset);

            Quaternion normalRotation = GetNormalRotation(transform, hit, settings);
            Vector3 offset = normalRotation * settings.positionOffset;

            //if (settings.addBoundsOffset)
            //{
            //    var bounds = BoundsExtensions.GetBounds(transform);
            //    offset += normalRotation * new Vector3(0, bounds.extents.y, 0);
            //}

            return offset;
        }

        private static Quaternion GetNormalRotation(Transform transform, RaycastHit hit, SnapSettings settings)
        {
            if (!hit.collider)
                return transform.rotation;

            Quaternion normalRotation = Quaternion.LookRotation(hit.normal);
            normalRotation *= Quaternion.Euler(90, 0, 0);

            Vector3 normalForward = normalRotation * Vector3.forward;
            normalForward.y = transform.forward.y;
            float angle = Vector3.SignedAngle(transform.forward, normalForward, transform.up);
            Quaternion rotOff = Quaternion.Euler(0, -angle, 0);

            normalRotation *= rotOff;
            normalRotation *= Quaternion.Euler(settings.rotationAngleOffset);
            return normalRotation;
        }
    }
}