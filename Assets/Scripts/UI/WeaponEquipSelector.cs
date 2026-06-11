using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEquipSelector : MonoBehaviour
{
    public static WeaponEquipSelector instance;

    [Header("Slot Primária (← →)")]
    [SerializeField] private Image currentPrimaryIcon;

    [Header("Slot Secundária (↑ ↓)")]
    [SerializeField] private Image currentSecondaryIcon;

    [Header("Animacao Troca")]
    [SerializeField] private float fadeDuration = 0.18f;

    private List<PowerUps> availablePrimary = new List<PowerUps>();
    private List<PowerUps> availableSecondary = new List<PowerUps>();

    private int primaryIndex = 0;
    private int secondaryIndex = 0;
    private bool isPrimaryAnimating = false;
    private bool isSecondaryAnimating = false;

    private static readonly Dictionary<PowerUps, string> SpritePathMap = new Dictionary<PowerUps, string>
    {
        { PowerUps.Bastao,  "Sprites/Armas/IconesSelected/BastaoIcone" },
        { PowerUps.Marreta, "Sprites/Armas/IconesSelected/MarretaIcone"    },
        { PowerUps.Arco,    "Sprites/Armas/IconesSelected/ArcoIcone"   },
        { PowerUps.Sino,    "Sprites/Armas/IconesSelected/SinoIcone"       },
        { PowerUps.Mascara, "Sprites/Armas/IconesSelected/MascaraIcone"    },
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

        primaryIndex = FindIndex(availablePrimary, SaveData.Instance.equippedPrimary);
        secondaryIndex = FindIndex(availableSecondary, SaveData.Instance.equippedSecondary);
    }

    private void CyclePrimary(int direction)
    {
        if (availablePrimary.Count == 0) return;

        int nextIndex = Wrap(primaryIndex + direction, availablePrimary.Count);
        if (nextIndex == primaryIndex) return;

        if (isPrimaryAnimating || currentPrimaryIcon == null)
        {
            CommitPrimary(nextIndex);
            return;
        }

        isPrimaryAnimating = true;
        StartCoroutine(AnimateFadeSwap(
            currentPrimaryIcon,
            () => CommitPrimary(nextIndex),
            () => isPrimaryAnimating = false));
    }

    private void CycleSecondary(int direction)
    {
        if (availableSecondary.Count == 0) return;

        int nextIndex = Wrap(secondaryIndex + direction, availableSecondary.Count);
        if (nextIndex == secondaryIndex) return;

        if (isSecondaryAnimating || currentSecondaryIcon == null)
        {
            CommitSecondary(nextIndex);
            return;
        }

        isSecondaryAnimating = true;
        StartCoroutine(AnimateFadeSwap(
            currentSecondaryIcon,
            () => CommitSecondary(nextIndex),
            () => isSecondaryAnimating = false));
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
        if (availablePrimary.Count == 0)
        {
            ClearSlot(currentPrimaryIcon);
            return;
        }

        ApplySprite(currentPrimaryIcon, availablePrimary[primaryIndex], 1f);
    }

    private void RefreshSecondary()
    {
        if (availableSecondary.Count == 0)
        {
            ClearSlot(currentSecondaryIcon);
            return;
        }

        ApplySprite(currentSecondaryIcon, availableSecondary[secondaryIndex], 1f);
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

    // Fade out -> troca sprite (onCommit) -> fade in
    private System.Collections.IEnumerator AnimateFadeSwap(
        Image slot,
        Action onCommit,
        Action onFinally)
    {
        float half = Mathf.Max(0.01f, fadeDuration * 0.5f);
        Color c = slot.color;

        // Fade out
        float t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / half);
            slot.color = c;
            yield return null;
        }
        c.a = 0f;
        slot.color = c;

        // Troca o sprite (Refresh seta alpha=1, forcamos de volta a 0 antes do fade in)
        onCommit?.Invoke();
        c = slot.color;
        c.a = 0f;
        slot.color = c;

        // Fade in
        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / half);
            slot.color = c;
            yield return null;
        }
        c.a = 1f;
        slot.color = c;

        onFinally?.Invoke();
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
