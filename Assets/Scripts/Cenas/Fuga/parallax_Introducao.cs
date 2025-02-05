using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax_Introducao : MonoBehaviour
{
    public GameObject cam;

    [SerializeField] private float parallaxEffectX;
    [SerializeField] private float parallaxEffectY;
    private float xPosition;
    private float yPosition;

    void Start()
    {
        cam = GameObject.Find("Main Camera");

        xPosition = transform.position.x;
        yPosition = transform.position.y;
    }

    void Update()
    {
        float distanceMoveX = cam.transform.position.x * parallaxEffectX;
        float distanceMoveY = cam.transform.position.y * parallaxEffectY;

        transform.position = new Vector3(xPosition + distanceMoveX, yPosition + distanceMoveY);
    }
}
