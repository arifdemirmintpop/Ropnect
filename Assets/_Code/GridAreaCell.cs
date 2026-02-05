using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class GridAreaCell : MonoBehaviour
{
    // grid coordinate (x,z)
    public Vector2 coordinate;
    public Collider selectCollider;

    [SerializeField]
    private ReelController _reel;

    public ReelController reel
    {
        get => _reel;
        set
        {
            if (_reel == value) return;
            _reel = value;
            UpdateColliderEnabled();
        }
    }

    // helper to set coordinate
    public void SetCoordinate(int x, int z)
    {
        coordinate = new Vector2(x, z);
        gameObject.name = $"Cell_{x}_{z}";
    }

    void UpdateColliderEnabled()
    {
        bool shouldEnable = _reel != null;
        selectCollider.enabled = shouldEnable;
        return;
    }
}
