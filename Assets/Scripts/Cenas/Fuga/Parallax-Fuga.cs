using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxFuga : MonoBehaviour
{
    public Transform cam;
    public float parallaxEffectX = 0.5f;
    public float parallaxEffectY = 0.5f;

    private Vector3 lastCamPosition;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main.transform;
        }

        lastCamPosition = cam.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cam.position - lastCamPosition;

        transform.position += new Vector3(
            deltaMovement.x * parallaxEffectX,
            deltaMovement.y * parallaxEffectY,
            0
        );

        lastCamPosition = cam.position;
    }
}
