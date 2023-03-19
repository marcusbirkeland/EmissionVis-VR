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

    private MapRenderer target;
    private Collider targetCollider;
    public float zoomSpeed = 3;
    public float zoomSpeedF = 1; // TODO: remove
    private XRRayInteractor rightRayInteractor; 
    private XRRayInteractor leftRayInteractor;
    
    public double dragSpeed = 500; 
    private Vector3 rightPreviousPos;
    private Vector3 leftPreviousPos;

    private bool rightEnabled = false;
    private bool leftEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        target = map.GetComponent<MapRenderer>();
        targetCollider = map.GetComponent<Collider>();
        rightRayInteractor = InitController(rightController, RightPressed, RightReleased);
        leftRayInteractor = InitController(leftController, LeftPressed, LeftReleased);
        //enabled = false;
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
        rightPreviousPos = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void LeftPressed(InputAction.CallbackContext context)
    {
        leftEnabled = true;
        leftPreviousPos = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void RightReleased(InputAction.CallbackContext context)
    {
        rightEnabled = false;
    }

    private void LeftReleased(InputAction.CallbackContext context)
    {
        leftEnabled = false;
    }

    // Delta controller movement in world space, adjusted for map rotation
    private Vector3 GetRayHit(XRRayInteractor rayInteractor)
    {
        RaycastHit hit;
        if (!rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            return Vector3.zero;
        }

        if (hit.collider != targetCollider)
        {
            return Vector3.zero;
        }

        return hit.point;
    }

    private Vector3 Delta(Vector3 hit, ref Vector3 previousHit)
    {
        if (previousHit == Vector3.zero) previousHit = hit;
        Vector3 deltaHit = previousHit - hit;
        previousHit = hit;
        return deltaHit;
    }

    private LatLon TranslateMap(Vector3 delta)
    {
        // initially (if not rotated) target.transform.right = Vector3(1.0, 0.0, 0.0), correspongding to the positive direction of latitude movement
        Vector3 latVector = target.transform.right;
        float latVectorLength = latVector.x * delta.x + latVector.y * delta.y + latVector.z * delta.z;


        // initially (if not rotated) target.transform.forward = Vector3(0.0, 0.0, 1.0), correspongding to the positive direction of longitude movement
        Vector3 lonVector = target.transform.forward;
        float lonVectorLength = lonVector.x * delta.x + lonVector.y * delta.y + lonVector.z * delta.z;


        LatLon deltaLatLon = new LatLon(latVectorLength, lonVectorLength);

        double currentLat = target.Center.LatitudeInDegrees;
        double currentLon = target.Center.LongitudeInDegrees;

        // Increasing ZoomLevel by 1 means doubling the map zoom. To retain proper movement on
        // different ZoomLevels, divide by 2^ZoomLevel
        // https://learn.microsoft.com/en-us/bingmaps/articles/understanding-scale-and-resolution
        float correctedDragSpeed = (float)(dragSpeed / Math.Pow(2.0, target.ZoomLevel)) / target.transform.lossyScale.x;

        // divide by cos(lat) as to adjust for the change in circumference (lon) at different latitutes
        return new LatLon(currentLat + deltaLatLon.LongitudeInDegrees * correctedDragSpeed, currentLon +
            (deltaLatLon.LatitudeInDegrees / Math.Cos((Math.PI / 180) * currentLat)) * correctedDragSpeed);
    }

  
    // Update is called once per frame
    void Update()
    {
        // hit points:
        Vector3 rightHit = Vector3.zero;
        Vector3 leftHit = Vector3.zero;
        if (rightEnabled)
        {
            rightHit = GetRayHit(rightRayInteractor);
            if (rightHit == Vector3.zero) return;
        }
        if (leftEnabled)
        {
            leftHit = GetRayHit(leftRayInteractor);
            if (leftHit == Vector3.zero) return;
        }
        
        
        Vector3 deltaRight = Delta(rightHit, ref rightPreviousPos) * ((float) Math.Cos(MapRendererTransformExtensions.TransformWorldPointToLatLon(target, rightHit).LatitudeInRadians));
        Vector3 deltaLeft = Delta(leftHit, ref leftPreviousPos) * ((float) Math.Cos(MapRendererTransformExtensions.TransformWorldPointToLatLon(target, leftHit).LatitudeInRadians));


        if (rightEnabled && leftEnabled)
        {
            float deltaZoom = 0;

            deltaZoom += (float)(zoomSpeed * (deltaRight.x - deltaLeft.x) * (leftHit.x > rightHit.x ? 1 : -1));
            deltaZoom += (float)(zoomSpeed * (deltaRight.y - deltaLeft.y) * (leftHit.y > rightHit.y ? 1 : -1));
            deltaZoom += (float)(zoomSpeed * (deltaRight.z - deltaLeft.z) * (leftHit.z > rightHit.z ? 1 : -1));

            Vector3 midpoint = (rightHit + leftHit) / 2;
            Vector3 center = MapRendererTransformExtensions.TransformLatLonAltToWorldPoint(target, new LatLonAlt(target.Center.LatitudeInDegrees, target.Center.LongitudeInDegrees, 0.0));
            if (deltaZoom > zoomSpeed / 300 || deltaZoom < -zoomSpeed / 300)
            {
                target.ZoomLevel += deltaZoom;
                // panning to the midpoint of the two controller rays that hit the map
                target.Center = TranslateMap((midpoint - center) * deltaZoom * zoomSpeedF);
            }
            ;
        }
        // Map movement
        Vector3 translate = new Vector3(0.0f, 0.0f, 0.0f);
        if (rightEnabled) { translate += deltaRight; }
        if (leftEnabled) { translate += deltaLeft; }
        target.Center = TranslateMap(translate * (rightEnabled && leftEnabled ? 0.5f : 1.0f));
    }
}
