using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using System.Linq;

namespace InitializationSystem.View
{
    [CustomEditor(typeof(ProjectInitSettings))]
    public class ProjectInitSettingsEditor : Editor
    {
        private const string UxmlPath = "Assets/InitializationSystem/Editor/UI/ProjectInitSettingsEditor.uxml";
        private const string UssPath = "Assets/InitializationSystem/Editor/UI/ProjectInitSettingsEditor.uss";
        private const string ModulesField = "modules";

        private VisualElement _root;
        private VisualElement _settingsContainer;
        private ScrollView _modulesList;
        private Button _addModuleButton;
        private Dictionary<Object, bool> _foldoutStates = new Dictionary<Object, bool>();

        public override VisualElement CreateInspectorGUI()
        {
            // Create base UI
            _root = new VisualElement();
            EditorUIBuilder.SetupBaseUI(_root, UxmlPath, UssPath);

            // Add header
            EditorUIBuilder.AddHeader(_root, "Project Initialization Settings");

            // Setup banner if it exists
            SetupBanner();

            // Get container
            _settingsContainer = _root.Q<VisualElement>("settingsContainer") ?? _root;

            // Get all SO fields
            var soFields = EditorUtils.GetScriptableFieldNames(serializedObject);

            // Draw regular fields (excluding modules field)
            EditorUtils.DrawAllFieldsExcept(serializedObject, _settingsContainer, new[] { ModulesField });

            // Draw inline SO inspectors (except modules)
            foreach (var path in soFields)
            {
                if (path == ModulesField) continue;
                EditorUtils.DrawInlineScriptableObjectField(serializedObject, _settingsContainer, path, _foldoutStates);
            }

            // Setup modules list
            SetupModulesList();

            return _root;
        }

        private void SetupBanner()
        {
            var banner = _root.Q<VisualElement>("top_banner");
            if (banner != null)
            {
                banner.style.height = 220;
                banner.style.marginBottom = 16;
                banner.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
                banner.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
                banner.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
            }
        }

        private void SetupModulesList()
        {
            _modulesList = _root.Q<ScrollView>("modulesList");

            if (_modulesList == null)
            {
                // Create modules list
                _modulesList = new ScrollView();
                _modulesList.AddToClassList("modules-list");

                // Add modules section header
                var modulesHeader = new Label("Initialization Modules");
                modulesHeader.AddToClassList("section-header");
                _settingsContainer.Add(modulesHeader);

                _settingsContainer.Add(_modulesList);
            }

            _addModuleButton = _root.Q<Button>("addModuleButton");

            if (_addModuleButton == null)
            {
                _addModuleButton = new Button(OnAddModuleClicked);
                _addModuleButton.text = "Add Module";
                _addModuleButton.AddToClassList("add-button");
                _settingsContainer.Add(_addModuleButton);
            }
            else
            {
                _addModuleButton.clicked += OnAddModuleClicked;
                _addModuleButton.AddToClassList("add-button");
            }

            // Setup drag and drop and refresh list
            DragAndDropHandler.Setup(_modulesList, "Module", OnModuleDropped);
            RefreshModuleList();
        }

        private void RefreshModuleList()
        {
            _modulesList.Clear();
            serializedObject.Update();

            var arrayProp = serializedObject.FindProperty(ModulesField);
            if (arrayProp == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (arrayProp.arraySize == 0)
            {
                _modulesList.Add(EditorUIBuilder.CreateEmptyMessage("No modules. Drag & drop modules here or use the Add Module button."));
            }
            else
            {
                for (int i = 0; i < arrayProp.arraySize; i++)
                {
                    int index = i;  // Local copy for closure
                    var elementProp = arrayProp.GetArrayElementAtIndex(i);
                    var moduleSO = elementProp.objectReferenceValue as ScriptableObject;

                    // Create row and expandable container
                    var row = EditorUIBuilder.CreateObjectListRow(serializedObject, ModulesField, index,
                        () => {
                            EditorUtils.SafeRemoveArrayElementAt(serializedObject, ModulesField, index, "Module");
                            RefreshModuleList();
                        });
                    _modulesList.Add(row);

                    var inlineContainer = EditorUIBuilder.CreateExpandableContainer();
                    _modulesList.Add(inlineContainer);

                    if (moduleSO != null)
                        EditorUIBuilder.SetupExpandableEditor(row, inlineContainer, moduleSO);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnAddModuleClicked()
        {
            serializedObject.Update();
            var arrayProp = serializedObject.FindProperty(ModulesField);
            arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
            arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1).objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
            RefreshModuleList();
        }

        private void OnModuleDropped(Object[] objects)
        {
            if (objects == null || objects.Length == 0) return;

            var validModules = objects.Where(obj =>
                obj is ScriptableObject && obj.GetType().Name.Contains("Module")).ToList();

            if (validModules.Count == 0) return;

            serializedObject.Update();
            var arrayProp = serializedObject.FindProperty(ModulesField);
            bool dirty = false;

            foreach (var module in validModules)
            {
                // Check for duplicates
                if (EditorUtils.ContainsSameTypeObject(arrayProp, module, out string existingName))
                {
                    EditorUtility.DisplayDialog("Duplicate Module",
                        $"Module of type '{module.GetType().Name}' already exists ('{existingName}'). Skipping.",
                        "OK");
                    continue;
                }

                arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
                arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1).objectReferenceValue = module;
                dirty = true;
            }

            serializedObject.ApplyModifiedProperties();
            RefreshModuleList();

            // Mark asset dirty только если что-то добавили
            if (dirty)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }
}