using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace InitializationSystem.View
{
    /// <summary>
    /// Helper class for building UI elements for custom editors
    /// </summary>
    public static class EditorUIBuilder
    {
        /// <summary>
        /// Sets up the base UI by loading UXML and USS files
        /// </summary>
        public static void SetupBaseUI(VisualElement root, string uxmlPath, string ussPath)
        {
            var tree = EditorUtils.LoadUXML(uxmlPath);
            if (tree != null) root.Add(tree.CloneTree());
            EditorUtils.LoadUSS(root, ussPath);
        }

        /// <summary>
        /// Adds a header to the root element
        /// </summary>
        public static void AddHeader(VisualElement root, string text)
        {
            var header = new Label(text);
            header.AddToClassList("header");
            root.Insert(0, header);
        }

        /// <summary>
        /// Adds a type label to the root element
        /// </summary>
        public static void AddTypeLabel(VisualElement root, string text)
        {
            var typeLabel = new Label(text);
            typeLabel.AddToClassList("type-label");
            root.Insert(1, typeLabel);
        }

        /// <summary>
        /// Creates an object list row with an object field and a remove button
        /// </summary>
        public static VisualElement CreateObjectListRow(SerializedObject so, string arrayPath, int index, Action removeCallback, string itemTypeName = null)
        {
            so.Update();
            var arrProp = so.FindProperty(arrayPath);
            var prop = arrProp.GetArrayElementAtIndex(index);
            var obj = prop.objectReferenceValue;

            var row = new VisualElement();
            row.AddToClassList("list-item");
            row.AddToClassList("clickable");

            var headerRow = new VisualElement();
            headerRow.AddToClassList("header-row");
            row.Add(headerRow);

            // Object field
            var objField = new ObjectField
            {
                objectType = typeof(ScriptableObject),
                allowSceneObjects = false,
                value = obj,
                style = { flexGrow = 1 }
            };

            objField.RegisterValueChangedCallback(evt =>
            {
                so.Update();
                var newValue = evt.newValue;

                // Check for duplicates if a new value is being assigned
                if (newValue != null)
                {
                    if (EditorUtils.ContainsSameTypeObject(arrProp, newValue, out string existingName, index))
                    {
                        // Alert about duplicate and revert to previous value
                        EditorUtility.DisplayDialog("Duplicate Object",
                            $"Object of type '{newValue.GetType().Name}' already exists ('{existingName}'). Cannot add duplicate.",
                            "OK");

                        objField.SetValueWithoutNotify(evt.previousValue);
                        return;
                    }
                }

                var curProp = arrProp.GetArrayElementAtIndex(index);
                curProp.objectReferenceValue = newValue;
                so.ApplyModifiedProperties();
            });

            headerRow.Add(objField);

            // Rest of the method remains the same...
            // Optional type label
            if (!string.IsNullOrEmpty(itemTypeName))
            {
                var typeLabel = new Label(itemTypeName);
                typeLabel.name = "itemTypeLabel";
                typeLabel.AddToClassList("type-label");
                headerRow.Add(typeLabel);
            }

            // Remove button
            var buttonsContainer = new VisualElement();
            buttonsContainer.AddToClassList("buttons-container");
            headerRow.Add(buttonsContainer);

            var removeBtn = new Button(removeCallback);
            removeBtn.text = "×";
            removeBtn.AddToClassList("icon-button");
            removeBtn.AddToClassList("remove-button");
            buttonsContainer.Add(removeBtn);

            return row;
        }

        /// <summary>
        /// Creates a visual element with empty message styling
        /// </summary>
        public static Label CreateEmptyMessage(string message)
        {
            var emptyMessage = new Label(message);
            emptyMessage.AddToClassList("empty-list-message");
            return emptyMessage;
        }

        /// <summary>
        /// Creates a container for expandable content
        /// </summary>
        public static VisualElement CreateExpandableContainer()
        {
            var container = new VisualElement();
            container.style.display = DisplayStyle.None;
            container.AddToClassList("nested-inspector");
            container.AddToClassList("compact");
            return container;
        }

        /// <summary>
        /// Sets up expandable editor for a ScriptableObject
        /// </summary>
        public static void SetupExpandableEditor(VisualElement clickableElement, VisualElement expandableContainer, ScriptableObject target)
        {
            var editor = Editor.CreateEditor(target);
            VisualElement inspectorElement;

            try { inspectorElement = editor.CreateInspectorGUI(); }
            catch { inspectorElement = new IMGUIContainer(editor.OnInspectorGUI); }

            inspectorElement.Bind(editor.serializedObject);
            expandableContainer.Add(inspectorElement);

            // Toggle expandable content on click
            clickableElement.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.target is Button) return; // Don't toggle when clicking buttons

                // Toggle display
                var isExpanded = expandableContainer.style.display.value == DisplayStyle.Flex;
                expandableContainer.style.display = isExpanded ? DisplayStyle.None : DisplayStyle.Flex;

                // Toggle selected styling
                if (!isExpanded)
                    clickableElement.AddToClassList("list-item-selected");
                else
                    clickableElement.RemoveFromClassList("list-item-selected");
            });
        } 
    }
}