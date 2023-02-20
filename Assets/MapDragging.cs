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
    public XRRayInteractor rayInteractor;
    public ActionBasedController controller;
    public InputActionReference selectReference;
    public double dragSpeed; 

    private Vector3 previousPos;
    private float correctedDragSpeed;
    private RaycastHit hit;
    private float deltaX;
    private float deltaY;
    private float deltaZ;
    private Vector3 latVector;
    private Vector3 lonVector;
    private float latVectorLength;
    private float lonVectorLength;
    private double currentLat;
    private double currentLon;
    // Start is called before the first frame update
    void Start()
    {
        selectReference.action.started += Pressed;
        selectReference.action.canceled += Released;
        enabled = false;
    }

    private void Pressed(InputAction.CallbackContext context)
    {
        enabled = true;
        previousPos = new Vector3(0.0f, 0.0f, 0.0f);
    }
    
    private void Released(InputAction.CallbackContext context)
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            return;
        }

        if (hit.collider != targetCollider)
        {
            return;
        }

        if (previousPos == new Vector3(0.0f, 0.0f, 0.0f))
        {
            previousPos = hit.point; 
        }

        Debug.Log(previousPos.x);
        deltaX = (previousPos.x - hit.point.x);
        deltaY = (previousPos.y - hit.point.y);
        deltaZ = (previousPos.z - hit.point.z);
        // initially (if not rotated) target.transform.right = Vector3(1.0, 0.0, 0.0)
        latVector = target.transform.right;
        latVectorLength = latVector.x * deltaX + latVector.y * deltaY + latVector.z * deltaZ;

        // initially (if not rotated) target.transform.forward = Vector3(0.0, 0.0, 1.0)
        lonVector = target.transform.forward;
        lonVectorLength = lonVector.x * deltaX + lonVector.y * deltaY + lonVector.z * deltaZ;

        currentLat = target.Center.LatitudeInDegrees;
        currentLon = target.Center.LongitudeInDegrees;

        // Increasing ZoomLevel by 1 means doubling the map zoom. To retain proper movement on
        // different ZoomLevels, divide by 2^ZoomLevel
        // https://learn.microsoft.com/en-us/bingmaps/articles/understanding-scale-and-resolution
        correctedDragSpeed = (float)(dragSpeed / Math.Pow(2.0, target.ZoomLevel));

        // divide by cos(lat) as to adjust for the change in circumference (lon) at different latitutes
        target.Center = new LatLon(currentLat + lonVectorLength * correctedDragSpeed, currentLon + 
            (latVectorLength / Math.Cos((Math.PI / 180) * currentLat)) * correctedDragSpeed);

        previousPos = hit.point;
    }
}
