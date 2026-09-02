using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisionDetectPlayer))]
public class VisionDetectPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VisionDetectPlayer script = (VisionDetectPlayer)target;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Configuração", EditorStyles.boldLabel);

        script.tipoDeteccao = (VisionDetectPlayer.TipoDeteccao)
            EditorGUILayout.EnumPopup("Tipo Detecção", script.tipoDeteccao);

        if (script.tipoDeteccao == VisionDetectPlayer.TipoDeteccao.Range)
        {
            script.raioDeteccao = EditorGUILayout.FloatField(
                "Raio Detecção",
                script.raioDeteccao
            );
        }
        else if (script.tipoDeteccao == VisionDetectPlayer.TipoDeteccao.Visao)
        {
            script.anguloVisao = EditorGUILayout.FloatField(
                "Ângulo Visão",
                script.anguloVisao
            );

            script.distanciaVisao = EditorGUILayout.FloatField(
                "Distância Visão",
                script.distanciaVisao
            );
        }

        script.obstaculoLayer = LayerMaskField(
            "Obstáculo Layer",
            script.obstaculoLayer
        );

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Persistência de Aggro",
            EditorStyles.boldLabel
        );

        script.manterAggroAoPerderVisao = EditorGUILayout.Toggle(
            "Manter Aggro",
            script.manterAggroAoPerderVisao
        );

        script.tempoPerderAggro = EditorGUILayout.FloatField(
            "Tempo Perder Aggro",
            script.tempoPerderAggro
        );

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }

    private static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        return EditorGUILayout.MaskField(
            label,
            layerMask,
            UnityEditorInternal.InternalEditorUtility.layers
        );
    }
}
