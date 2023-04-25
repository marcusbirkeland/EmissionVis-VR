using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class TeleportWallPositioning : MonoBehaviour
{
    // if changing maxTeleportDistance make sure the controller's raycast maxDistance is changed accordingly (raycast maxDistance >= this maxTeleportDistance)
    private float maxTeleportDistance = 999.0f;
    private float minTeleportDistance = 25.0f;
    public float distanceScale = 1.0f;
    public ActionBasedController rightController;

    void Start()
    {
        rightController.GetComponent<ActionBasedController>().selectAction.reference.action.performed += Pressed;
    }

    private void Pressed(InputAction.CallbackContext context)
    {
        // make sure the teleport wall does not interfere with any raycasting to the terrain
        int cachedLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Transform xrOrigin = transform.parent.parent.parent;
        // raycast from under the terrain upwards as to not accedentally hit the teleport wall
        Ray ray = new Ray(xrOrigin.position, Vector3.up * -1);
        float heightAboveTerrain = transform.position.y; // fall back value if ray cast for some reason doesn't work
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            heightAboveTerrain = xrOrigin.position.y - hit.point.y;
        }
        // reset to normal layer after raycasting
        gameObject.layer = cachedLayer;

        // make the minimun teleport distance kick in at 50 units before the player is below the threshold, as to make teleporting more controlled at lower altitudes
        Vector3 TeleportWallDistance = distanceScale * Math.Max(heightAboveTerrain - 50, minTeleportDistance) * transform.forward;

        if (TeleportWallDistance.magnitude > maxTeleportDistance)
        {
            TeleportWallDistance = TeleportWallDistance * (maxTeleportDistance / TeleportWallDistance.magnitude);
        }

        transform.position = TeleportWallDistance + transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
