using UnityEngine;

public class WindmillRotator : MonoBehaviour
{
    public float rotationSpeed = 100f; // Degrees per second

    void Update()
    {
        // Rotate around the local Z axis
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
