using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

public class RopeController : MonoBehaviour
{
    public List<Transform> armatures = new List<Transform>();

    public float totalDuration = 2f;

    public float jointAvoidance = 0.01f;

    public float staggerFactor = 0.05f;

    public bool autoPopulateChildren = true;

    public Vector3[] previewSegments;

    void Awake()
    {
        if (autoPopulateChildren && (armatures == null || armatures.Count == 0))
        {
            armatures = new List<Transform>();
            foreach (Transform t in transform)
            {
                armatures.Add(t);
            }
        }
    }

    public void AnimateAlongSegments(Vector3[] segmentsLocal)
    {
        if (segmentsLocal == null || segmentsLocal.Length == 0)
        {
            Debug.LogWarning("AnimateAlongSegments: no segments provided.");
            return;
        }

        if (armatures == null || armatures.Count == 0)
        {
            Debug.LogWarning("AnimateAlongSegments: no armatures assigned.");
            return;
        }

        Vector3[] targets = ComputeTargets(segmentsLocal);

        // Animate each armature to its target
        int count = armatures.Count;
        for (int i = 0; i < count; i++)
        {
            Transform a = armatures[i];
            if (a == null) continue;

            float delay = staggerFactor * i * (totalDuration / Mathf.Max(1, count));
            float duration = totalDuration; // all tweens run for same duration by default

#if DOTWEEN
            // DOTween path-like move to the target position
            a.DOMove(targets[i], duration).SetDelay(delay).SetEase(Ease.OutSine);
#else
            // Coroutine fallback when DOTween isn't available
            StartCoroutine(MoveTransformSmooth(a, targets[i], duration, delay));
#endif
        }
    }

    /// <summary>
    /// Compute evenly spaced target positions along the polyline defined by the given local-space segments.
    /// This is reused by both runtime animation and the editor preview.
    /// </summary>
    Vector3[] ComputeTargets(Vector3[] segmentsLocal)
    {
        // Build world-space polyline points from local segments
        List<Vector3> points = new List<Vector3>();
        Vector3 p = transform.position;
        points.Add(p);
        foreach (var segLocal in segmentsLocal)
        {
            p += transform.TransformVector(segLocal);
            points.Add(p);
        }

        // Compute cumulative lengths
        int segCount = Mathf.Max(0, points.Count - 1);
        float[] segLengths = new float[segCount];
        float totalLength = 0f;
        for (int i = 0; i < segCount; i++)
        {
            float l = Vector3.Distance(points[i], points[i + 1]);
            segLengths[i] = l;
            totalLength += l;
        }

        int count = (armatures != null) ? armatures.Count : 0;
        if (count == 0)
        {
            Debug.LogWarning("ComputeTargets: no armatures assigned.");
            return new Vector3[0];
        }

        if (totalLength <= 0.0001f)
        {
            Debug.LogWarning("ComputeTargets: total path length is zero.");
            // return all at start
            Vector3[] same = new Vector3[count];
            for (int i = 0; i < count; i++) same[i] = points[0];
            return same; 
        }

        float spacing = (count > 1) ? totalLength / (count - 1) : 0f;

        // Generate target positions sampled along the polyline. Ensure we don't place a sample exactly on a joint.
        Vector3[] targets = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            float s = spacing * i; // distance along the path

            // If s coincides with a joint (within jointAvoidance), nudge it slightly backward (towards previous segment)
            float cumulative = 0f;
            for (int seg = 0; seg < segCount; seg++)
            {
                cumulative += segLengths[seg];
                if (Mathf.Abs(s - cumulative) <= jointAvoidance)
                {
                    // Nudge by a fraction of spacing. If this is the first sample, nudge forward instead.
                    float nudge = Mathf.Min(jointAvoidance, spacing * 0.4f);
                    if (i == 0)
                        s += nudge;
                    else
                        s -= nudge;
                    break;
                }
            }

            targets[i] = GetPointAtDistance(points, segLengths, s);
        }

        return targets;
    }

    [ContextMenu("Preview Segments")]
    public void PreviewSegments()
    {
        if (previewSegments == null || previewSegments.Length == 0)
        {
            Debug.LogWarning("PreviewSegments: no previewSegments assigned.");
            return;
        }

        if (armatures == null || armatures.Count == 0)
        {
            // try to auto-populate if allowed
            if (autoPopulateChildren)
            {
                armatures = new List<Transform>();
                foreach (Transform t in transform)
                {
                    armatures.Add(t);
                }
            }
        }

        Vector3[] targets = ComputeTargets(previewSegments);
        if (targets.Length == 0) return;

        // Immediately set positions (editor-friendly preview, no tween)
        int count = Mathf.Min(targets.Length, armatures.Count);
        for (int i = 0; i < count; i++)
        {
            var a = armatures[i];
            if (a == null) continue;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(a, "RopeController Preview Segments");
            a.position = targets[i];
            UnityEditor.EditorUtility.SetDirty(a);
#else
            a.position = targets[i];
#endif
        }

        Debug.Log("PreviewSegments: applied preview positions to armatures.");
    }

    // Helper: map a distance along the polyline to a world position
    Vector3 GetPointAtDistance(List<Vector3> points, float[] segLengths, float distance)
    {
        if (distance <= 0f) return points[0];

        float remaining = distance;
        for (int i = 0; i < segLengths.Length; i++)
        {
            if (remaining <= segLengths[i])
            {
                float t = (segLengths[i] <= 0f) ? 0f : (remaining / segLengths[i]);
                return Vector3.Lerp(points[i], points[i + 1], t);
            }
            remaining -= segLengths[i];
        }

        return points[points.Count - 1];
    }

    IEnumerator MoveTransformSmooth(Transform t, Vector3 target, float duration, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        Vector3 start = t.position;
        if (duration <= 0f)
        {
            t.position = target;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            t.position = Vector3.Lerp(start, target, alpha);
            yield return null;
        }

        t.position = target;
    }
}