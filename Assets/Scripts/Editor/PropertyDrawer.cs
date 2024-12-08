using System.IO;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResourcePrefabReference))]
public class ResourcePrefabReferencePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var resourcePath = property.FindPropertyRelative("resourcePath");
        Object resource = Resources.Load(resourcePath.stringValue);

        Object obj = EditorGUI.ObjectField(position, "Prefab", resource, typeof(Object),false);

        string path = AssetDatabase.GetAssetPath(obj);
        if (!string.IsNullOrEmpty(path))
        {
            string extension = Path.GetExtension(path);
            path = path.Replace("Assets/Resources/", string.Empty);
            path = path.Replace(extension, string.Empty);
        }
        resourcePath.stringValue = path;
        


    }
}
