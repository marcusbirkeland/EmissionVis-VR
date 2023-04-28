using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class TeleportWallPositioning : MonoBehaviour
{
    // if changing maxTeleportDistance make sure the controller's raycast maxDistance is changed accordingly (raycast maxDistance >= this maxTeleportDistance)
    [SerializeField] private float maxTeleportDistance = 900.0f;
    [SerializeField] private float minTeleportDistance = 30.0f;
    [SerializeField] private InputActionReference teleportBumper;
    [SerializeField] private InputActionReference teleportJoystick;

    private void Start()
    {
        teleportBumper.action.performed += Pressed;
        teleportJoystick.action.performed += Pressed;
    }

    private void Pressed(InputAction.CallbackContext context)
    {
        // make sure the teleport wall does not interfere with any raycasting to the terrain
        int cachedLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Transform xrOrigin = transform.parent.parent.parent;
        // raycast from under the terrain upwards as to not accedentally hit the teleport wall
        Ray ray = new(xrOrigin.position, Vector3.up * -1);
        float heightAboveTerrain = transform.position.y; // fall back value if ray cast for some reason doesn't work
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            heightAboveTerrain = xrOrigin.position.y - hit.point.y;
        }
        // reset to normal layer after raycasting
        gameObject.layer = cachedLayer;

        // make the minimum teleport distance kick in at 20 units before the player is below the threshold, as to make teleporting more controlled at lower altitudes
        Vector3 teleportWallDistance = Math.Max(heightAboveTerrain - 20, minTeleportDistance) * transform.forward;

        if (teleportWallDistance.magnitude > maxTeleportDistance)
        {
            teleportWallDistance *= maxTeleportDistance / teleportWallDistance.magnitude;
        }

        transform.position = teleportWallDistance + transform.parent.position;
    }
}
