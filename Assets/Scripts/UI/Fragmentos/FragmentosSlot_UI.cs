using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FragmentosSlot_UI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image FragmentoImagem;
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private TextMeshProUGUI FragmentoTexto;
    private float animDuration = 0.4f;

    [SerializeField] private Transform canvasFragmentoFrente;
    private Transform originalParent;
    private int originalSiblingIndex;
    private GameObject placeholderSlot;

    public FragmentoItem Fragmento;

    void Start()
    {
        placeholderSlot = Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(obj => obj.name == "Placeholder");

        canvasFragmentoFrente = GameObject.Find("CanvasFrente").transform;
    }

    public void UpdateInventory(FragmentoItem _newItem)
    {
        Fragmento = _newItem;

        if (Fragmento != null)
        {
            FragmentoImagem.sprite = Fragmento.FragmentoData.Icon;

            if (Fragmento.stackSize >= 1)
            {
                FragmentoTexto.text = Fragmento.stackSize.ToString();
            }
            else
            {
                FragmentoTexto.text = "";
            }
        }
    }

    public void CleanUpSlot()
    {
        Fragmento = null;
        FragmentoImagem.sprite = originalSprite;
        FragmentoTexto.text = "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Fragmento == null || Fragmento.FragmentoData == null)
        {
            Debug.LogWarning("Fragmento ou FragmentoData está nulo!");
            return;
        }

        if (ArmasSystem.instance.deck.Count >= ArmasSystem.instance.maxDeckSize)
        {
            Debug.Log("Deck está cheio! Não é possível adicionar mais cartas.");
            return;
        }

        if (!ArmasSystem.instance.PodeAdicionarFragmento(Fragmento.FragmentoData))
        {
            Debug.Log("Você atingiu o limite para esse tipo de fragmento!");
            return;
        }

        if (Fragmento != null && Fragmento.FragmentoData != null)
        {
            //Ele pega o index do slot de destino baseado na quantidade de cartas no deck.
            int slotDestinoIndex = ArmasSystem.instance.deck.Count;
            Transform destinoSlot = FragmentoSystem.instance.DeckBuilderSlotParent.GetChild(slotDestinoIndex).transform;

            // Move a carta para o DeckBuilder antes de adicionar
            MoveFragmentoToDeckBuilder(destinoSlot, () =>
            {
                AdicionarFragmentoAoDeck();
            });
        }
        else
        {
            Debug.Log("Item ou itemData é null!");
        }
    }

    private void MoveFragmentoToDeckBuilder(Transform destino, Action onMoveComplete)
    {
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        placeholderSlot = new GameObject("Placeholder");
        RectTransform placeholderRect = placeholderSlot.AddComponent<RectTransform>();
        RectTransform thisRect = GetComponent<RectTransform>();
        RectTransform destinoRect = destino as RectTransform;

        placeholderRect.sizeDelta = thisRect.sizeDelta;
        placeholderSlot.transform.SetParent(originalParent);
        placeholderSlot.transform.SetSiblingIndex(originalSiblingIndex);

        HorizontalLayoutGroup horizontalLayout = originalParent.GetComponent<HorizontalLayoutGroup>();
        if (horizontalLayout != null) horizontalLayout.enabled = false;

        Vector2 telaOrigem = RectTransformUtility.WorldToScreenPoint(null, thisRect.position);
        Vector2 telaDestino = RectTransformUtility.WorldToScreenPoint(null, destinoRect.position);

        transform.SetParent(canvasFragmentoFrente, false);

        Vector2 origemLocal, destinoLocal;
        RectTransform canvasRect = canvasFragmentoFrente as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, telaOrigem, null, out origemLocal);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, telaDestino, null, out destinoLocal);

        // 🔸 AQUI: define os offsets de partida e chegada
        Vector2 offsetOrigem = new Vector2(50f, -50f);
        Vector2 offsetDestino = new Vector2(50f, 0f);

        thisRect.anchoredPosition = origemLocal + offsetOrigem;

        thisRect.DOAnchorPos(destinoLocal + offsetDestino, animDuration)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                transform.SetParent(originalParent, false);
                transform.SetSiblingIndex(originalSiblingIndex);
                thisRect.anchoredPosition = Vector2.zero;

                if (horizontalLayout != null) horizontalLayout.enabled = true;

                Destroy(placeholderSlot);
                onMoveComplete?.Invoke();
            });
    }


    private void AdicionarFragmentoAoDeck()
    {
        if (Fragmento.FragmentoData.TipoFragmento == fragmentoType.Tempo || Fragmento.FragmentoData.TipoFragmento == fragmentoType.Movimento || Fragmento.FragmentoData.TipoFragmento == fragmentoType.Vida || Fragmento.FragmentoData.TipoFragmento == fragmentoType.Caos || Fragmento.FragmentoData.TipoFragmento == fragmentoType.Ordem)
        {
            bool adicionou = ArmasSystem.instance.AdicionarAoDeck(Fragmento.FragmentoData);

            if (adicionou)
            {
                FragmentoSystem.instance.RemoveItem(Fragmento.FragmentoData);
            }
            else
            {
                Debug.Log("Deck está cheio! Não é possível adicionar mais cartas.");
            }
        }
        else
        {
            Debug.Log("Tipo de carta inválido!");
        }
    }
}
