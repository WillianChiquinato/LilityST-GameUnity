using TMPro;
using UnityEngine;

public class BastaoPowerUp : PowerUp
{
    public SaveData saveData;
    public PlayerMoviment playerMoviment;

    public TextMeshPro textoAcima;
    public string textoIndicator;

    protected override void Awake()
    {
        base.Awake();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        startPosition = transform.position;

        saveData = SaveData.Instance;
        Debug.Log("Lista de PowerUps: " + string.Join(", ", SaveData.Instance.powerUps));
    }

    protected override void Update()
    {
        base.Update();
        if (playerMoviment.animacao.GetCurrentAnimatorStateInfo(0).IsName("IsPowerUp") && playerMoviment.animacao.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9)
        {
            playerMoviment.animacao.SetBool("isPowerUp", false);
            Destroy(this.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            textoAcima.text = textoIndicator;
            if (playerMoviment.entrar)
            {
                playerMoviment.animacao.SetBool("isPowerUp", true);
                if (!SaveData.Instance.powerUps.Contains(PowerUps.Bastao))
                {
                    SaveData.Instance.powerUps.Add(PowerUps.Bastao);
                    SaveData.Instance.attackUnlocked = true;
                    saveData.attackUnlocked = true;
                    Debug.Log("PowerUp adicionado com sucesso!");
                }
                else
                {
                    Debug.Log("PowerUp já estava na lista!");
                }

                Debug.Log("Lista de PowerUps: " + string.Join(", ", SaveData.Instance.powerUps));

            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            textoAcima.text = "";
        }
    }
}
