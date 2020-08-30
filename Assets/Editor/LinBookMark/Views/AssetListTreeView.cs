using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LinBookMark
{
    public class AssetListTreeView:TreeView
    {
        public IList<string> pathList;
        private EditorWindow parentWindow;
        public AssetListTreeView(TreeViewState state) : base(state)
        {
        }

        public AssetListTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        public void SetAssetPathList(IList<string> assetPaths)
        {
            pathList = assetPaths;
            BuildRoot();
            Reload();
        }
        
        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(0,-1,"assetRoot");
            var allItems = GetAssetItems();
            // Utility method that initializes the TreeViewItem.children and -parent for all items.
            SetupParentsAndChildrenFromDepths(root, allItems);
            return root;
        }

        IList<TreeViewItem> GetAssetItems()
        {
            List<TreeViewItem> result = new List<TreeViewItem>();
            if (pathList != null)
            {
                foreach (var path in pathList)
                {
                    if (Directory.Exists(path))
                    {
                        var subFiles = Directory.GetFiles(path).Where(x => x.EndsWith(".meta")==false);
                        foreach (var subFile in subFiles)
                        {
                            AddOneFileItem(subFile, result);
                        }
                    }
                    else
                    {
                        if (File.Exists(path))
                        {
                            AddOneFileItem(path, result);
                        }
                    }
                }
            }
            return result;
        }

        private static void AddOneFileItem(string filePath, List<TreeViewItem> result)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(filePath);
            if (obj)
            {
                TreeViewItem item = new TreeViewItem(obj.GetInstanceID(), 0, obj.name);
                item.icon = (Texture2D) AssetDatabase.GetCachedIcon(filePath);
                result.Add(item);
            }
        }


        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
            List<string> paths = new List<string>();
            List<Object> objects = new List<Object>();
            foreach (var draggedItemID in args.draggedItemIDs)
            {
                var obj = EditorUtility.InstanceIDToObject(draggedItemID);
                if (obj)
                {
                    paths.Add(AssetDatabase.GetAssetPath(obj));
                    objects.Add(obj);
                }
            }
            
            if (objects.Count > 0)
            {
                string title = objects.Count > 1 ? "<Multiple>" : objects.GetType().Name;
                DragAndDrop.StartDrag(title);
            }
            else
            {
                DragAndDrop.StartDrag("nothing to drag");
            }

            DragAndDrop.paths = paths.ToArray();
            DragAndDrop.objectReferences = objects.ToArray();

        }

        string GetTargetAssetPath(DragAndDropArgs args)
        {
            string result = string.Empty;
            if (args.parentItem != null)
            {
                var obj = EditorUtility.InstanceIDToObject(args.parentItem.id);
                if (obj)
                {
                    result = AssetDatabase.GetAssetPath(obj);
                }
            }
            return result;
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            var draggedObjects = DragAndDrop.objectReferences;
            if (draggedObjects.Length == 0 && DragAndDrop.paths.Length==0 )
            {
                return DragAndDropVisualMode.None;
            }

            if (args.performDrop)
            {
                DragDropUtil.FixDragDropFromOutSideProject();
                if (DragDropUtil.IsDraggingNonProjectPath())
                {
                    CheckImportAssetByPath(args);
                }else  if (DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0)
                {
                    AssetOperationUtil.CreatePrefab(GetSharedParentFolderPath());
                }
                else
                {
                    HandleOperationInSideProject(args);
                }
            }

            return DragAndDropVisualMode.Move;
        }

        private void HandleOperationInSideProject(DragAndDropArgs args)
        {
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                {
                    
                    break;
                }
                case DragAndDropPosition.BetweenItems:
                case DragAndDropPosition.OutsideItems:
                    var parentFolderPath = GetSharedParentFolderPath();
                    if (string.IsNullOrEmpty(parentFolderPath))
                    {
                        EditorUtility.DisplayDialog("add item ?", "Multiple   folders or none  selected ,I don't know where to drop ",
                            "Got it");
                    }
                    else
                    {
                        DragDropUtil.MoveDraggingAssetToFolder(parentFolderPath);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void CheckImportAssetByPath(DragAndDropArgs args)
        {
            var assetPath = GetTargetAssetPath(args);
            if (args.dragAndDropPosition == DragAndDropPosition.UponItem)
            {
                DragDropUtil.TryReplaceAsset(assetPath);
            }
            else
            {
                var parentFolderPath = GetSharedParentFolderPath();
                if (string.IsNullOrEmpty(parentFolderPath))
                {
                    EditorUtility.DisplayDialog("add item ?", "Multiple   folders or none  selected ,I don't know where to drop ",
                        "Got it");
                }
                else
                {
                    DragDropUtil.TryImportDragAssets(parentFolderPath);
                }
            }
        }

        string GetSharedParentFolderPath()
        {
            var result = string.Empty;
            foreach (var path in pathList)
            {
                var parentPath = GetFolderPathFromPathString(path);
                if (string.IsNullOrEmpty(parentPath)==false)
                {
                    
                    if (string.IsNullOrEmpty(result))
                    {
                        result = parentPath;
                    }
                    else
                    {
                        if (parentPath != result)
                        {
                            // only return the parent path when all asset share same one
                            return string.Empty;
                        }
                    }
                }
            }
            return result;
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var obj = EditorUtility.InstanceIDToObject(id);
            EditorGUIUtility.PingObject(obj);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            foreach (var selectedId in selectedIds)
            {
                var obj = EditorUtility.InstanceIDToObject(selectedId);
                if (obj)
                {
                    Selection.SetActiveObjectWithContext(obj,null);
                }
            }
        }


        private static string GetFolderPathFromPathString(string path)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
            else
            {
                if (File.Exists(path))
                {
                    return Path.GetDirectoryName(path);
                }
            }
            return string.Empty;
        }

        public string GetParentFolderDesc()
        {
            if (pathList.Count == 1)
            {
                return pathList[0];
            }else if (pathList.Count == 0)
            {
                return "No root selected";
            }
            else
            {
                var shareFolder = GetSharedParentFolderPath();
                if (string.IsNullOrEmpty(shareFolder))
                {
                    return "Showing MultipleFolders ...";
                }
                else
                {
                    return shareFolder;
                }
            }
        }
    }
}