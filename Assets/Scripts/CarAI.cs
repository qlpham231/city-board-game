using UnityEngine;

public class CarAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float reachThreshold = 1f;
    public float rotationSpeed = 5f;

    private int currentWaypointIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Raycast down to get the surface normal of the road beneath the car
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 3f))
        {
            Vector3 surfaceNormal = hit.normal;

            // Calculate the movement direction toward the target waypoint
            Vector3 moveDirection = (target.position - transform.position).normalized;

            // Calculate the rotation using LookRotation with the surface normal to align with the road
            Quaternion surfaceRotation = Quaternion.LookRotation(moveDirection, surfaceNormal);

            transform.rotation = Quaternion.Slerp(transform.rotation, surfaceRotation, Time.deltaTime * rotationSpeed);
        }

        if (Vector3.Distance(transform.position, target.position) < reachThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop
        }
    }
}