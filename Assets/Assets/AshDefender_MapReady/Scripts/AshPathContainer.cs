using UnityEngine;

/// <summary>
/// Holds waypoint transforms for an enemy path and draws the path in Scene view.
/// Enemy spawners/movers can read Waypoints to move from spawn to Core.
/// </summary>
public class AshPathContainer : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Color gizmoColor = Color.yellow;
    [SerializeField] private float gizmoPointRadius = 0.15f;

    public Transform[] Waypoints => waypoints;

    public void SetWaypoints(Transform[] points)
    {
        waypoints = points;
    }

    private void OnDrawGizmos()
    {
        Transform[] points = waypoints;

        if (points == null || points.Length == 0)
        {
            points = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                points[i] = transform.GetChild(i);
            }
        }

        if (points == null || points.Length == 0) return;

        Gizmos.color = gizmoColor;

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null) continue;
            Gizmos.DrawSphere(points[i].position, gizmoPointRadius);

            if (i < points.Length - 1 && points[i + 1] != null)
            {
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
        }
    }
}
