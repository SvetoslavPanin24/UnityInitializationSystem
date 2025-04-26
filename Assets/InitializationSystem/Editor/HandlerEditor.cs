using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace InitializationSystem.View
{
    [CustomEditor(typeof(Handler), true)]
    public class HandlerEditor : Editor
    {
        private const string UxmlPath = "Assets/InitializationSystem/Editor/UI/HandlerEditor.uxml";
        private const string UssPath = "Assets/InitializationSystem/Editor/UI/ProjectInitSettingsEditor.uss";

        // Shared across all instances
        private static readonly Dictionary<Object, bool> _foldoutStates = new Dictionary<Object, bool>();

        public override VisualElement CreateInspectorGUI()
        {
            // Create base UI
            var root = new VisualElement();
            EditorUIBuilder.SetupBaseUI(root, UxmlPath, UssPath);

            // Add header with handler name
            EditorUIBuilder.AddHeader(root, target.name);

            // Add type label if needed
            var typeName = ObjectNames.NicifyVariableName(target.GetType().Name.Replace("Handler", ""));
            if (!string.IsNullOrEmpty(typeName) && typeName != "")
                EditorUIBuilder.AddTypeLabel(root, typeName);

            // Get container
            var settingsContainer = root.Q<VisualElement>("settingsContainer") ?? root;

            // Get all SO field paths
            var soFields = EditorUtils.GetScriptableFieldNames(serializedObject);

            // Draw fields and inline inspectors
            EditorUtils.DrawAllFieldsExcept(serializedObject, settingsContainer, soFields.ToArray());

            foreach (var path in soFields)
                EditorUtils.DrawInlineScriptableObjectField(serializedObject, settingsContainer, path, _foldoutStates);

            return root;
        }
    }
}