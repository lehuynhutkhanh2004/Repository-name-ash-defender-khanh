using UnityEngine;

public class NKEnemyWaypointMover : MonoBehaviour
{
    public NKPathContainer path;
    public float moveSpeed = 2f;
    public bool destroyAtEnd = true;

    private Transform[] points;
    private int currentIndex;

    private void Start()
    {
        if (path != null)
        {
            points = path.Points;
            if (points.Length > 0)
            {
                transform.position = points[0].position;
                currentIndex = 1;
            }
        }
    }

    private void Update()
    {
        if (points == null || points.Length == 0 || currentIndex >= points.Length)
        {
            return;
        }

        Transform target = points[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.localScale = new Vector3(direction.x < 0 ? -1f : 1f, 1f, 1f);
        }

        if (Vector3.Distance(transform.position, target.position) <= 0.05f)
        {
            currentIndex++;
            if (currentIndex >= points.Length && destroyAtEnd)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetPath(NKPathContainer newPath)
    {
        path = newPath;
        points = path != null ? path.Points : null;
        currentIndex = 0;
        if (points != null && points.Length > 0)
        {
            transform.position = points[0].position;
            currentIndex = 1;
        }
    }
}
