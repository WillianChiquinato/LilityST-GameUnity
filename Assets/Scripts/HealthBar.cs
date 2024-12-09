using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //Referencia do damage
    Damage playerDamage;
    public PlayerMoviment player;
    public Slider slider;

    void Awake()
    {
        player = GameObject.FindFirstObjectByType<PlayerMoviment>();

        if (player == null)
        {
            Debug.Log("Nao achei o player");
        }
        playerDamage = player.GetComponent<Damage>();
    }

    void Start()
    {
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
