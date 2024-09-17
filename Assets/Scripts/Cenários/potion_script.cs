using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class potion_script : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public TextMeshProUGUI potions;
    public int maxPotionsInt = 3;
    public int potionInt;

    void Awake()
    {
        potions.GetComponent<TextMeshProUGUI>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();

        potionInt = maxPotionsInt;
        potions.text = potionInt.ToString();
    }


    void Update()
    {
        potions.text = potionInt.ToString();
    }

    public void HealigMetod()
    {
        potionInt--;
    }
}
