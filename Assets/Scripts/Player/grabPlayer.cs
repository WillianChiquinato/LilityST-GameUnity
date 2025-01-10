using Unity.VisualScripting;
using UnityEngine;

public class grabPlayer : MonoBehaviour
{
    [SerializeField] private Transform grabPoint;

    [SerializeField] private Transform rayPoint;
    [SerializeField] private float rayDistance;

    public GameObject grabandoObject;
    [SerializeField] private LayerMask playerLayerMask;

    public bool grabActived = false;

    void Update()
    {
        RaycastHit2D Hitgrab = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance, playerLayerMask);

        if (grabandoObject == null && Hitgrab.collider != null && grabActived)
        {
            grabandoObject = Hitgrab.collider.gameObject;
            grabandoObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            grabandoObject.transform.position = grabPoint.position;
            grabandoObject.transform.SetParent(transform);
        }
        else if (grabandoObject != null && grabActived == false)
        {
            Debug.Log("Testando o cuzinho");
            grabandoObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            grabandoObject.transform.SetParent(null);
            grabandoObject = null;
        }

        Debug.DrawRay(rayPoint.position, transform.right * rayDistance, Color.green);
    }
}
