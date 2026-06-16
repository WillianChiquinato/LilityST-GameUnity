using UnityEditor;

[CustomEditor(typeof(Item_drop), true)]
public class ItemDropEditor : Editor
{
    SerializedProperty possibleDrops;

    void OnEnable()
    {
        possibleDrops = serializedObject.FindProperty("possibleDrops");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Desenha o Inspector padrão
        DrawPropertiesExcluding(serializedObject, "m_Script", "possibleDrops");

        // Só mostra o possibleDrops se NÃO for PlayerItemDrop
        if (!(target is PlayerItemDrop))
        {
            EditorGUILayout.PropertyField(possibleDrops, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
