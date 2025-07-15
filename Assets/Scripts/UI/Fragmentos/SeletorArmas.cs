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

    public string[] nomesDasArmas = { "Bastão", "Arco", "Marreta", "Mascara", "Sino" };

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

        ArmasSystem.instance.AtualizarDeckUI(armaAtual);
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