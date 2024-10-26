using UnityEditor;
using UnityEngine;

public class HorizontalTrioAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(HorizontalTrioAttribute))]
public class HorizontalTrioDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Definir a largura para cada campo na linha
        float width = position.width / 3;
        
        // Ajustar as posições de cada campo para exibição horizontal
        Rect pos1 = new Rect(position.x, position.y, width, position.height);
        Rect pos2 = new Rect(position.x + width, position.y, width, position.height);
        Rect pos3 = new Rect(position.x + width * 2, position.y, width, position.height);
        
        EditorGUI.BeginProperty(position, label, property);

        //Exibir os três campos
        property.NextVisible(true);
        EditorGUI.PropertyField(pos1, property, GUIContent.none);
        property.NextVisible(true);
        EditorGUI.PropertyField(pos2, property, GUIContent.none);
        property.NextVisible(true);
        EditorGUI.PropertyField(pos3, property, GUIContent.none);
        
        EditorGUI.EndProperty();
    }
}
