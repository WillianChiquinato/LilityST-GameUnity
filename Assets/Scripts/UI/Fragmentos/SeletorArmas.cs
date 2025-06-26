using UnityEngine;
using UnityEngine.UI;

public class SeletorArmas : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] public Button previousButton;
    [SerializeField] public Button nextButton;

    public int currentIndex = 0;

    [Header("Conteudo UI")]
    public Image conteudoUI;

    void Awake()
    {
        SelectArma(0);
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
        previousButton.interactable = (index != 0);
        nextButton.interactable = (index != 4);

        currentIndex = index;
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
