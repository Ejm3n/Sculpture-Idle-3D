﻿using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayScriptableObjectPropertiesAttribute))]
public class DisplayScriptableObjectPropertiesDrawer : PropertyDrawer
{
    static bool showProperty = false;
    float DrawerHeight = 0;

    private float buttonXofs = 15f;
    private float buttonYofs = 2f;
    private float buttonSize = 14f;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var e = Editor.CreateEditor(property.objectReferenceValue);
        var indent = EditorGUI.indentLevel;
        Rect temp = new Rect(position.x - buttonXofs, position.y + buttonYofs, buttonSize, buttonSize);
        showProperty = EditorGUI.Foldout(temp, showProperty, label);
        
        DrawerHeight = 0;
        position.height = 16;
        EditorGUI.PropertyField(position, property);
        position.y += 20;
        if (!showProperty) return;
        if (e != null)
        {
            position.x += 20;
            position.width -= 40;
            var so = e.serializedObject;
            so.Update();
            var prop = so.GetIterator();
            prop.NextVisible(true);
            int depthChilden = 0;
            bool showChilden = false;
            while (prop.NextVisible(true))
            {
                if (prop.depth == 0)
                {
                    showChilden = false;
                    depthChilden = 0;
                }

                if (showChilden && prop.depth > depthChilden)
                {
                    continue;
                }

                position.height = 16;
                EditorGUI.indentLevel = indent + prop.depth;
                if (EditorGUI.PropertyField(position, prop))
                {
                    showChilden = false;
                }
                else
                {
                    showChilden = true;
                    depthChilden = prop.depth;
                }

                position.y += 20;
                SetDrawerHeight(20);
            }

            if (GUI.changed)
            {
                so.ApplyModifiedProperties();
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);
        height += DrawerHeight;
        return height;
    }

    void SetDrawerHeight(float height)
    {
        this.DrawerHeight += height;
    }
}