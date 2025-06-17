using UnityEditor;

[CustomEditor(typeof(Item_drop), true)]
public class ItemDropEditor : Editor
{
    SerializedProperty possibleDrop;

    void OnEnable()
    {
        possibleDrop = serializedObject.FindProperty("possibleDrop");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Desenha o Inspector padrão
        DrawPropertiesExcluding(serializedObject, "m_Script", "possibleDrop");

        // Só mostra o possibleDrop se NÃO for PlayerItemDrop
        if (!(target is PlayerItemDrop))
        {
            EditorGUILayout.PropertyField(possibleDrop);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
