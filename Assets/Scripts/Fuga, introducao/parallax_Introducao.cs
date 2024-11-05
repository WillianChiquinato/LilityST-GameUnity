using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax_Introducao : MonoBehaviour
{
    public GameObject cam;

    [SerializeField] private float parallaxEffect;
    private float xPosition;

    void Start()
    {
        cam = GameObject.Find("Main Camera");

        xPosition = transform.position.x;
    }

    void Update()
    {
        float distanceMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceMove, transform.position.y);
    }
}
