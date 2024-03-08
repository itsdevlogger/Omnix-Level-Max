using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Omnix.Hierarchy
{
    public static class FolderHandler
    {
        [MenuItem("GameObject/Create Folder", true)]
        private static bool ValidateCreate() => Selection.objects.Length <= 1;

        [MenuItem("GameObject/Turn Into Folder", true)]
        private static bool ValidateTurnInto() => Selection.objects.Length == 1;

        [MenuItem("GameObject/Create Folder", false, -1)]
        private static void Create()
        {
            var folder = new GameObject("New Folder");
            Undo.RegisterCreatedObjectUndo(folder, folder.name);
            Transform activeTransform = Selection.activeTransform;
            if (activeTransform != null)
            {
                folder.transform.SetParent(activeTransform);
                if (activeTransform.GetComponentInParent<RectTransform>(true) != null)
                {
                    folder.AddComponent<RectTransform>();
                }
            }

            ResetPosition(folder);
            BasicSetup(folder);
            HierarchyUtils.StartRenamingObject(folder);
        }

        [MenuItem("GameObject/Turn Into Folder", false, -2)]
        private static void TurnTo()
        {
            GameObject active = Selection.activeGameObject;
            if (active == null) return;

            if (active.GetComponents<Component>().Length > 1)
            {
                bool shouldContinue = EditorUtility.DisplayDialog("Confirm", $"The object {active.name} has component(s) other than Transform.\nAll these components will be destroyed.\nWish to continue?", "Yes", "No");
                if (shouldContinue == false) return;
            }

            foreach (Component component in active.GetComponents<Component>().Where(component => component is not Transform).ToList())
            {
                Object.DestroyImmediate(component);
            }
            
            ResetPositionAdvanced(active.transform);
            BasicSetup(active);
        }

        
        private static void BasicSetup(GameObject folder)
        {
            EnsureTagExists();
            folder.tag = Settings.FOLDER_TAG;

            EditorUtility.SetDirty(folder);
            Selection.activeGameObject = folder;
        }

        public static void EnsureTagExists()
        {
            Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (asset == null || asset.Length <= 0)
            {
                Debug.LogError("Something is seriously wrong. TagManager does not exit.");
                return;
            }

            var so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == Settings.FOLDER_TAG)
                {
                    return; // Tag already present, nothing to do.
                }
            }

            tags.arraySize++;
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = Settings.FOLDER_TAG;
            so.ApplyModifiedProperties();
            so.Update();
        }

        private static void ResetPosition(GameObject target)
        {
            if (target.TryGetComponent(out RectTransform rect) == false)
            {
                if (PrettyHierarchy.IsInsidePrefab == false && target.GetComponentInParent<RectTransform>(true) != null)
                {
                    rect = target.AddComponent<RectTransform>();
                }
            }
            
            if (rect != null)
            {
                rect.anchorMax = Vector2.one;
                rect.anchorMin = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
                rect.pivot = new Vector2(0.5f, 0.5f);
            }
            else
            {
                target.transform.localPosition = Vector3.zero;
            }

            target.transform.localRotation = Quaternion.identity;
            target.transform.localScale = Vector3.one;
        }

        private static void ResetPositionAdvanced(Transform target)
        {
            var children = new Transform[target.childCount];
            for (int i = 0; i < target.childCount; i++)
            {
                children[i] = target.GetChild(i);
            }

            foreach (Transform child in children)
            {
                Undo.RecordObject(child, "Changed parent");
                child.SetParent(null);
            }

            ResetPosition(target.gameObject);

            foreach (Transform child in children)
            {
                Undo.RecordObject(child, "Changed parent");
                child.SetParent(target);
            }
        }
        
        private static void ActiveButton(GameObject target, Rect selectionRect)
        {
            Rect rect = selectionRect;
            rect.xMax = rect.width;
            rect.x = selectionRect.xMax;

            GUI.color = new Color(0, 0, 0, 0f);

            if (GUI.Button(rect, ""))
            {
                target.SetActive(!target.activeInHierarchy);
            }

            GUI.color = new Color(1, 1, 1, 1);


            //label placement calc
            rect.x = selectionRect.xMax;

            //show label based on active state
            GUI.Label(rect, target.activeInHierarchy ? EditorGUIUtility.IconContent("d_toggle_on_focus") : EditorGUIUtility.IconContent("d_toggle_bg"));
        }

        public static void Handle(GameObject target, Rect rect)
        {
            ActiveButton(target, rect);
            ResetPosition(target);

            target.transform.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
            target.hideFlags = HideFlags.HideInInspector;
            SceneVisibilityManager.instance.DisablePicking(target, false);

            PrettyHierarchy.HideDefaultIcon();
            EditorGUI.LabelField(rect, HierarchyUtils.GetFolderIcon(target.transform.childCount > 0));
        }
    }
}