using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Animação Sutil")]
    public float floatSpeed;
    public float floatAmplitude;
    public Vector3 startPosition;
    public Animator animator;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        //Animação sutil de item.
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
