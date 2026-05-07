using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class SeletorArmas : MonoBehaviour
{
    public static SeletorArmas instance;

    [Header("Buttons")]
    [SerializeField] public Button previousButton;
    [SerializeField] public Button nextButton;

    public int currentIndex = 0;

    [Header("Conteudo UI")]
    public Image imagemUI;
    public TextMeshProUGUI tituloUI;

    public string[] nomesDasArmas = { "Bastão", "Arco", "Marreta", "Luva", "Mascara", "Sino" };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        switch (currentIndex)
        {
            case 0:
                imagemUI.sprite = Resources.Load<Sprite>("Sprites/Armas/BastaoIcon");
                tituloUI.text = PowerUpCategory.ArmaPrimaria.GetStringCategory();
                break;
            case 1:
                imagemUI.sprite = Resources.Load<Sprite>("Sprites/Armas/ArcoIcon");
                tituloUI.text = PowerUpCategory.ArmaSecundaria.GetStringCategory();
                break;
            case 2:
                imagemUI.sprite = Resources.Load<Sprite>("Sprites/Armas/Marreta");
                tituloUI.text = PowerUpCategory.ArmaPrimaria.GetStringCategory();
                break;
            case 3:
                imagemUI.sprite = Resources.Load<Sprite>("Sprites/Armas/Mascara");
                tituloUI.text = PowerUpCategory.ArmaSecundaria.GetStringCategory();
                break;
            case 4:
                imagemUI.sprite = Resources.Load<Sprite>("Sprites/Armas/Sino");
                tituloUI.text = PowerUpCategory.ArmaSecundaria.GetStringCategory();
                break;
        }
    }

    public void SelectArma(int index)
    {
        if (index < 0 || index >= nomesDasArmas.Length)
            return;

        previousButton.interactable = (index != 0);
        nextButton.interactable = (index != nomesDasArmas.Length - 1);

        currentIndex = index;

        string armaAtual = nomesDasArmas[currentIndex];

        // Sincroniza a arma selecionada no ArmasSystem
        ArmasSystem.instance.armaSelecionada = armaAtual;
        
        // Garante que o deck runtime existe
        if (!ArmasSystem.instance.decksPorArmaRuntime.ContainsKey(armaAtual))
        {
            Debug.LogWarning($"Deck runtime não existe para arma '{armaAtual}', criando...");
            ArmasSystem.instance.decksPorArmaRuntime[armaAtual] = new List<FragmentoData>();
        }
        
        ArmasSystem.instance.AtualizarDeckUI(armaAtual);
        
        // Chama o método de seleção no FragmentoSystem
        FragmentoSystem.instance.SelecionarArma(armaAtual);
    }

    public void NextArma()
    {
        SelectArma(currentIndex + 1);
    }

    public void PreviousArma()
    {
        SelectArma(currentIndex - 1);
    }
}