using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Omnix.Hierarchy
{
    [InitializeOnLoad]
    public static class PrettyHierarchy
    {
        private static bool hierarchyHasFocus;
        public static bool IsInsidePrefab { get; private set; }
        private static Color curBackColor;
        private static Rect curRect;

        static PrettyHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            EditorApplication.update += ApplicationUpdate;
        }

        private static void ApplicationUpdate()
        {
            hierarchyHasFocus = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == HierarchyUtils.HIERARCHY_TYPE;
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect rect)
        {
            var target = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (target == null) return;

            GameObject source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target);
            IsInsidePrefab = source != null;
            bool isSelected = Selection.instanceIDs.Contains(instanceID);
            bool isHovering = rect.Contains(Event.current.mousePosition);
            
            curRect = rect;
            curBackColor = HierarchyUtils.GetBackgroundColor(isSelected, isHovering, hierarchyHasFocus);
            FolderHandler.EnsureTagExists();
            if (target.CompareTag(Settings.FOLDER_TAG)) FolderHandler.Handle(target, rect);
            else IconsHandler.Handle(target, rect);
        }
        
        public static void HideDefaultIcon()
        {
            Rect backRect = new Rect(curRect.x, curRect.y, 18.5f, curRect.height);
            EditorGUI.DrawRect(backRect, curBackColor);
        }
    }
}