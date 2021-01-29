
using UnityEditor;
using UnityEditor.UI;

using ExerComps.Controls.SystemExtensions.PaintableImage;

[CustomEditor(typeof(PaintableImage), true)]
[CanEditMultipleObjects]
public class PaintableImageEditor : ImageEditor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("lineColor"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("thickness"));
        /*
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_onImageLinkClick"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_imageContainer"));
            */
        serializedObject.ApplyModifiedProperties();
    }
    
}
