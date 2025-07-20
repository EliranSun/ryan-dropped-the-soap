#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Dialog
{
    [CustomPropertyDrawer(typeof(DialogLine))]
    public class DialogLinePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // Limit the depth to prevent infinite recursion
            if (property.depth > 3)
            {
                EditorGUI.LabelField(position, label.text, "Reference (depth limited)");
                EditorGUI.EndProperty();
                return;
            }
            
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }
    }
}
#endif