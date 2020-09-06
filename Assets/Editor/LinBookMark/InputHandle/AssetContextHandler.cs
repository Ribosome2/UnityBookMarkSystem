using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LinBookMark
{
    public class AssetContextHandler:IAssetContextHandler
    {
        public static  void ShowContextMenu(string path)
        {
            Debug.Log("show "+path);
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("PingProject"), false, delegate (object data) {
                EditorGUIUtility.PingObject(obj);
            }, null);
            menu.AddItem(new GUIContent("Copy"), false, delegate (object data) {
                CommandsHandler.ExecuteCommand<CopyAssetCommand>(new object[] {path});;
            }, null);
            if (KyleSelections.CopyPaths.Count > 0)
            {
                menu.AddItem(new GUIContent("Paste"), false, delegate (object data) {
                    CommandsHandler.ExecuteCommand<PasteAssetCommand>(new object[] {path});;
                }, null);
            }
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            AddSpriteMenu(sprite, menu);
            menu.ShowAsContext();
        }

        private static void AddSpriteMenu(Sprite sprite, GenericMenu menu)
        {
            menu.AddItem(new GUIContent("SpriteEditor"), false, delegate(object data)
            {
                Selection.activeObject = sprite;
                Type type = System.Type.GetType("UnityEditor.SpriteEditorWindow,UnityEditor");
                EditorWindow.GetWindow(type);
            }, null);

            menu.AddItem(new GUIContent("PingHierarchy"), false,
                delegate(object data) { CommandsHandler.ExecuteCommand<PingSpriteInHierarchyCommand>(new object[] {sprite}); },
                null);
        }

        public void HandlerAssetContextClick(string path)
        {
            ShowContextMenu(path);
        }
    }
}