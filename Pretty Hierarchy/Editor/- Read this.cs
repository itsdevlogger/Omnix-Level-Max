using System;
using System.Collections.Generic;
using System.IO;
using MenuManagement.Base;
using MenuManagement.Behaviours;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class SetIconsWithInheritanceWindow
{
    private const string MENU_PATH = "Assets/Copy asset GUID";

    /// <summary>
    /// If you want to assign some icon to a base class and all of its child classes,
    /// Place it here.
    /// Key: Type of Base Class
    /// Value: GUID of icon (Right click on icon and click "Copy asset GUID" option to get Icon Guid)
    /// </summary>
    private static readonly Dictionary<Type, string> GENERIC_TYPES_ICONS_GUID = new Dictionary<Type, string>()
    {
        { typeof(BaseMenu), "52c5f97f77263b040b2c4f7961b64b4c" },
        { typeof(BaseDynamicMenu<,,>), "52c5f97f77263b040b2c4f7961b64b4c" },
        // { typeof(ITrigger), "2edb8a89c739ee745ae8a7e4b2f6e9c2" },
        // { typeof(BaseAction), "c03cd662dad067c48a0821d17531b828" },
    };

    private Type _baseType;
    private Texture2D _selectedTexture;
    private string _errorMessage;

    #region Editor Menu
    [MenuItem(MENU_PATH, priority = 0)]
    public static void ShowMenuItem()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        GUIUtility.systemCopyBuffer = AssetDatabase.AssetPathToGUID(path);
        EditorUtility.DisplayDialog("Done", "Copied object guid to clipboard", "Okay");
    }


    [MenuItem(MENU_PATH, validate = true)]
    public static bool ShowMenuItemValidation()
    {
        return Selection.objects.Length == 1;
    }
    #endregion

    #region Update All Script Icons
    static SetIconsWithInheritanceWindow()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        UpdateIcons();
    }

    private static void UpdateIcons()
    {
        AssetDatabase.StartAssetEditing();
        foreach (string scriptGuid in AssetDatabase.FindAssets("t:MonoScript"))
        {
            string path = AssetDatabase.GUIDToAssetPath(scriptGuid);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (TryGetIcon(script.GetClass(), out Texture2D icon) == false) continue;

            var importer = (MonoImporter)AssetImporter.GetAtPath(path);
            if (importer == null) continue;
            Texture2D texture2D = importer.GetIcon();
            if (texture2D != null && texture2D.name != "d_cs Script Icon")
            {
                continue;
            }

            importer.SetIcon(icon);
            AssetDatabase.ImportAsset(path);
        }

        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    private static bool TryGetIcon(Type type, out Texture2D icon)
    {
        foreach (KeyValuePair<Type, string> pair in GENERIC_TYPES_ICONS_GUID)
        {
            if (pair.Key.IsAssignableFrom(type) == false) continue;
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(pair.Value));
            return icon != null;
        }

        icon = null;
        return false;
    }
    #endregion
}