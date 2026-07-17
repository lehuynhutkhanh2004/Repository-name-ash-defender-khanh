using UnityEngine;

public class NKPathContainer : MonoBehaviour
{
    public Transform[] Points
    {
        get
        {
            Transform[] points = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                points[i] = transform.GetChild(i);
            }
            return points;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform current = transform.GetChild(i);
            Gizmos.DrawSphere(current.position, 0.18f);
            if (i < transform.childCount - 1)
            {
                Transform next = transform.GetChild(i + 1);
                Gizmos.DrawLine(current.position, next.position);
            }
        }
    }
}
