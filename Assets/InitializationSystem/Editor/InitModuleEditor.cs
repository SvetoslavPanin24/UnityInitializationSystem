using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using System.Linq;
using Unity.VisualScripting;

namespace InitializationSystem.View
{
    [CustomEditor(typeof(InitModule), true)]
    public class InitModuleEditor : Editor
    {
        private const string UxmlPath = "Assets/InitializationSystem/Editor/UI/InitModuleEditor.uxml";
        private const string UssPath = "Assets/InitializationSystem/Editor/UI/ProjectInitSettingsEditor.uss";

        private VisualElement _root;
        private VisualElement _settingsContainer;
        private ScrollView _handlersList;
        private Button _addHandlerButton;
        private string _handlerArrayName;

        // Shared across all instances
        private static readonly Dictionary<Object, bool> _foldoutStates = new Dictionary<Object, bool>();

        public override VisualElement CreateInspectorGUI()
        {
            // Create base UI
            _root = new VisualElement();
            EditorUIBuilder.SetupBaseUI(_root, UxmlPath, UssPath);

            // Add header with module name
            EditorUIBuilder.AddHeader(_root, target.name);

            // Add module type if needed
            var typeName = ObjectNames.NicifyVariableName(target.GetType().Name.Replace("Module", ""));
            if (!string.IsNullOrEmpty(typeName) && typeName != "Init")
                EditorUIBuilder.AddTypeLabel(_root, typeName);

            // Find handler array name
            _handlerArrayName = FindFirstHandlerArray();

            // Get container
            _settingsContainer = _root.Q<VisualElement>("settingsContainer") ?? _root;

            // Get all SO field paths
            var soFields = EditorUtils.GetScriptableFieldNames(serializedObject);

            // Draw regular fields excluding handlers array and SO fields
            var excludeFields = new List<string> { _handlerArrayName };
            excludeFields.AddRange(soFields);
            EditorUtils.DrawAllFieldsExcept(serializedObject, _settingsContainer, excludeFields.ToArray());

            // Draw inline SO inspectors
            foreach (var path in soFields)
            {
                if (path == _handlerArrayName) continue;
                EditorUtils.DrawInlineScriptableObjectField(serializedObject, _settingsContainer, path, _foldoutStates);
            }

            // Setup handlers list (if handler array exists)
            if (!string.IsNullOrEmpty(_handlerArrayName))
                SetupHandlersList();

            return _root;
        }

        private void SetupHandlersList()
        {
            _handlersList = _root.Q<ScrollView>("handlersList");

            if (_handlersList == null)
            {
                // Add handlers section header
                var handlersHeader = new Label("Handlers");
                handlersHeader.AddToClassList("section-header");
                _settingsContainer.Add(handlersHeader);

                // Create handlers list
                _handlersList = new ScrollView();
                _handlersList.AddToClassList("handlers-list");
                _settingsContainer.Add(_handlersList);

                // Add handler button
                _addHandlerButton = new Button(OnAddHandlerClicked);
                _addHandlerButton.text = "Add Handler";
                _addHandlerButton.AddToClassList("add-button");
                _settingsContainer.Add(_addHandlerButton);
            }
            else
            {
                _addHandlerButton = _root.Q<Button>("addHandlerButton");
                if (_addHandlerButton != null)
                    _addHandlerButton.clicked += OnAddHandlerClicked;
            }

            // Setup drag and drop and refresh list
            DragAndDropHandler.Setup(_handlersList, "Handler", OnHandlerDropped);
            RefreshHandlerList();
        }

        private void RefreshHandlerList()
        {
            if (_handlersList == null || string.IsNullOrEmpty(_handlerArrayName)) return;

            _handlersList.Clear();
            serializedObject.Update();

            var arrayProp = serializedObject.FindProperty(_handlerArrayName);
            if (arrayProp == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (arrayProp.arraySize == 0)
            {
                _handlersList.Add(EditorUIBuilder.CreateEmptyMessage("No handlers. Drag & drop handlers here or use the Add Handler button."));
            }
            else
            {
                for (int i = 0; i < arrayProp.arraySize; i++)
                {
                    int index = i;  // Local copy for closure
                    var elementProp = arrayProp.GetArrayElementAtIndex(i);
                    var handlerObj = elementProp.objectReferenceValue as ScriptableObject;

                    // Create row and expandable container
                    var row = EditorUIBuilder.CreateObjectListRow(serializedObject, _handlerArrayName, index,
                        () => {
                            EditorUtils.SafeRemoveArrayElementAt(serializedObject, _handlerArrayName, index, "Handler");
                            RefreshHandlerList();  
                        });
                    _handlersList.Add(row);

                    var inlineContainer = EditorUIBuilder.CreateExpandableContainer();
                    _handlersList.Add(inlineContainer);

                    if (handlerObj != null)
                        EditorUIBuilder.SetupExpandableEditor(row, inlineContainer, handlerObj);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnAddHandlerClicked()
        {
            serializedObject.Update();
            var arrayProp = serializedObject.FindProperty(_handlerArrayName);
            arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
            arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1).objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
            RefreshHandlerList();
        }

        private void OnHandlerDropped(Object[] objects)
        {
            if (objects == null || objects.Length == 0) return;

            var validHandlers = objects.Where(obj =>
                obj is ScriptableObject && obj.GetType().Name.Contains("Handler")).ToList();

            if (validHandlers.Count == 0) return;

            serializedObject.Update();
            var arrayProp = serializedObject.FindProperty(_handlerArrayName);
            bool dirty = false;

            foreach (var handler in validHandlers)
            {
                // Check for duplicates
                if (EditorUtils.ContainsSameTypeObject(arrayProp, handler, out string existingName))
                {
                    EditorUtility.DisplayDialog("Duplicate Handler",
                        $"Handler of type '{handler.GetType().Name}' already exists ('{existingName}'). Skipping.",
                        "OK");
                    continue;
                }

                arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
                arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1).objectReferenceValue = handler;
                dirty = true;
            }

            serializedObject.ApplyModifiedProperties();
            RefreshHandlerList();

            // Mark asset dirty только если что-то добавили
            if (dirty)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private string FindFirstHandlerArray()
        {
            var targetType = target.GetType();

            foreach (var field in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                    continue;

                Type elementType = null;

                if (field.FieldType.IsArray)
                    elementType = field.FieldType.GetElementType();
                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    elementType = field.FieldType.GetGenericArguments()[0];

                if (elementType != null && (typeof(Handler).IsAssignableFrom(elementType) || elementType.Name.Contains("Handler")))
                    return field.Name;
            }

            return null;
        }
    }
}