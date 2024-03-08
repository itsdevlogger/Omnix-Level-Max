using System;
using UnityEditor;
using UnityEngine;

namespace Omnix.Hierarchy
{
    public static class HierarchyUtils
    {
        public static readonly Type HIERARCHY_TYPE = Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor");
        
        #region Fields: Colors
        private static readonly Color DEFAULT_COLOR = new Color(0.7843f, 0.7843f, 0.7843f);
        private static readonly Color DEFAULT_PRO_COLOR = new Color(0.2196f, 0.2196f, 0.2196f);

        private static readonly Color SELECTED_COLOR = new Color(0.22745f, 0.447f, 0.6902f);
        private static readonly Color SELECTED_PRO_COLOR = new Color(0.1725f, 0.3647f, 0.5294f);

        private static readonly Color SELECTED_UN_FOCUSED_COLOR = new Color(0.68f, 0.68f, 0.68f);
        private static readonly Color SELECTED_UN_FOCUSED_PRO_COLOR = new Color(0.3f, 0.3f, 0.3f);

        private static readonly Color HOVERED_COLOR = new Color(0.698f, 0.698f, 0.698f);
        private static readonly Color HOVERED_PRO_COLOR = new Color(0.2706f, 0.2706f, 0.2706f);
        #endregion

        #region Fields: Icon Names
        private const string EMPTY_ICON = "FolderEmpty Icon";
        private const string EMPTY_PRO_ICON = "FolderEmpty On Icon";
        private const string FILED_ICON = "FolderOpened Icon";
        private const string FILED_PRO_ICON = "FolderOpened On Icon";
        #endregion
        
        public static Color GetBackgroundColor(bool isSelected, bool isHovered, bool isWindowFocused)
        {
            if (isSelected)
            {
                if (isWindowFocused) return EditorGUIUtility.isProSkin ? SELECTED_PRO_COLOR : SELECTED_COLOR;
                return EditorGUIUtility.isProSkin ? SELECTED_UN_FOCUSED_PRO_COLOR : SELECTED_UN_FOCUSED_COLOR;
            }

            if (isHovered) return EditorGUIUtility.isProSkin ? HOVERED_PRO_COLOR : HOVERED_COLOR;
            return EditorGUIUtility.isProSkin ? DEFAULT_PRO_COLOR : DEFAULT_COLOR;
        }

        public static GUIContent GetFolderIcon(bool isFilled)
        {
            if (isFilled) return EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? FILED_PRO_ICON : FILED_ICON);
            return EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? EMPTY_PRO_ICON : EMPTY_ICON);
        }

        public static void StartRenamingObject(GameObject target)
        {
            Selection.activeGameObject = target;
            EditorApplication.delayCall += () =>
            {
                if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == HIERARCHY_TYPE)
                {
                    EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Rename"));
                }
            };
        }
    }
}