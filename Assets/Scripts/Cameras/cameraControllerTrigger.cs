using Cinemachine;
using UnityEngine;
using UnityEditor;

public class CameraControllerTrigger : MonoBehaviour
{
    public CustomInspectorObje customInspectorObje;
    public bool PlayerDetect;

    private Collider2D _coll;

    void Start()
    {
        _coll = GetComponent<Collider2D>();
        PlayerDetect = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerDetect = true;

            if (customInspectorObje.panCameraContact && !collision.gameObject.GetComponent<PlayerMoviment>().arcoEffect)
            {
                Vector2 panValues = GetPanValues(customInspectorObje.panDirection, customInspectorObje.panDistance, customInspectorObje.panDistance2);

                CameraManager.instance.PanCameraContact(
                    panValues, 
                    customInspectorObje.panTime, 
                    customInspectorObje.panDirection, 
                    false
                );
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerDetect = false;

            if (customInspectorObje.panCameraContact)
            {
                Vector2 panValues = GetPanValues(customInspectorObje.panDirection, customInspectorObje.panDistance, customInspectorObje.panDistance2);

                CameraManager.instance.PanCameraContact(
                    panValues, 
                    customInspectorObje.panTime, 
                    customInspectorObje.panDirection, 
                    true
                );
            }
        }
    }

    private Vector2 GetPanValues(PanDirecao dir, float d1, float d2)
    {
        // Retorna (x, y) baseado na direção
        switch (dir)
        {
            case PanDirecao.Up: return new Vector2(0, d1);
            case PanDirecao.Down: return new Vector2(0, -d1);
            case PanDirecao.Left: return new Vector2(-d1, 0);
            case PanDirecao.Right: return new Vector2(d1, 0);

            case PanDirecao.UpLeft: return new Vector2(-d2, d1);
            case PanDirecao.UpRight: return new Vector2(d2, d1);
            case PanDirecao.DownLeft: return new Vector2(-d2, -d1);
            case PanDirecao.DownRight: return new Vector2(d2, -d1);

            default: return Vector2.zero;
        }
    }
}

[System.Serializable]
public class CustomInspectorObje
{
    public bool swapCameras = false;
    public bool panCameraContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirecao panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panDistance2 = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum PanDirecao
{
    Up,
    Down,
    Left,
    Right,
    UpRight,
    UpLeft,
    DownRight,
    DownLeft
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraControllerTrigger))]
public class myScriptEditor : Editor
{
    CameraControllerTrigger cameraControllerTrigger;

    private void OnEnable()
    {
        cameraControllerTrigger = (CameraControllerTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraControllerTrigger.customInspectorObje.swapCameras)
        {
            cameraControllerTrigger.customInspectorObje.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraControllerTrigger.customInspectorObje.cameraOnLeft, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            cameraControllerTrigger.customInspectorObje.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControllerTrigger.customInspectorObje.cameraOnRight, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraControllerTrigger.customInspectorObje.panCameraContact)
        {
            cameraControllerTrigger.customInspectorObje.panDirection = (PanDirecao)EditorGUILayout.EnumPopup("Camera Pan Direction", cameraControllerTrigger.customInspectorObje.panDirection);
            cameraControllerTrigger.customInspectorObje.panDistance = EditorGUILayout.FloatField("Pan Distance (Primary)", cameraControllerTrigger.customInspectorObje.panDistance);

            // Mostra o segundo campo só se a direção for diagonal
            if (cameraControllerTrigger.customInspectorObje.panDirection == PanDirecao.UpLeft ||
                cameraControllerTrigger.customInspectorObje.panDirection == PanDirecao.UpRight ||
                cameraControllerTrigger.customInspectorObje.panDirection == PanDirecao.DownLeft ||
                cameraControllerTrigger.customInspectorObje.panDirection == PanDirecao.DownRight)
            {
                cameraControllerTrigger.customInspectorObje.panDistance2 = EditorGUILayout.FloatField("Pan Distance 2 (Secondary)", cameraControllerTrigger.customInspectorObje.panDistance2);
            }

            cameraControllerTrigger.customInspectorObje.panTime = EditorGUILayout.FloatField("Pan Time", cameraControllerTrigger.customInspectorObje.panTime);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(cameraControllerTrigger);
    }
}
#endif
