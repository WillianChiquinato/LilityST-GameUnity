using System.Collections.Generic;
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
    public Image conteudoUI;

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
                conteudoUI.sprite = Resources.Load<Sprite>("Sprites/Armas/Bastao");
                break;
            case 1:
                conteudoUI.sprite = Resources.Load<Sprite>("Sprites/Armas/Arco");
                break;
            case 2:
                conteudoUI.sprite = Resources.Load<Sprite>("Sprites/Armas/Marreta");
                break;
            case 3:
                conteudoUI.sprite = Resources.Load<Sprite>("Sprites/Armas/Mascara");
                break;
            case 4:
                conteudoUI.sprite = Resources.Load<Sprite>("Sprites/Armas/Sino");
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

        Debug.Log($"SeletorArmas.SelectArma - Selecionando arma: {armaAtual} (índice: {index})");

        // Sincroniza a arma selecionada no ArmasSystem
        ArmasSystem.instance.armaSelecionada = armaAtual;
        
        // Garante que o deck runtime existe
        if (!ArmasSystem.instance.decksPorArmaRuntime.ContainsKey(armaAtual))
        {
            Debug.LogWarning($"Deck runtime não existe para arma '{armaAtual}', criando...");
            ArmasSystem.instance.decksPorArmaRuntime[armaAtual] = new List<FragmentoData>();
        }
        
        // Atualiza a UI do deck para a arma selecionada
        ArmasSystem.instance.AtualizarDeckUI(armaAtual);
        
        // Chama o método de seleção no FragmentoSystem
        FragmentoSystem.instance.SelecionarArma(armaAtual);
        
        Debug.Log($"Arma '{armaAtual}' selecionada com sucesso");
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