using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Sistema_Pause sistema_Pause;

    public GameObject CoracoesLility;
    public GameObject CoracoesLility2;
    public GameObject CoracoesLility3;
    public GameObject CoracoesLility4;
    public Damage playerDamage;

    // Start is called before the first frame update
    void Start()
    {
        sistema_Pause = GameObject.FindObjectOfType<Sistema_Pause>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerDamage = player.GetComponent<Damage>();

        CoracoesLility = GameObject.FindGameObjectWithTag("Vida");
        CoracoesLility2 = GameObject.FindGameObjectWithTag("Vida2");
        CoracoesLility3 = GameObject.FindGameObjectWithTag("Vida3");
        CoracoesLility4 = GameObject.FindGameObjectWithTag("Vida4");

    }

    // Update is called once per frame
    void Update()
    {
        if (playerDamage.Health == 3)
        {
            CoracoesLility.gameObject.SetActive(false);
        }
        if (playerDamage.Health == 2)
        {
            CoracoesLility2.gameObject.SetActive(false);
        }
        if (playerDamage.Health == 1)
        {
            CoracoesLility3.gameObject.SetActive(false);
        }
        if (playerDamage.Health == 0)
        {
            CoracoesLility4.gameObject.SetActive(false);
        }
    }
}
