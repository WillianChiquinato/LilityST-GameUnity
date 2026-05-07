using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEquipSelector : MonoBehaviour
{
    public static WeaponEquipSelector instance;

    [Header("Carousel Primária (← →)")]
    [SerializeField] private Image prevPrimaryIcon;
    [SerializeField] private Image currentPrimaryIcon;
    [SerializeField] private Image nextPrimaryIcon;
    [SerializeField] private TextMeshProUGUI primaryLabel;

    [Header("Carousel Secundária (↑ ↓)")]
    [SerializeField] private Image prevSecondaryIcon;
    [SerializeField] private Image currentSecondaryIcon;
    [SerializeField] private Image nextSecondaryIcon;
    [SerializeField] private TextMeshProUGUI secondaryLabel;

    [Header("Visual")]
    [Range(0f, 1f)]
    [SerializeField] private float dimAlpha = 0.4f;

    [Header("Animacao Troca")]
    [SerializeField] private float swapDuration = 0.18f;
    [SerializeField] private float swapDistance = 120f;

    // Listas de armas disponíveis, filtradas do SaveData
    private List<PowerUps> availablePrimary = new List<PowerUps>();
    private List<PowerUps> availableSecondary = new List<PowerUps>();

    private int primaryIndex = 0;
    private int secondaryIndex = 0;
    private bool isPrimaryAnimating = false;
    private bool isSecondaryAnimating = false;

    // Mapeamento sprite por arma (deve bater com os paths em Resources/)
    private static readonly Dictionary<PowerUps, string> SpritePathMap = new Dictionary<PowerUps, string>
    {
        { PowerUps.Bastao,  "Sprites/Armas/BastaoIcon" },
        { PowerUps.Marreta, "Sprites/Armas/Marreta"    },
        { PowerUps.Arco,    "Sprites/Armas/ArcoIcon"   },
        { PowerUps.Sino,    "Sprites/Armas/Sino"       },
        { PowerUps.Mascara, "Sprites/Armas/Mascara"    },
    };

    private static readonly Dictionary<PowerUps, string> DisplayName = new Dictionary<PowerUps, string>
    {
        { PowerUps.Bastao,  "Bastão"  },
        { PowerUps.Marreta, "Marreta" },
        { PowerUps.Arco,    "Arco"    },
        { PowerUps.Sino,    "Sino"    },
        { PowerUps.Mascara, "Máscara" },
    };

    #region Unity Lifecycle

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        BuildAvailableLists();
        RefreshAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) CyclePrimary(+1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) CyclePrimary(-1);

        if (Input.GetKeyDown(KeyCode.UpArrow)) CycleSecondary(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) CycleSecondary(+1);
    }

    #endregion

    #region Build & Cycle

    public void BuildAvailableLists()
    {
        availablePrimary.Clear();
        availableSecondary.Clear();

        foreach (PowerUpData pu in SaveData.Instance.powerUps)
        {
            if (pu.category == PowerUpCategory.ArmaPrimaria)
                availablePrimary.Add(pu.name);
            else if (pu.category == PowerUpCategory.ArmaSecundaria)
                availableSecondary.Add(pu.name);
        }

        // Restaura seleção salva — cai para 0 se não encontrar
        primaryIndex = FindIndex(availablePrimary, SaveData.Instance.equippedPrimary);
        secondaryIndex = FindIndex(availableSecondary, SaveData.Instance.equippedSecondary);
    }

    private void CyclePrimary(int direction)
    {
        if (availablePrimary.Count == 0) return;

        int nextIndex = Wrap(primaryIndex + direction, availablePrimary.Count);
        if (nextIndex == primaryIndex) return;

        if (isPrimaryAnimating || availablePrimary.Count < 2 || currentPrimaryIcon == null)
        {
            CommitPrimary(nextIndex);
            return;
        }

        StartCoroutine(AnimateCenterSwap(
            prevPrimaryIcon,
            currentPrimaryIcon,
            nextPrimaryIcon,
            availablePrimary[nextIndex],
            availablePrimary.Count,
            direction,
            () => CommitPrimary(nextIndex),
            () => isPrimaryAnimating = false));

        isPrimaryAnimating = true;
    }

    private void CycleSecondary(int direction)
    {
        if (availableSecondary.Count == 0) return;

        int nextIndex = Wrap(secondaryIndex + direction, availableSecondary.Count);
        if (nextIndex == secondaryIndex) return;

        if (isSecondaryAnimating || availableSecondary.Count < 2 || currentSecondaryIcon == null)
        {
            CommitSecondary(nextIndex);
            return;
        }

        StartCoroutine(AnimateCenterSwap(
            prevSecondaryIcon,
            currentSecondaryIcon,
            nextSecondaryIcon,
            availableSecondary[nextIndex],
            availableSecondary.Count,
            direction,
            () => CommitSecondary(nextIndex),
            () => isSecondaryAnimating = false));

        isSecondaryAnimating = true;
    }

    #endregion
    #region UI Refresh

    private void RefreshAll()
    {
        RefreshPrimary();
        RefreshSecondary();
    }

    private void RefreshPrimary()
    {
        RefreshRow(availablePrimary, primaryIndex,
                   prevPrimaryIcon, currentPrimaryIcon, nextPrimaryIcon, primaryLabel);
    }

    private void RefreshSecondary()
    {
        RefreshRow(availableSecondary, secondaryIndex,
                   prevSecondaryIcon, currentSecondaryIcon, nextSecondaryIcon, secondaryLabel);
    }

    private void RefreshRow(List<PowerUps> list, int index,
                            Image prevSlot, Image currentSlot, Image nextSlot,
                            TextMeshProUGUI label)
    {
        if (list.Count == 0)
        {
            ClearSlot(currentSlot);
            ClearSlot(prevSlot);
            ClearSlot(nextSlot);
            if (label) label.text = "Nenhuma";
            return;
        }

        ApplySprite(currentSlot, list[index], 1f);
        if (label) label.text = GetDisplayName(list[index]);

        if (list.Count == 1)
        {
            // Só existe um item — não repete nos lados
            ClearSlot(prevSlot);
            ClearSlot(nextSlot);
        }
        else if (list.Count == 2)
        {
            // Existe só um "outro" item — mostra à esquerda, direita vazia
            int other = Wrap(index + 1, list.Count);
            ApplySprite(prevSlot, list[other], dimAlpha);
            ClearSlot(nextSlot);
        }
        else
        {
            // 3 ou mais: carousel completo
            int prev = Wrap(index - 1, list.Count);
            int next = Wrap(index + 1, list.Count);
            ApplySprite(prevSlot,  list[prev], dimAlpha);
            ApplySprite(nextSlot,  list[next], dimAlpha);
        }
    }

    #endregion
    #region Helpers

    private void CommitPrimary(int nextIndex)
    {
        primaryIndex = nextIndex;
        SaveData.Instance.equippedPrimary = availablePrimary[primaryIndex].ToString();
        RefreshPrimary();
    }

    private void CommitSecondary(int nextIndex)
    {
        secondaryIndex = nextIndex;
        SaveData.Instance.equippedSecondary = availableSecondary[secondaryIndex].ToString();
        RefreshSecondary();
    }

    private System.Collections.IEnumerator AnimateCenterSwap(
        Image prevSlot,
        Image centerSlot,
        Image nextSlot,
        PowerUps incomingWeapon,
        int listCount,
        int direction,
        Action onComplete,
        Action onFinally)
    {
        Sprite incomingSprite = GetSprite(incomingWeapon);
        if (incomingSprite == null)
        {
            onComplete?.Invoke();
            onFinally?.Invoke();
            yield break;
        }

        RectTransform parentRect = centerSlot.transform.parent as RectTransform;
        if (parentRect == null)
        {
            onComplete?.Invoke();
            onFinally?.Invoke();
            yield break;
        }

        Vector2 centerPos = GetSlotCenterInParent(centerSlot.rectTransform, parentRect);
        Vector2 prevPos = prevSlot != null
            ? GetSlotCenterInParent(prevSlot.rectTransform, parentRect)
            : centerPos + new Vector2(-swapDistance, 0f);
        Vector2 nextPos = nextSlot != null
            ? GetSlotCenterInParent(nextSlot.rectTransform, parentRect)
            : centerPos + new Vector2(swapDistance, 0f);

        // Para 2 itens o layout mostra só o lado esquerdo (prev), então a troca sempre vem dele.
        // Para 3+ mantém regra normal:
        //   next -> current e current -> prev
        //   prev -> current e current -> next
        bool twoItemsMode = listCount == 2;
        Vector2 incomingStart = twoItemsMode ? prevPos : (direction >= 0 ? nextPos : prevPos);
        Vector2 outgoingEnd = twoItemsMode ? prevPos : (direction >= 0 ? prevPos : nextPos);

        Image outgoing = CreateOverlayImage(centerSlot, centerSlot.sprite, centerPos);
        Image incoming = CreateOverlayImage(centerSlot, incomingSprite, incomingStart);

        Color centerStartColor = centerSlot.color;
        centerSlot.enabled = false;

        Color outgoingColor = outgoing.color;
        Color incomingColor = incoming.color;

        float duration = Mathf.Max(0.01f, swapDuration);
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            float ease = p * p * (3f - 2f * p);

            outgoing.rectTransform.localPosition = Vector2.LerpUnclamped(centerPos, outgoingEnd, ease);
            outgoingColor.a = centerStartColor.a * (1f - ease);
            outgoing.color = outgoingColor;

            incoming.rectTransform.localPosition = Vector2.LerpUnclamped(incomingStart, centerPos, ease);
            incomingColor.a = ease;
            incoming.color = incomingColor;

            yield return null;
        }

        centerSlot.color = centerStartColor;
        centerSlot.enabled = centerSlot.sprite != null;

        if (outgoing != null)
            Destroy(outgoing.gameObject);

        if (incoming != null)
            Destroy(incoming.gameObject);

        onComplete?.Invoke();
        onFinally?.Invoke();
    }

    private Image CreateOverlayImage(Image source, Sprite sprite, Vector2 startPosition)
    {
        GameObject go = new GameObject(
            "WeaponSwapOverlay",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(LayoutElement),
            typeof(Image));
        go.transform.SetParent(source.transform.parent, false);

        Image img = go.GetComponent<Image>();
        LayoutElement layoutElement = go.GetComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        img.sprite = sprite;
        img.type = source.type;
        img.fillMethod = source.fillMethod;
        img.fillOrigin = source.fillOrigin;
        img.fillAmount = source.fillAmount;
        img.fillClockwise = source.fillClockwise;
        img.maskable = source.maskable;
        img.preserveAspect = source.preserveAspect;
        img.raycastTarget = false;

        RectTransform src = source.rectTransform;
        RectTransform dst = img.rectTransform;
        dst.anchorMin = new Vector2(0.5f, 0.5f);
        dst.anchorMax = new Vector2(0.5f, 0.5f);
        dst.pivot = new Vector2(0.5f, 0.5f);
        dst.sizeDelta = src.sizeDelta;
        dst.localPosition = startPosition;
        dst.SetAsLastSibling();

        Color c = source.color;
        c.a = Mathf.Clamp01(c.a);
        img.color = c;

        return img;
    }

    private Vector2 GetSlotCenterInParent(RectTransform slotRect, RectTransform parentRect)
    {
        Vector3 worldCenter = slotRect.TransformPoint(slotRect.rect.center);
        return parentRect.InverseTransformPoint(worldCenter);
    }

    private Sprite GetSprite(PowerUps arma)
    {
        if (!SpritePathMap.TryGetValue(arma, out string path))
            return null;

        return Resources.Load<Sprite>(path);
    }

    private void ApplySprite(Image slot, PowerUps arma, float alpha)
    {
        if (slot == null) return;

        slot.sprite = GetSprite(arma);

        Color c = slot.color;
        c.a = alpha;
        slot.color = c;
        slot.enabled = slot.sprite != null;
    }

    private void ClearSlot(Image slot)
    {
        if (slot == null) return;
        slot.sprite = null;
        slot.enabled = false;
    }

    private string GetDisplayName(PowerUps arma) =>
        DisplayName.TryGetValue(arma, out string name) ? name : arma.ToString();

    private int FindIndex(List<PowerUps> list, string savedName)
    {
        if (string.IsNullOrEmpty(savedName)) return 0;

        if (Enum.TryParse(savedName, out PowerUps parsed))
        {
            int idx = list.IndexOf(parsed);
            if (idx >= 0) return idx;
        }
        return 0;
    }
    private static int Wrap(int index, int count) => ((index % count) + count) % count;

    #endregion
    #region Public Reference

    public PowerUps? GetEquippedPrimary() =>
        availablePrimary.Count > 0 ? availablePrimary[primaryIndex] : (PowerUps?)null;

    public PowerUps? GetEquippedSecondary() =>
        availableSecondary.Count > 0 ? availableSecondary[secondaryIndex] : (PowerUps?)null;

    public void OnSaveDataChanged()
    {
        BuildAvailableLists();
        RefreshAll();
    }

    #endregion
}
