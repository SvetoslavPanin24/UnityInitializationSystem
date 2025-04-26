using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using System.Linq;

namespace InitializationSystem.View
{
    /// <summary>
    /// Handles drag and drop operations for editor UI elements
    /// </summary>
    public static class DragAndDropHandler
    {
        /// <summary>
        /// Sets up drag and drop handling for a visual element
        /// </summary>
        /// <param name="container">Container to setup drag and drop for</param>
        /// <param name="acceptedType">Type name filter (e.g. "Module", "Handler")</param>
        /// <param name="onObjectsDropped">Callback when objects are dropped</param>
        public static void Setup(VisualElement container, string acceptedType, Action<Object[]> onObjectsDropped)
        {
            if (container == null) return;

            // Remove existing callbacks to prevent duplicates
            container.UnregisterCallback<DragEnterEvent>(OnDragEnter);
            container.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            container.UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
            container.UnregisterCallback<DragPerformEvent>(OnDragPerform);

            // Store data in userData for callbacks
            container.userData = new DragDropData
            {
                AcceptedType = acceptedType,
                OnObjectsDropped = onObjectsDropped
            };

            // Register new callbacks
            container.RegisterCallback<DragEnterEvent>(OnDragEnter);
            container.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            container.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            container.RegisterCallback<DragPerformEvent>(OnDragPerform);

            // Add visual drop zone indicator
            CreateDropZone(container, acceptedType);
        }

        private static void CreateDropZone(VisualElement container, string typeName)
        {
            // Remove any existing drop zone
            var existingDropZone = container.Q("drop-zone");
            if (existingDropZone != null) container.Remove(existingDropZone);

            // Create new drop zone
            var dropZone = new VisualElement();
            dropZone.name = "drop-zone";
            dropZone.AddToClassList("drop-zone");
            dropZone.style.display = DisplayStyle.None;
            dropZone.style.position = Position.Absolute;
            dropZone.style.left = dropZone.style.right = dropZone.style.top = dropZone.style.bottom = 0;
            dropZone.style.backgroundColor = new Color(0.2f, 0.6f, 1f, 0.3f);

            // Add drop zone label
            var label = new Label($"Drop {typeName}s Here");
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.color = Color.white;
            label.style.fontSize = 16;
            label.style.position = Position.Absolute;
            label.style.left = label.style.right = label.style.top = label.style.bottom = 0;
            dropZone.Add(label);

            container.Add(dropZone);
        }

        private static void OnDragEnter(DragEnterEvent evt)
        {
            var container = evt.currentTarget as VisualElement;
            var dropZone = container?.Q("drop-zone");
            if (dropZone != null) dropZone.style.display = DisplayStyle.Flex;
        }

        private static void OnDragLeave(DragLeaveEvent evt)
        {
            var container = evt.currentTarget as VisualElement;
            var dropZone = container?.Q("drop-zone");
            if (dropZone != null) dropZone.style.display = DisplayStyle.None;
        }

        private static void OnDragUpdated(DragUpdatedEvent evt)
        {
            var container = evt.currentTarget as VisualElement;
            var data = container?.userData as DragDropData;
            if (data == null) return;

            bool validDrag = DragAndDrop.objectReferences.Any(obj =>
                obj is ScriptableObject && obj.GetType().Name.Contains(data.AcceptedType));

            DragAndDrop.visualMode = validDrag ?
                DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

            evt.StopPropagation();
        }

        private static void OnDragPerform(DragPerformEvent evt)
        {
            var container = evt.currentTarget as VisualElement;
            var data = container?.userData as DragDropData;
            if (data == null) return;

            var validObjects = DragAndDrop.objectReferences
                .Where(obj => obj is ScriptableObject && obj.GetType().Name.Contains(data.AcceptedType))
                .ToArray();

            if (validObjects.Length > 0)
            {
                data.OnObjectsDropped?.Invoke(validObjects);
                DragAndDrop.AcceptDrag();
            }

            var dropZone = container.Q("drop-zone");
            if (dropZone != null) dropZone.style.display = DisplayStyle.None;

            evt.StopPropagation();
        }

        private class DragDropData
        {
            public string AcceptedType;
            public Action<Object[]> OnObjectsDropped;
        }
    }
}