using UnityEditor;
using UnityEngine;

namespace Omnix.Hierarchy
{
    public class EditObjectInfoWindow : EditorWindow
    {
        private GameObject _target;
        private string _currentText;
        
        private void OnEnable()
        {
            _target = Selection.activeGameObject;
            if (_target == null)
            {
                Close();
                return;
            }

            if (_target.TryGetComponent(out ObjectInfo info))
            {
                _currentText = info.info;
            }
            else
            {
                _currentText = "";
            }
        }

        private void OnGUI()
        {
            if (_target == null)
            {
                EditorGUILayout.HelpBox("Something went wrong", MessageType.Error);
                return;
            }

            _currentText = EditorGUILayout.TextArea(_currentText);

            if (GUILayout.Button("Save"))
            {
                Save();
            }

        }

        private void Save()
        {
            if (_target == null || string.IsNullOrEmpty(_currentText))
            {
                EditorUtility.DisplayDialog("Error", "Something went wrong, try again", "Okay");
                return;
            }

            if (_target.TryGetComponent(out ObjectInfo info))
            {
                Undo.RecordObject(info, "Updated Object Info");
                info.info = _currentText;
                EditorUtility.SetDirty(_target);
            }
            else
            {
                info = _target.AddComponent<ObjectInfo>();
                Undo.RegisterCreatedObjectUndo(info, "Added ObjectInfo Component");
                info.info = _currentText;
                EditorUtility.SetDirty(_target);
            }
            Close();
        }
    }
}