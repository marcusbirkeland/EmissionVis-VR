using UnityEngine;
using System;
using UnityEngine.InputSystem;

/// <summary>
/// Dynamically positions an invisible wall for linear teleportation in full scale.
/// </summary>
public class TeleportWallPositioning : MonoBehaviour
{
    /// <summary>
    /// Make sure the controller's raycast maxDistance is set to a value higher than this variable
    /// </summary>
    [SerializeField] private float maxTeleportDistance = 900.0f;

    [SerializeField] private float minTeleportDistance = 30.0f;

    /// <summary>
    /// The teleport bumper on the right controller used for instant teleportation.
    /// </summary>
    [SerializeField] private InputActionReference teleportBumper;

    /// <summary>
    /// The teleport joystick in the right controller used for teleportation.
    /// </summary>
    [SerializeField] private InputActionReference teleportJoystick;

    
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        teleportBumper.action.performed += Pressed;
        teleportJoystick.action.performed += Pressed;
    }

    
    /// <summary>
    /// Handles a controller teleport action. Positions the teleport wall according to the player's 
    /// height above ground level.
    /// </summary>
    /// <param name="context">A callback to be executed once a controller action has been performed.</param>
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
