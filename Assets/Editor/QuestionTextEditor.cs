using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

using ExerComps.Controls.SystemExtensions.QuestionText;

[CustomEditor(typeof(QuestionText), true)]
[CanEditMultipleObjects]
public class QuestionTextEditor : TextEditor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_embedImage"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_textures"), true);
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_imagePrefab"));
        /*
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_onImageLinkClick"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_imageContainer"));
            */
        serializedObject.ApplyModifiedProperties();
    }
    
}
