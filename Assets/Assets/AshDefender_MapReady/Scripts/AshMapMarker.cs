using UnityEngine;

/// <summary>
/// Simple marker used only to show important map points in Scene view.
/// It does not affect gameplay unless you use the object in your own scripts.
/// </summary>
public class AshMapMarker : MonoBehaviour
{
    public enum MarkerType
    {
        Spawn,
        BossSpawn,
        Core,
        DeployPoint,
        Waypoint
    }

    [SerializeField] private MarkerType markerType;
    [SerializeField] private float radius = 0.25f;
    [SerializeField] private Color color = Color.white;

    public MarkerType Type => markerType;

    public void Setup(MarkerType type, Color markerColor, float markerRadius)
    {
        markerType = type;
        color = markerColor;
        radius = markerRadius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, radius * 1.35f);
    }
}
