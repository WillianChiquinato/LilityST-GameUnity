using TMPro;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public static MapUI Instance;

    public GameObject notePanel;
    public TMP_InputField inputField;

    private MapMarker currentMarker;

    void Awake()
    {
        Instance = this;
        notePanel.SetActive(false);
    }

    public void OpenNoteInput(MapMarker marker)
    {
        currentMarker = marker;
        inputField.text = "";
        notePanel.SetActive(true);
        GameManager.instance.player.canMove = false;
        inputField.ActivateInputField();
    }

    public void ConfirmNote()
    {
        currentMarker.SetNote(inputField.text);
        notePanel.SetActive(false);
        GameManager.instance.player.canMove = true;
    }
}
