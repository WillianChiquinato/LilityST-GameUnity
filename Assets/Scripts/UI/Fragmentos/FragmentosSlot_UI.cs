using System;
using System.Collections.Generic;
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

        // Verificar se este slot está no DeckBuilder
        bool estaNoDeckBuilder = transform.IsChildOf(FragmentoSystem.instance.DeckBuilderSlotParent);

        Debug.Log($"Clique detectado - Fragmento: {Fragmento.FragmentoData.NomeFragmento}, EstáNoDeckBuilder: {estaNoDeckBuilder}");

        if (estaNoDeckBuilder)
        {
            // Remover do deck com animação
            string armaAtual = ArmasSystem.instance.armaSelecionada;
            Debug.Log($"Removendo fragmento '{Fragmento.FragmentoData.NomeFragmento}' do deck da arma '{armaAtual}'");

            // Determinar slot de destino no inventário baseado no tipo
            Transform slotDestinoInventario = GetSlotDestinoInventario(Fragmento.FragmentoData.TipoFragmento);
            Debug.Log($"Slot destino inventário encontrado: {slotDestinoInventario != null}");

            if (slotDestinoInventario != null)
            {
                Debug.Log($"Iniciando animação de volta para inventário");
                // Animar de volta para o inventário
                MoveFragmentoToInventario(slotDestinoInventario, () =>
                {
                    Debug.Log($"Animação completa, removendo fragmento do deck");
                    // Após a animação, remove do deck e adiciona ao inventário
                    bool removido = ArmasSystem.instance.RemoverDoDeck(armaAtual, Fragmento.FragmentoData);

                    if (removido)
                    {
                        FragmentoSystem.instance.AddItem(Fragmento.FragmentoData);
                        Debug.Log($"Fragmento '{Fragmento.FragmentoData.NomeFragmento}' removido do deck e retornado ao inventário.");

                        // Atualizar UI do inventário e depois salvar
                        UpdateInventory(Fragmento);
                    }
                });
            }
            else
            {
                Debug.LogWarning($"Slot destino não encontrado, fazendo remoção sem animação");
                // Fallback: remoção sem animação
                bool removido = ArmasSystem.instance.RemoverDoDeck(armaAtual, Fragmento.FragmentoData);

                if (removido)
                {
                    FragmentoSystem.instance.AddItem(Fragmento.FragmentoData);
                    Debug.Log($"Fragmento '{Fragmento.FragmentoData.NomeFragmento}' removido do deck e retornado ao inventário.");

                    // Atualizar UI do inventário e depois salvar
                    UpdateInventory(Fragmento);
                }
            }
            return;
        }

        // Código original para adicionar ao deck
        string armaAtual2 = SeletorArmas.instance.nomesDasArmas[SeletorArmas.instance.currentIndex];
        Debug.Log($"Tentando adicionar fragmento '{Fragmento.FragmentoData.NomeFragmento}' ao deck da arma '{armaAtual2}'");

        // Verificar se o deck existe no runtime
        if (!ArmasSystem.instance.decksPorArmaRuntime.ContainsKey(armaAtual2))
        {
            Debug.LogError($"Deck para arma '{armaAtual2}' não encontrado!");
            return;
        }

        var deckAtual = ArmasSystem.instance.decksPorArmaRuntime[armaAtual2];
        Debug.Log($"Deck atual da arma '{armaAtual2}' tem {deckAtual.Count} fragmentos");

        if (deckAtual.Count >= ArmasSystem.instance.maxDeckSize)
        {
            Debug.Log("Deck está cheio! Não é possível adicionar mais cartas.");
            return;
        }

        if (!ArmasSystem.instance.PodeAdicionarFragmento(deckAtual, Fragmento.FragmentoData))
        {
            Debug.Log("Você atingiu o limite para esse tipo de fragmento!");
            return;
        }

        if (Fragmento != null && Fragmento.FragmentoData != null)
        {
            int slotDestinoIndex = ArmasSystem.instance.GetPrimeiroSlotVazioOuFragmentoExistente(deckAtual, Fragmento.FragmentoData);
            Debug.Log($"Slot destino calculado: {slotDestinoIndex}");

            if (slotDestinoIndex == -1)
            {
                Debug.Log("Deck cheio! Não há slot disponível para este fragmento.");
                return;
            }

            if (slotDestinoIndex >= FragmentoSystem.instance.DeckBuilderSlotParent.childCount)
            {
                Debug.LogError($"Índice do slot destino ({slotDestinoIndex}) é maior que o número de slots disponíveis ({FragmentoSystem.instance.DeckBuilderSlotParent.childCount})");
                return;
            }

            Transform destinoSlot = FragmentoSystem.instance.DeckBuilderSlotParent.GetChild(slotDestinoIndex).transform;
            Debug.Log($"Iniciando animação para slot destino: {slotDestinoIndex}");

            // Move a carta para o DeckBuilder antes de adicionar
            MoveFragmentoToDeckBuilder(destinoSlot, () =>
            {
                Debug.Log($"Animação completa, adicionando fragmento ao deck");
                AdicionarFragmentoAoDeck(armaAtual2);
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
        Vector2 offsetDestino = new Vector2(0f, 0f);

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

        StartCoroutine(SalvarAposDelay());
        UpdateInventory(Fragmento);
    }


    private void AdicionarFragmentoAoDeck(string armaAtual)
    {
        Debug.Log($"AdicionarFragmentoAoDeck chamado para arma '{armaAtual}' com fragmento '{Fragmento.FragmentoData.NomeFragmento}'");

        if (Fragmento.FragmentoData.TipoFragmento == fragmentoType.Tempo || Fragmento.FragmentoData.TipoFragmento == fragmentoType.Movimento || Fragmento.FragmentoData.TipoFragmento == fragmentoType.Vida || Fragmento.FragmentoData.TipoFragmento == fragmentoType.Caos || Fragmento.FragmentoData.TipoFragmento == fragmentoType.Ordem)
        {
            Debug.Log($"Tipo de fragmento válido: {Fragmento.FragmentoData.TipoFragmento}");
            bool adicionou = ArmasSystem.instance.AdicionarAoDeck(armaAtual, Fragmento.FragmentoData);

            if (adicionou)
            {
                Debug.Log($"Fragmento adicionado com sucesso, removendo do inventário");
                FragmentoSystem.instance.RemoveItem(Fragmento.FragmentoData);
            }
            else
            {
                Debug.Log("Não é possível adicionar mais.");
            }
        }
        else
        {
            Debug.Log($"Tipo de carta inválido: {Fragmento.FragmentoData.TipoFragmento}!");
        }
    }

    private Transform GetSlotDestinoInventario(fragmentoType tipoFragmento)
    {
        Transform parentSlot = null;
        List<FragmentoItem> listaFragmentos = null;

        switch (tipoFragmento)
        {
            case fragmentoType.Tempo:
                parentSlot = FragmentoSystem.instance.TempoSlotParent;
                listaFragmentos = FragmentoSystem.instance.ChaveTempo;
                break;
            case fragmentoType.Movimento:
                parentSlot = FragmentoSystem.instance.MovimentoSlotParent;
                listaFragmentos = FragmentoSystem.instance.ChaveMovimento;
                break;
            case fragmentoType.Vida:
                parentSlot = FragmentoSystem.instance.VidaSlotParent;
                listaFragmentos = FragmentoSystem.instance.ChaveVida;
                break;
            case fragmentoType.Caos:
                parentSlot = FragmentoSystem.instance.CaosSlotParent;
                listaFragmentos = FragmentoSystem.instance.ChaveCaos;
                break;
            case fragmentoType.Ordem:
                parentSlot = FragmentoSystem.instance.OrdemSlotParent;
                listaFragmentos = FragmentoSystem.instance.ChaveOrdem;
                break;
        }

        if (parentSlot != null && parentSlot.childCount > 0)
        {
            // Primeiro, verificar se o fragmento já existe no inventário
            for (int i = 0; i < listaFragmentos.Count; i++)
            {
                if (listaFragmentos[i].FragmentoData == Fragmento.FragmentoData)
                {
                    // Encontrou o fragmento, retornar o slot correspondente
                    if (i < parentSlot.childCount)
                    {
                        Debug.Log($"Fragmento já existe no inventário no slot {i}");
                        return parentSlot.GetChild(i);
                    }
                }
            }

            // Se não encontrou o fragmento existente, ele será adicionado na próxima posição disponível
            int proximaPosicao = listaFragmentos.Count;
            if (proximaPosicao < parentSlot.childCount)
            {
                Debug.Log($"Fragmento será adicionado na posição {proximaPosicao}");
                return parentSlot.GetChild(proximaPosicao);
            }

            // Fallback: retorna o primeiro slot do tipo
            Debug.Log($"Usando fallback: primeiro slot do tipo");
            return parentSlot.GetChild(0);
        }

        Debug.LogWarning($"Não foi possível encontrar slot de destino para tipo {tipoFragmento}");
        return null;
    }

    private void MoveFragmentoToInventario(Transform destino, Action onMoveComplete)
    {
        Debug.Log($"MoveFragmentoToInventario iniciado para destino: {destino.name}");

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        // Criar placeholder para manter o layout do deck
        placeholderSlot = new GameObject("Placeholder");
        RectTransform placeholderRect = placeholderSlot.AddComponent<RectTransform>();
        RectTransform thisRect = GetComponent<RectTransform>();
        RectTransform destinoRect = destino as RectTransform;

        placeholderRect.sizeDelta = thisRect.sizeDelta;
        placeholderSlot.transform.SetParent(originalParent);
        placeholderSlot.transform.SetSiblingIndex(originalSiblingIndex);

        // Desabilitar layout temporariamente do deck
        HorizontalLayoutGroup horizontalLayoutDeck = originalParent.GetComponent<HorizontalLayoutGroup>();
        if (horizontalLayoutDeck != null) horizontalLayoutDeck.enabled = false;

        // Calcular posições na tela
        Vector2 telaOrigem = RectTransformUtility.WorldToScreenPoint(null, thisRect.position);
        Vector2 telaDestino = RectTransformUtility.WorldToScreenPoint(null, destinoRect.position);

        Debug.Log($"Posição origem: {telaOrigem}, Posição destino: {telaDestino}");

        // Mover para canvas da frente
        transform.SetParent(canvasFragmentoFrente, false);

        // Converter posições para coordenadas locais do canvas
        Vector2 origemLocal, destinoLocal;
        RectTransform canvasRect = canvasFragmentoFrente as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, telaOrigem, null, out origemLocal);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, telaDestino, null, out destinoLocal);

        // Offsets para animação suave de volta ao inventário
        Vector2 offsetOrigem = new Vector2(50f, -50f);
        Vector2 offsetDestino = new Vector2(50f, 0f);

        thisRect.anchoredPosition = origemLocal + offsetOrigem;

        Debug.Log($"Iniciando animação de {origemLocal + offsetOrigem} para {destinoLocal}");

        // Animar para a posição de destino
        thisRect.DOAnchorPos(destinoLocal, animDuration)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                Debug.Log($"Animação para inventário completa, movendo para slot correto: {destino.name}");

                // Mover para o slot de destino correto no inventário
                transform.SetParent(destino, false);
                transform.SetSiblingIndex(0); // Primeiro filho do slot

                // Resetar posição para ficar alinhado no slot
                thisRect.anchoredPosition = Vector2.zero;
                thisRect.anchorMin = Vector2.zero;
                thisRect.anchorMax = Vector2.one;
                thisRect.offsetMin = Vector2.zero;
                thisRect.offsetMax = Vector2.zero;

                // Reabilitar layout do deck
                if (horizontalLayoutDeck != null) horizontalLayoutDeck.enabled = true;

                // Limpar placeholder
                if (placeholderSlot != null)
                    Destroy(placeholderSlot);

                // Executar callback
                onMoveComplete?.Invoke();
            });


        StartCoroutine(SalvarAposDelay());
        UpdateInventory(Fragmento);
    }

    private System.Collections.IEnumerator SalvarAposDelay()
    {
        yield return new WaitForSeconds(0.1f);
        FragmentoSystem.instance.SaveFragment();
        Debug.Log($"Salvamento realizado após delay");
    }
}