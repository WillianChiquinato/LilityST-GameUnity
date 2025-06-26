using UnityEngine;

public class IconIdle : MonoBehaviour
{
    [Header("Animação Sutil")]
    public float floatSpeed;
    public float floatAmplitude;
    public Vector3 startPosition;
    public Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
