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
    public MapRenderer target;
    public Collider targetCollider;
    public ActionBasedController rightController;
    public ActionBasedController leftController;

    private float zoomSpeed = 3;
    private XRRayInteractor rightRayInteractor; 
    
    private XRRayInteractor leftRayInteractor;
    
    private double dragSpeed = 500; 
    private Vector3 rightPreviousPos;
    private Vector3 leftPreviousPos;

    private bool rightEnabled = false;
    private bool leftEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
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
    private LatLon DragDelta(XRRayInteractor rayInteractor, ref Vector3 previousPos)
    {
        RaycastHit hit;
        if (!rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            return new LatLon(0.0, 0.0);
        }

        if (hit.collider != targetCollider)
        {
            return new LatLon(0.0, 0.0);
        }

        if (previousPos == new Vector3(0.0f, 0.0f, 0.0f))
        {
            previousPos = hit.point;
        }

        Vector3 delta = previousPos - hit.point;
        previousPos = hit.point;


        // initially (if not rotated) target.transform.right = Vector3(1.0, 0.0, 0.0), correspongding to the positive direction of latitude movement
        Vector3 latVector = target.transform.right;
        float latVectorLength = latVector.x * delta.x + latVector.y * delta.y + latVector.z * delta.z;

        // initially (if not rotated) target.transform.forward = Vector3(0.0, 0.0, 1.0), correspongding to the positive direction of longitude movement
        Vector3 lonVector = target.transform.forward;
        float lonVectorLength = lonVector.x * delta.x + lonVector.y * delta.y + lonVector.z * delta.z;

        return new LatLon(latVectorLength, lonVectorLength);
    }

    private LatLon TranslateMap(LatLon delta)
    {
        double currentLat = target.Center.LatitudeInDegrees;
        double currentLon = target.Center.LongitudeInDegrees;

        // Increasing ZoomLevel by 1 means doubling the map zoom. To retain proper movement on
        // different ZoomLevels, divide by 2^ZoomLevel
        // https://learn.microsoft.com/en-us/bingmaps/articles/understanding-scale-and-resolution
        float correctedDragSpeed = (float)(dragSpeed / Math.Pow(2.0, target.ZoomLevel));

        // divide by cos(lat) as to adjust for the change in circumference (lon) at different latitutes
        return new LatLon(currentLat + delta.LongitudeInDegrees * correctedDragSpeed, currentLon +
            (delta.LatitudeInDegrees / Math.Cos((Math.PI / 180) * currentLat)) * correctedDragSpeed);
    }

  
    // Update is called once per frame
    void Update()
    {
        LatLon deltaRight = DragDelta(rightRayInteractor, ref rightPreviousPos);
        LatLon deltaLeft = DragDelta(leftRayInteractor, ref leftPreviousPos);

        // Highly scuffed implementation atm
        Vector3 prevRight = new Vector3(0.1f, 0.1f, 0.1f);
        Vector3 prevLeft = new Vector3(0.1f, 0.1f, 0.1f);
        LatLon rightHit = DragDelta(rightRayInteractor, ref prevRight);
        LatLon leftHit = DragDelta(leftRayInteractor, ref prevLeft);

        // Zoom (fix, don't use so many ifs) 
        if (rightEnabled && leftEnabled)
        {
            float deltaZoom = 0;
            if (leftHit.LatitudeInDegrees > rightHit.LatitudeInDegrees)
            {
                deltaZoom += (float)(((deltaLeft.LatitudeInDegrees - deltaRight.LatitudeInDegrees)) * zoomSpeed);
            }
            else
            {
                deltaZoom += (float)(((deltaRight.LatitudeInDegrees - deltaLeft.LatitudeInDegrees)) * zoomSpeed);
            }
            if (leftHit.LongitudeInDegrees > rightHit.LongitudeInDegrees)
            {
                deltaZoom += (float)(((deltaLeft.LongitudeInDegrees - deltaRight.LongitudeInDegrees)) * zoomSpeed);
            }
            else
            {
                deltaZoom += (float)(((deltaRight.LongitudeInDegrees - deltaLeft.LongitudeInDegrees)) * zoomSpeed);
            }

            target.ZoomLevel += deltaZoom;
            return;
        }
        // Map movement
        if (rightEnabled) { target.Center = TranslateMap(deltaRight); return; }
        if (leftEnabled) { target.Center = TranslateMap(deltaLeft); return; }
    }
}
