using UnityEngine;

public class PathRoute : MonoBehaviour
{
    [Header("Hiển thị đường đi trong Scene")]
    public Color gizmoColor = Color.green;
    public float pointRadius = 0.15f;

    public Transform[] GetWaypoints()
    {
        Transform[] points = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            points[i] = transform.GetChild(i);
        }

        return points;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentPoint = transform.GetChild(i);
            Gizmos.DrawSphere(currentPoint.position, pointRadius);

            if (i < transform.childCount - 1)
            {
                Transform nextPoint = transform.GetChild(i + 1);
                Gizmos.DrawLine(currentPoint.position, nextPoint.position);
            }
        }
    }
}