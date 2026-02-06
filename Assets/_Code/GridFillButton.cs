using UnityEngine;
using DG.Tweening;

// Simple clickable button that moves its "buttonBody" up and down using DOTween when clicked.
// While the press animation is running the button is ignored for further clicks.
public class GridFillButton : MonoBehaviour
{
    [Tooltip("Transform that visually represents the button. If null, the GameObject's transform will be used.")]
    public Transform buttonBody;

    [Tooltip("Local offset applied when the button is pressed (relative to original local position).")]
    public Vector3 pressMove = new Vector3(0f, -0.1f, 0f);

    [Tooltip("Duration of the press (down) or release (up) tween in seconds.")]
    public float pressDuration = 0.12f;

    [Tooltip("Reference to GridAreaFiller to invoke when button is pressed. Assign this from the Inspector.")]
    public GridAreaFiller gridAreaFiller;

    bool isBusy = false;
    Vector3 originalLocalPos;
    Collider[] colliders;

    void Start()
    {
        if (buttonBody == null) buttonBody = transform;
        originalLocalPos = buttonBody.localPosition;
        colliders = GetComponents<Collider>();

        // Do NOT auto-find GridAreaFiller; require assignment from Inspector
        if (gridAreaFiller == null)
        {
            Debug.LogWarning("GridFillButton: GridAreaFiller is not assigned. Please assign a GridAreaFiller from the Inspector.");
            // disable this component to prevent accidental clicks when not configured
            enabled = false;
            return;
        }
    }

    void OnMouseDown()
    {
        if (isBusy) return;

        // make non-clickable during animation
        isBusy = true;
        if (colliders != null)
        {
            foreach (var c in colliders) if (c != null) c.enabled = false;
        }

        // build press + release sequence
        var seq = DOTween.Sequence();
        seq.Append(buttonBody.DOLocalMove(originalLocalPos + pressMove, pressDuration).SetEase(Ease.OutQuad));
        seq.Append(buttonBody.DOLocalMove(originalLocalPos, pressDuration).SetEase(Ease.InQuad));
        seq.OnComplete(() =>
        {
            // restore clickable state
            if (colliders != null)
            {
                foreach (var c in colliders) if (c != null) c.enabled = true;
            }

            isBusy = false;

            // trigger grid fill
            if (gridAreaFiller != null)
            {
                gridAreaFiller.FillGridRandomly();
            }
            else
            {
                Debug.LogWarning("GridFillButton: GridAreaFiller not assigned. This component should have been disabled in Start().");
            }
        });
    }
}
