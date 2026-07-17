using UnityEngine;

public class NKMapMarker : MonoBehaviour
{
    public enum MarkerType
    {
        Spawn,
        BossSpawn,
        Core,
        Waypoint,
        HeroArea,
        Container,
        Trap,
        UI
    }

    public MarkerType markerType = MarkerType.Waypoint;
    public float gizmoSize = 0.35f;
    public string note;

    private Color GetColor()
    {
        switch (markerType)
        {
            case MarkerType.Spawn: return new Color(1f, 0.35f, 0.15f, 0.9f);
            case MarkerType.BossSpawn: return new Color(0.7f, 0.1f, 1f, 0.9f);
            case MarkerType.Core: return new Color(0.2f, 0.9f, 1f, 0.9f);
            case MarkerType.HeroArea: return new Color(0.1f, 1f, 0.25f, 0.45f);
            case MarkerType.Trap: return new Color(1f, 0f, 0f, 0.8f);
            case MarkerType.Container: return new Color(0.8f, 0.8f, 0.8f, 0.6f);
            case MarkerType.UI: return new Color(0.35f, 0.55f, 1f, 0.8f);
            default: return new Color(1f, 0.9f, 0.1f, 0.9f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GetColor();
        if (markerType == MarkerType.HeroArea)
        {
            Gizmos.DrawCube(transform.position, new Vector3(1.2f, 1.2f, 0.05f));
        }
        else if (markerType == MarkerType.Core)
        {
            Gizmos.DrawWireSphere(transform.position, gizmoSize * 1.7f);
            Gizmos.DrawSphere(transform.position, gizmoSize * 0.8f);
        }
        else
        {
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
    }
}
