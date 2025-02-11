using Unity.VisualScripting;
using UnityEngine;

public class grabPlayer : MonoBehaviour
{
    [Header("Grab Instances")]
    [SerializeField] private Transform grabPoint;
    public PlayerMoviment grabandoObject;


    [Header("Grab Settings")]
    public bool grabActived = false;
    public bool continuosGrab = false;

    private void Start()
    {
        grabandoObject = GameObject.FindFirstObjectByType<PlayerMoviment>();
    }

    void Update()
    {
        if (grabActived)
        {
            if (continuosGrab)
            {
                grabandoObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                grabandoObject.transform.SetParent(transform);
                grabandoObject.transform.position = grabPoint.position;
            }
            else
            {
                grabandoObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                grabandoObject.transform.SetParent(null);
                grabandoObject = null;
            }
        }
    }
}
