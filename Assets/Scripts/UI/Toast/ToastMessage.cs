using UnityEngine;

public class ToastMessage : MonoBehaviour
{
    public static ToastMessage Instance;
    public GameObject toastPrefab;
    [HideInInspector] public Toast activeToast;

    void Awake()
    {
        Instance = this;
    }

    public void ShowToast(string message, ToastType type)
    {
        if (activeToast == null)
        {
            activeToast = Instantiate(toastPrefab, transform).GetComponent<Toast>();
        }

        activeToast.Show(message, type);
    }
}
