using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LinBookMark
{
    public class LinBookMarkSettingWnd : EditorWindow
    {
        [MenuItem("KyleKit/LinBookMarkSetting")]
        private static void ShowWindow()
        {
            var window = GetWindow<LinBookMarkSettingWnd>();
            window.titleContent = new GUIContent("Settings");
            window.Show();
        }

        private const string prefKey = "KyleTreeViewCreatePrefabRoot";
        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Prefab to Hierarchy Root： ",GUILayout.Width(170));
                EditorPrefs.SetString(prefKey,EditorGUILayout.TextField(EditorPrefs.GetString(prefKey, "")));
            }
        }
    }
}