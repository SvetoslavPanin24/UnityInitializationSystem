using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace InitializationSystem.View
{
    public static class EditorUtils
    {
        public static VisualTreeAsset LoadUXML(string path) => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
       
        public static void LoadUSS(VisualElement root, string path)
        {
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            if (stylesheet != null) root.styleSheets.Add(stylesheet);
        }

        public static List<string> GetScriptableFieldNames(SerializedObject so)
        {
            var result = new List<string>();
            var iterator = so.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.name == "m_Script") continue;

                if (iterator.propertyType == SerializedPropertyType.ObjectReference &&
                    iterator.objectReferenceValue is ScriptableObject)                
                    result.Add(iterator.propertyPath);                
            }

            return result;
        }

        public static void DrawAllFieldsExcept(
            SerializedObject so,
            VisualElement container,
            params string[] exclude)
        {
            var excludeSet = new HashSet<string>(exclude ?? Array.Empty<string>());
            var iterator = so.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.name == "m_Script") continue;

                if (!excludeSet.Contains(iterator.propertyPath))
                {
                    var field = new PropertyField(iterator.Copy());
                    container.Add(field);
                    field.Bind(so);
                }
            }
        }

        public static void DrawInlineScriptableObjectField(
            SerializedObject so,
            VisualElement container,
            string propertyPath,
            Dictionary<Object, bool> foldoutStates)
        {
            so.Update();
            var prop = so.FindProperty(propertyPath);
            if (prop == null || prop.objectReferenceValue == null) return;

            var target = prop.objectReferenceValue;

            // Create container
            var inlineSoContainer = new VisualElement();
            inlineSoContainer.AddToClassList("inline-so-container");
            container.Add(inlineSoContainer);

            // Create foldout
            var displayName = ObjectNames.NicifyVariableName(prop.displayName);
            var foldout = new Foldout { text = displayName, value = false };
            foldout.AddToClassList("inline-so-foldout");
            inlineSoContainer.Add(foldout);

            // Set initial state from dictionary
            if (foldoutStates.TryGetValue(target, out bool state))
                foldout.value = state;

            // Save state on change
            foldout.RegisterValueChangedCallback(evt => {
                foldoutStates[target] = evt.newValue;
            });

            // Create inspector container
            var inlineInspector = new VisualElement();
            inlineInspector.AddToClassList("inline-so-inspector");
            foldout.Add(inlineInspector);

            // Add object field
            var objField = new ObjectField(string.Empty)
            {
                objectType = target.GetType(),
                value = target,
                allowSceneObjects = false
            };

            objField.RegisterValueChangedCallback(evt => {
                so.Update();
                prop.objectReferenceValue = evt.newValue;
                so.ApplyModifiedProperties();
            });

            inlineInspector.Add(objField);

            // Create and add editor
            var editor = Editor.CreateEditor(target);
            VisualElement inspectorElement;

            try { inspectorElement = editor.CreateInspectorGUI(); }
            catch { inspectorElement = new IMGUIContainer(editor.OnInspectorGUI); }

            inspectorElement.Bind(editor.serializedObject);
            inlineInspector.Add(inspectorElement);
        }

        /// <summary>
        /// Safely removes an element from a serialized array with confirmation dialog
        /// </summary>
        public static void SafeRemoveArrayElementAt(
            SerializedObject so,
            string arrayPath,
            int index,
            string elementTypeName)
        {
            so.Update();
            var arrayProp = so.FindProperty(arrayPath);

            // Validate index
            if (index < 0 || index >= arrayProp.arraySize)
            {
                Debug.LogError($"Cannot remove {elementTypeName} at index {index}: Index out of range");
                return;
            }

            // Get object reference
            var elementProp = arrayProp.GetArrayElementAtIndex(index);
            Object elementObject = elementProp.objectReferenceValue;

            // Confirm deletion if needed
            bool shouldDelete = true;
            if (elementObject != null)
                shouldDelete = EditorUtility.DisplayDialog($"Remove {elementTypeName}",
                    $"Are you sure you want to remove '{elementObject.name}'?", "Remove", "Cancel");

            if (shouldDelete)
            {
                // Revalidate index (could have changed during dialog)
                so.Update();
                arrayProp = so.FindProperty(arrayPath);

                if (index < 0 || index >= arrayProp.arraySize)
                {
                    Debug.LogError($"Index became invalid during confirmation: {index}");
                    return;
                }

                // Delete the element
                arrayProp.DeleteArrayElementAtIndex(index);
                so.ApplyModifiedProperties();
            }
        }
        public static bool ContainsSameTypeObject(SerializedProperty arrayProp, Object objectToAdd, out string existingObjectName, int excludeIndex = -1)
        {
            existingObjectName = null;

            if (arrayProp == null || objectToAdd == null) return false;

            Type typeToAdd = objectToAdd.GetType();

            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                // Skip the current index if it's provided
                if (i == excludeIndex) continue;

                var element = arrayProp.GetArrayElementAtIndex(i);
                var existingObject = element.objectReferenceValue;

                if (existingObject != null && existingObject.GetType() == typeToAdd)
                {
                    existingObjectName = existingObject.name;
                    return true;
                }
            }

            return false;
        } 
    }
}