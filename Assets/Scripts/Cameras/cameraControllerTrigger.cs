using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEditor;

public class CameraControllerTrigger : MonoBehaviour
{
    public CustomInspectorObje customInspectorObje;

    private Collider2D _coll;

    void Start()
    {
        _coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (customInspectorObje.panCameraContact)
            {
                //Executa o efeito da camera
                CameraManager.instance.PanCameraContact(customInspectorObje.panDistance, customInspectorObje.panTime, customInspectorObje.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (customInspectorObje.panCameraContact)
            {
                //Executa o efeito da camera
                CameraManager.instance.PanCameraContact(customInspectorObje.panDistance, customInspectorObje.panTime, customInspectorObje.panDirection, true);
            }
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
    [HideInInspector] public float panTime = 0.35f;


}

public enum PanDirecao
{
    Up,
    Down,
    Left,
    Right
}

//Aqui so colei, PQP que bgl dificil, mas sao propriedades da UNITY em si;
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
        //dfds
        DrawDefaultInspector();
        if (cameraControllerTrigger.customInspectorObje.swapCameras)
        {
            cameraControllerTrigger.customInspectorObje.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraControllerTrigger.customInspectorObje.cameraOnLeft, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControllerTrigger.customInspectorObje.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControllerTrigger.customInspectorObje.cameraOnRight, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraControllerTrigger.customInspectorObje.panCameraContact)
        {
            cameraControllerTrigger.customInspectorObje.panDirection = (PanDirecao)EditorGUILayout.EnumPopup("Camera Pan Direction", cameraControllerTrigger.customInspectorObje.panDirection);

            cameraControllerTrigger.customInspectorObje.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControllerTrigger.customInspectorObje.panDistance);
            cameraControllerTrigger.customInspectorObje.panTime = EditorGUILayout.FloatField("Pan Time", cameraControllerTrigger.customInspectorObje.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControllerTrigger);
        }
    }
}
#endif
