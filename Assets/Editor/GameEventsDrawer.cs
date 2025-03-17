using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(GameEvents))]
public class GameEventsDrawer : PropertyDrawer
{
    private bool isExpanded = false;
    private string searchText = "";
    private Vector2 scrollPosition;
    private const float ButtonHeight = 20f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the main enum field
        Rect enumRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if (!isExpanded)
        {
            EditorGUI.PropertyField(enumRect, property, label);
        }
        else
        {
            EditorGUI.LabelField(enumRect, label);
        }

        // Draw the dropdown button
        Rect buttonRect = new Rect(position.x + position.width - 20, position.y, 20, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(buttonRect, isExpanded ? "▲" : "▼", EditorStyles.miniButton))
        {
            isExpanded = !isExpanded;
        }

        // Draw the dropdown if expanded
        if (isExpanded)
        {
            // Calculate dropdown area
            float dropdownHeight = 200f;
            Rect dropdownRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, dropdownHeight);
            
            GUI.Box(dropdownRect, "");
            
            // Search field
            Rect searchRect = new Rect(dropdownRect.x + 2, dropdownRect.y + 2, dropdownRect.width - 4, EditorGUIUtility.singleLineHeight);
            string newSearchText = EditorGUI.TextField(searchRect, searchText);
            if (newSearchText != searchText)
            {
                searchText = newSearchText;
            }
            
            // Enum values list
            Rect listRect = new Rect(dropdownRect.x, searchRect.y + searchRect.height + 2, dropdownRect.width, dropdownRect.height - searchRect.height - 4);
            Rect viewRect = new Rect(0, 0, listRect.width - 20, GetEnumListHeight());
            
            scrollPosition = GUI.BeginScrollView(listRect, scrollPosition, viewRect);
            
            Array enumValues = Enum.GetValues(typeof(GameEvents));
            string[] enumNames = Enum.GetNames(typeof(GameEvents));
            
            float currentY = 0;
            for (int i = 0; i < enumValues.Length; i++)
            {
                string enumName = enumNames[i];
                
                // Skip if doesn't match search
                if (!string.IsNullOrEmpty(searchText) && !enumName.ToLower().Contains(searchText.ToLower()))
                    continue;
                
                Rect enumButtonRect = new Rect(0, currentY, viewRect.width, ButtonHeight);
                
                // Highlight selected value
                bool isSelected = (int)enumValues.GetValue(i) == property.enumValueIndex;
                if (isSelected)
                {
                    EditorGUI.DrawRect(enumButtonRect, new Color(0.3f, 0.5f, 0.8f, 0.3f));
                }
                
                if (GUI.Button(enumButtonRect, enumName, EditorStyles.label))
                {
                    property.enumValueIndex = (int)enumValues.GetValue(i);
                    isExpanded = false;
                }
                
                currentY += ButtonHeight;
            }
            
            GUI.EndScrollView();
        }

        EditorGUI.EndProperty();
    }

    private float GetEnumListHeight()
    {
        Array enumValues = Enum.GetValues(typeof(GameEvents));
        string[] enumNames = Enum.GetNames(typeof(GameEvents));
        
        int visibleCount = 0;
        for (int i = 0; i < enumNames.Length; i++)
        {
            if (string.IsNullOrEmpty(searchText) || enumNames[i].ToLower().Contains(searchText.ToLower()))
                visibleCount++;
        }
        
        return visibleCount * ButtonHeight;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (isExpanded)
            return EditorGUIUtility.singleLineHeight + 2 + 200f;
        else
            return EditorGUIUtility.singleLineHeight;
    }
}
#endif