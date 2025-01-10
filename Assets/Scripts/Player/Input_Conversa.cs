using UnityEngine;

public class Input_Conversa : MonoBehaviour
{
    [Header("Animação Sutil")]
    public float floatSpeed;
    public float floatAmplitude;
    private Vector3 startPosition;
    public GameObject NPCposition;
    public Vector3 offset;
    public Animator animator;

    void Awake()
    {
        this.gameObject.SetActive(false);

        animator = GetComponent<Animator>();
        startPosition = NPCposition.transform.position + offset;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
