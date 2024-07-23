using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles_Entrelacados : MonoBehaviour
{
    public float Tempo_Spawn;
    public float Tempo_Animacao;
    public bool Tempo_Spawn_Bool = false;

    public GameObject Tiles;
    BoxCollider2D boxCollider2D;

    void Start()
    {
        Tiles.gameObject.SetActive(false);
        boxCollider2D = Tiles.GetComponent<BoxCollider2D>();

        boxCollider2D.enabled = false;
    }

    void Update()
    {
        if (Tempo_Spawn_Bool == true)
        {
            Tempo_Spawn += Time.deltaTime;
            Tiles.gameObject.SetActive(true);
        }

        if (Tempo_Spawn >= Tempo_Animacao)
        {
            boxCollider2D.enabled = true;
            Tempo_Spawn = 0f;
            Tempo_Spawn_Bool = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Parry"))
        {
            Tempo_Spawn_Bool = true;
        }
    }
}
