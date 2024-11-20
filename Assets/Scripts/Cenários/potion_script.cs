using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class potion_script : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public int maxPotionsInt = 3;
    public int potionInt;

    public Image Healing01;
    public Image Healing02;
    public Image Healing03;

    public Image DesativadoHealing;

    void Awake()
    {
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();

        potionInt = maxPotionsInt;
    }


    void Update()
    {
        if (potionInt == 2f)
        {
            Healing03 = DesativadoHealing;
        }
        else if (potionInt == 1f)
        {
            Healing03 = DesativadoHealing;
            Healing02 = DesativadoHealing;
        }
        else if (potionInt == 0f)
        {
            Healing03 = DesativadoHealing;
            Healing02 = DesativadoHealing;
            Healing01 = DesativadoHealing;
        }
    }

    public void HealigMetod()
    {
        potionInt--;
    }
}
