using UnityEngine;
using Microsoft.Maps.Unity;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MapDraggingHandler : MonoBehaviour
{
    public GameObject map;
    public ActionBasedController rightController;
    public ActionBasedController leftController;
    public float dragSpeed = 70.0f;

    private MapRenderer _target;
    private Collider _targetCollider;
    private MapInteractionController _interactionController; 
    private XRRayInteractor _rightRayInteractor;
    private XRRayInteractor _leftRayInteractor;

    private Vector3 _rightPreviousPos;
    private Vector3 _leftPreviousPos;

    private bool _rightEnabled;
    private bool _leftEnabled;

    private void Start()
    {
        _target = map.GetComponent<MapRenderer>();
        _targetCollider = map.GetComponent<Collider>();
        _interactionController = map.GetComponent<MapInteractionController>();
        _rightRayInteractor = InitController(rightController, RightPressed, RightReleased);
        _leftRayInteractor = InitController(leftController, LeftPressed, LeftReleased);
    }


    private void Update()
    {
        if (!_rightEnabled && !_leftEnabled) return;

        Vector3 rightHit = _rightEnabled ? GetRayHit(_rightRayInteractor) : Vector3.zero;
        Vector3 leftHit = _leftEnabled ? GetRayHit(_leftRayInteractor) : Vector3.zero;
        if (rightHit == Vector3.zero && _rightEnabled) { _rightPreviousPos = Vector3.zero; return; }
        if (leftHit == Vector3.zero && _leftEnabled) { _leftPreviousPos = Vector3.zero; return; }

        Vector3 deltaRight = _rightEnabled ? Delta(rightHit, ref _rightPreviousPos) : Vector3.zero;
        Vector3 deltaLeft = _leftEnabled ? Delta(leftHit, ref _leftPreviousPos) : Vector3.zero;

        Vector3 pan = deltaRight + deltaLeft;
        TranslateMap(pan * (_rightEnabled && _leftEnabled ? 0.5f : 1.0f));

        if (!_rightEnabled || !_leftEnabled) return;

        float deltaZoom = 0;
        deltaZoom += (deltaRight.x - deltaLeft.x) * (leftHit.x > rightHit.x ? 1 : -1);
        deltaZoom += (deltaRight.y - deltaLeft.y) * (leftHit.y > rightHit.y ? 1 : -1);
        deltaZoom += (deltaRight.z - deltaLeft.z) * (leftHit.z > rightHit.z ? 1 : -1);

        Vector3 midpoint = (rightHit + leftHit) / 2;

        if ((_target.ZoomLevel + deltaZoom) < 3.0f) return;
        _target.ZoomLevel += deltaZoom;
        TranslateMap(midpoint * deltaZoom * (_target.MapDimension.x / 2));
    }


    private static XRRayInteractor InitController(Component controller, Action<InputAction.CallbackContext> pressed, Action<InputAction.CallbackContext> released)
    {
        InputActionReference selectReference = controller.GetComponent<ActionBasedController>().activateAction.reference;
        XRRayInteractor rayInteractor = controller.transform.GetChild(2).gameObject.GetComponent<XRRayInteractor>();

        selectReference.action.started += pressed;
        selectReference.action.canceled += released;

        return rayInteractor;
    }

    private void RightPressed(InputAction.CallbackContext context)
    {
        _rightEnabled = true;
        // Reset the previous position to some "null" value which can be used in comparisons
        _rightPreviousPos = Vector3.zero;
    }

    private void LeftPressed(InputAction.CallbackContext context)
    {
        _leftEnabled = true;
        _leftPreviousPos = Vector3.zero;
    }

    private void RightReleased(InputAction.CallbackContext context)
    {
        _rightEnabled = false;
    }

    private void LeftReleased(InputAction.CallbackContext context)
    {
        _leftEnabled = false;
    }

    private Vector3 GetRayHit(XRRayInteractor rayInteractor)
    {
        if (!rayInteractor.TryGetCurrentRaycast(out RaycastHit? raycastHit, out int _, out RaycastResult? uiRaycastHit, out int _, out bool _)) return Vector3.zero;
        if (uiRaycastHit is { }) return Vector3.zero;
        if (raycastHit is { } hit)
        {
            return hit.collider != _targetCollider ? Vector3.zero : map.transform.InverseTransformPoint(hit.point);

            // transform the raycast hit in world space to the map's local space
        }
        return Vector3.zero;
    }

    private static Vector3 Delta(Vector3 hit, ref Vector3 previousHit)
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
        // Rotate the delta by the map's current Y-axis rotation
        float mapRotationY = map.transform.localEulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, mapRotationY, 0);
        Vector3 rotatedDelta = rotation * delta;

        // Interpreted as a change in Vector2(latitude, longitude)
        _interactionController.Pan(new Vector2(rotatedDelta.x, rotatedDelta.z) * dragSpeed, true);
    }
}
