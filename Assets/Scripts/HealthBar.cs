using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //Referencia do damage
    Damage playerDamage;
    PlayerMoviment player;
    public Slider slider;

    void Start()
    {
        player = FindFirstObjectByType<PlayerMoviment>();
        playerDamage = player.GetComponent<Damage>();
        slider.value = CalcularPorcentagem(playerDamage.Health, playerDamage.maxHealth);
    }

    public void OnEnable()
    {
        playerDamage.healthChange.AddListener(OnPlayerChange);
    }

    public void OnDisable()
    {
        playerDamage.healthChange.RemoveListener(OnPlayerChange);
    }

    public float CalcularPorcentagem(float currentHealth, float maxHealth)
    {
        return currentHealth / maxHealth;
    }

    private void OnPlayerChange(int newHealth, int maxHealth)
    {
        slider.value = CalcularPorcentagem(newHealth, maxHealth);
    }
}
