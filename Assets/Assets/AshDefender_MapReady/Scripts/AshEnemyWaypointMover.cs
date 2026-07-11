using UnityEngine;

/// <summary>
/// Optional test script. Add this to an enemy object and assign a PathContainer.
/// The enemy will move from the first waypoint to the final waypoint.
/// </summary>
public class AshEnemyWaypointMover : MonoBehaviour
{
    [SerializeField] private AshPathContainer path;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private bool destroyAtEnd = true;

    private Transform[] points;
    private int currentIndex;

    public void Setup(AshPathContainer newPath, float speed)
    {
        path = newPath;
        moveSpeed = speed;
        InitializePath();
    }

    private void Start()
    {
        InitializePath();
    }

    private void InitializePath()
    {
        if (path == null)
        {
            Debug.LogWarning($"{name}: Path is missing.");
            enabled = false;
            return;
        }

        points = path.Waypoints;

        if (points == null || points.Length == 0)
        {
            Debug.LogWarning($"{name}: Path has no waypoints.");
            enabled = false;
            return;
        }

        transform.position = points[0].position;
        currentIndex = 1;
    }

    private void Update()
    {
        if (points == null || currentIndex >= points.Length)
        {
            if (destroyAtEnd)
            {
                Destroy(gameObject);
            }
            else
            {
                enabled = false;
            }
            return;
        }

        Transform target = points[currentIndex];
        if (target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentIndex++;
        }
    }
}
