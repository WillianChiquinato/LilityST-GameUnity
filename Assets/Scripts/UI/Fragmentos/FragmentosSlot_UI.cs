using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FragmentosSlot_UI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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

            // Salvar referência do fragmento antes da animação
            FragmentoData fragmentoParaRemover = Fragmento.FragmentoData;

            // Determinar slot de destino no inventário baseado no tipo
            Transform slotDestinoInventario = GetSlotDestinoInventario(fragmentoParaRemover.TipoFragmento);
            Debug.Log($"Slot destino inventário encontrado: {slotDestinoInventario != null}");

            if (slotDestinoInventario != null)
            {
                Debug.Log($"Iniciando animação de volta para inventário");
                // Animar de volta para o inventário
                MoveFragmentoToInventario(slotDestinoInventario, () =>
                {
                    Debug.Log($"Animação completa, removendo fragmento do deck");

                    // Após a animação, remove do deck
                    bool removido = ArmasSystem.instance.RemoverDoDeck(armaAtual, fragmentoParaRemover);

                    if (removido)
                    {
                        // Adicionar de volta ao inventário
                        FragmentoSystem.instance.AddItem(fragmentoParaRemover);
                        Debug.Log($"Fragmento '{fragmentoParaRemover.NomeFragmento}' removido do deck e retornado ao inventário.");

                        // Forçar atualização completa da UI
                        FragmentoSystem.instance.UpdateInventory();
                        ArmasSystem.instance.AtualizarDeckUI(armaAtual);

                        // Salvar após adicionar ao inventário
                        FragmentoSystem.instance.SaveFragment();
                        Debug.Log($"Dados salvos após adicionar fragmento ao inventário");
                    }
                });
            }
            else
            {
                Debug.LogWarning($"Slot destino não encontrado, fazendo remoção sem animação");
                // Fallback: remoção sem animação
                bool removido = ArmasSystem.instance.RemoverDoDeck(armaAtual, fragmentoParaRemover);

                if (removido)
                {
                    FragmentoSystem.instance.AddItem(fragmentoParaRemover);
                    Debug.Log($"Fragmento '{fragmentoParaRemover.NomeFragmento}' removido do deck e retornado ao inventário.");

                    // Forçar atualização completa da UI
                    FragmentoSystem.instance.UpdateInventory();
                    ArmasSystem.instance.AtualizarDeckUI(armaAtual);

                    // Salvar após adicionar ao inventário
                    FragmentoSystem.instance.SaveFragment();
                    Debug.Log($"Dados salvos após adicionar fragmento ao inventário (fallback)");

                    // Limpar este slot
                    CleanUpSlot();
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

                // Forçar atualização da UI após adicionar
                ArmasSystem.instance.AtualizarDeckUI(armaAtual2);

                // Salvar imediatamente após todas as operações
                FragmentoSystem.instance.SaveFragment();
                Debug.Log($"Dados salvos após adicionar fragmento ao deck");
            });
        }
        else
        {
            Debug.Log("Item ou itemData é null!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Fragmento != null && Fragmento.FragmentoData != null)
        {
            ShowToolTip();
        }
        else
        {
            Debug.Log("Item não encontrado");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipItems.Instance.HideToolTip();
    }

    public void ShowToolTip()
    {
        ToolTipItems.Instance.ShowToolTipFragmento(Fragmento.FragmentoData, Fragmento.stackSize);
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
        FragmentosSlot_UI[] slotsArray = null;

        switch (tipoFragmento)
        {
            case fragmentoType.Tempo:
                slotsArray = FragmentoSystem.instance.TempoSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
                break;
            case fragmentoType.Movimento:
                slotsArray = FragmentoSystem.instance.MovimentoSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
                break;
            case fragmentoType.Vida:
                slotsArray = FragmentoSystem.instance.VidaSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
                break;
            case fragmentoType.Caos:
                slotsArray = FragmentoSystem.instance.CaosSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
                break;
            case fragmentoType.Ordem:
                slotsArray = FragmentoSystem.instance.OrdemSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
                break;
        }

        if (slotsArray != null && slotsArray.Length > 0)
        {
            // Como cada fragmento é único, procurar apenas o primeiro slot vazio
            for (int i = 0; i < slotsArray.Length; i++)
            {
                if (slotsArray[i].Fragmento == null)
                {
                    Debug.Log($"Encontrado slot vazio na posição {i} para fragmento único");
                    return slotsArray[i].transform;
                }
            }

            Debug.LogWarning($"Não há slots vazios disponíveis para o tipo {tipoFragmento}");
            return null;
        }

        Debug.LogWarning($"Não foi possível encontrar slot de destino para tipo {tipoFragmento}");
        return null;
    }

    private void MoveFragmentoToInventario(Transform destino, Action onMoveComplete)
    {
        Debug.Log($"MoveFragmentoToInventario iniciado para destino: {destino.name}");

        // Salvar posição original
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        // Criar cópia visual para animação
        GameObject animationCopy = Instantiate(gameObject, canvasFragmentoFrente);
        RectTransform animCopyRect = animationCopy.GetComponent<RectTransform>();

        // Remover componentes interativos da cópia
        Destroy(animationCopy.GetComponent<FragmentosSlot_UI>());

        // Calcular posições na tela
        Vector2 telaOrigem = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        Vector2 telaDestino = RectTransformUtility.WorldToScreenPoint(null, destino.position);

        Debug.Log($"Posição origem: {telaOrigem}, Posição destino: {telaDestino}");

        // Converter posições para coordenadas locais do canvas
        Vector2 origemLocal, destinoLocal;
        RectTransform canvasRect = canvasFragmentoFrente as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, telaOrigem, null, out origemLocal);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, telaDestino, null, out destinoLocal);

        // Posicionar cópia na posição inicial
        Vector2 offsetOrigem = new Vector2(50f, -50f);
        Vector2 offsetDestino = new Vector2(50f, -100f);
        animCopyRect.anchoredPosition = origemLocal + offsetOrigem;

        Debug.Log($"Iniciando animação de {origemLocal} para {destinoLocal}");

        // Limpar o slot original imediatamente (será repovoado pela UI update)
        CleanUpSlot();

        // Animar a cópia para a posição de destino
        animCopyRect.DOAnchorPos(destinoLocal + offsetDestino, animDuration)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                Debug.Log($"Animação para inventário completa");

                // Destruir a cópia de animação
                Destroy(animationCopy);

                // Executar callback
                onMoveComplete?.Invoke();
                ToastMessage.Instance.ShowToast("Fragmento Adicionado!", ToastType.Success);
            });
    }

    private System.Collections.IEnumerator SalvarAposDelay()
    {
        yield return new WaitForSeconds(0.1f);
        FragmentoSystem.instance.SaveFragment();
        Debug.Log($"Salvamento realizado após delay");
    }

    private System.Collections.IEnumerator SalvarAposAdicionarAoDeck()
    {
        yield return new WaitForSeconds(0.2f);
        FragmentoSystem.instance.SaveFragment();
        Debug.Log($"Salvamento realizado após adicionar ao deck");
    }
}