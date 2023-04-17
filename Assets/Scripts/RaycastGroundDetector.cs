using UnityEngine;

public class RaycastGroundDetector : MonoBehaviour
{
    public float raycastDistance;

    private void Update()
    {
        Ray ray = new(transform.position, Vector3.down);
        
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 1f);


        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            Debug.Log("Raycast hit the ground at position: " + hit.point);
        }
    }
}