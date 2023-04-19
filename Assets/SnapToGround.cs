using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    public Transform xrOriginTransform;

    public void RaycastAndMovePlayer()
    {
        Ray ray = new Ray(xrOriginTransform.position, xrOriginTransform.up * -1);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            xrOriginTransform.position = hit.point;
        }
    }
}
