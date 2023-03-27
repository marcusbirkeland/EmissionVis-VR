using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Maps.Unity;
using Microsoft.Geospatial;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class MapDragging : MonoBehaviour
{
    public GameObject map;
    public ActionBasedController rightController;
    public ActionBasedController leftController;
    public float dragSpeed = 70.0f;

    private MapRenderer target;
    private Collider targetCollider;
    private MapInteractionController interactionController; 
    private XRRayInteractor rightRayInteractor;
    private XRRayInteractor leftRayInteractor;

    private Vector3 rightPreviousPos;
    private Vector3 leftPreviousPos;

    private bool rightEnabled = false;
    private bool leftEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        target = map.GetComponent<MapRenderer>();
        targetCollider = map.GetComponent<Collider>();
        interactionController = map.GetComponent<MapInteractionController>();
        rightRayInteractor = InitController(rightController, RightPressed, RightReleased);
        leftRayInteractor = InitController(leftController, LeftPressed, LeftReleased);
    }

    private XRRayInteractor InitController(ActionBasedController controller, Action<InputAction.CallbackContext> pressed, Action<InputAction.CallbackContext> released)
    {
        InputActionReference selectReference = controller.GetComponent<ActionBasedController>().activateAction.reference;
        XRRayInteractor rayInteractor = controller.transform.GetChild(2).gameObject.GetComponent<XRRayInteractor>();

        selectReference.action.started += pressed;
        selectReference.action.canceled += released;

        return rayInteractor;
    }

    private void RightPressed(InputAction.CallbackContext context)
    {
        rightEnabled = true;
        // Reset the previous position to some "null" value which can be used in comparisons
        rightPreviousPos = Vector3.zero;
    }

    private void LeftPressed(InputAction.CallbackContext context)
    {
        leftEnabled = true;
        leftPreviousPos = Vector3.zero;
    }

    private void RightReleased(InputAction.CallbackContext context)
    {
        rightEnabled = false;
    }

    private void LeftReleased(InputAction.CallbackContext context)
    {
        leftEnabled = false;
    }

    private Vector3 GetRayHit(XRRayInteractor rayInteractor)
    {
        RaycastHit hit;
        if (!rayInteractor.TryGetCurrent3DRaycastHit(out hit)) return Vector3.zero;

        if (hit.collider != targetCollider) return Vector3.zero;

        // transform the raycast hit in world space to the map's local space
        return map.transform.InverseTransformPoint(hit.point);
    }

    private Vector3 Delta(Vector3 hit, ref Vector3 previousHit)
    {
        // If "previousHit" is not set to a valid hit we want the panning in the first frame to be stationary.
        // This to avoid large jumps going from either Vector3.zero to "hit", or from "previousHit" from the last 
        // button press to "hit" (which could be in two entirely different locations).
        if (previousHit == Vector3.zero) previousHit = hit;
        Vector3 deltaHit = previousHit - hit;
        previousHit = hit;
        return deltaHit;
    }

    private void TranslateMap(Vector3 delta)
    {
        // Interpreted as a change in Vector2(latitude, longitude)
        interactionController.Pan(new Vector2(delta.z, -delta.x) * dragSpeed, true);
    }

    // Update is called once per frame
    void Update()
    {
        // hit points:
        Vector3 rightHit = Vector3.zero;
        Vector3 leftHit = Vector3.zero;
        Vector3 pan = Vector3.zero;
        Vector3 deltaRight = Vector3.zero;
        Vector3 deltaLeft = Vector3.zero;
        if (rightEnabled)
        {
            rightHit = GetRayHit(rightRayInteractor);
            if (rightHit == Vector3.zero) { rightPreviousPos = Vector3.zero; return; }
            deltaRight = Delta(rightHit, ref rightPreviousPos);
            if (rightEnabled) pan += deltaRight;
        }
        if (leftEnabled)
        {
            leftHit = GetRayHit(leftRayInteractor);
            if (leftHit == Vector3.zero) { leftPreviousPos = Vector3.zero; return; }
            deltaLeft = Delta(leftHit, ref leftPreviousPos);
            if (leftEnabled) pan += deltaLeft;
        }

        // If panning is done by both controllers, pan by the average of their delta movement
        TranslateMap(pan * (rightEnabled && leftEnabled ? 0.5f : 1.0f));

        if (!rightEnabled || !leftEnabled) return;
        
        float deltaZoom = 0;
        deltaZoom += (float)((deltaRight.x - deltaLeft.x) * (leftHit.x > rightHit.x ? 1 : -1));
        deltaZoom += (float)((deltaRight.y - deltaLeft.y) * (leftHit.y > rightHit.y ? 1 : -1));
        deltaZoom += (float)((deltaRight.z - deltaLeft.z) * (leftHit.z > rightHit.z ? 1 : -1));

        Vector3 midpoint = (rightHit + leftHit) / 2;

        if ((target.ZoomLevel + deltaZoom) < 3.0f) return; 
        target.ZoomLevel += deltaZoom;

        // panning to the midpoint of the two controller rays that hit the map
        // This equation assumes a 1 to 1 mapping between the ray and map pan (i.e. ray hitpoint
        // stays in the same geographical location when panning) to work correctly.
        // Increasing ZoomLevel by 1 means doubling the zoom, i.e. we need to move in the direction 
        // "midpoint" by an amount "deltaZoom" * half the size of the map. 
        TranslateMap(midpoint * deltaZoom * (target.MapDimension.x / 2));
    }
}
