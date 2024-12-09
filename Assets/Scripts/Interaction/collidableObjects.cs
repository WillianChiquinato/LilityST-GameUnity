using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObjects : MonoBehaviour
{
    [SerializeField] private Collider2D z_Collider;
    [SerializeField] private ContactFilter2D z_filtro;
    private List<Collider2D> z_ColliderObjects = new List<Collider2D>(1);
    public GameObject paredes_pretas;

    protected virtual void Start()
    {
        z_Collider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        z_Collider.Overlap(z_filtro, z_ColliderObjects);
        foreach (var obj in z_ColliderObjects)
        {
            OnCollider(obj.gameObject);
        }
    }

    protected virtual void OnCollider(GameObject ColliderObject)
    {
        
    }
}
